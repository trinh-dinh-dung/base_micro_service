using Application.Abstractions.Persistence;
using Application.Common.Specifications;
using Application.Exceptions;
using Application.GetMap;
using Application.IServices;
using Application.Request;
using Domain.Aggregates.Department;
using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> GetPaging(DepartmentRequest request)
        {
            Expression<Func<Departments, bool>> finalEntityFilter = department => department.Note != null;
            Expression<Func<Departments, bool>> entityFilter = department => department.IsActive != false;
            Expression<Func<Departments, bool>> entityFilterOr = department => department.IsDelete != true;
            finalEntityFilter = ExpressionCombiner.And(finalEntityFilter, entityFilter);
            finalEntityFilter = ExpressionCombiner.Or(finalEntityFilter, entityFilterOr);
            await _unitOfWork.Repository<Departments>().FirstOrDefault(filter: finalEntityFilter);

            return await CreateDepartment(request);
        }

        public async Task<bool> CreateDepartment(DepartmentRequest request)
        {
            try
            {
                var exists = await _unitOfWork.Repository<Departments>().FirstOrDefault(
                    filter: m => m.DepartmentCode == request.DepartmentCode.Trim() && m.IsDelete != true);
                if (exists != null)
                    throw new DomainException("Mã phòng ban đã tồn tại");

                var code = EntityCode.Create(request.DepartmentCode, "Mã phòng ban");
                var aggregate = DepartmentAggregate.Create(
                    code,
                    request.DepartmentName,
                    request.ParentId,
                    request.Note);

                await _unitOfWork.Repository<Departments>().Insert(aggregate.Root);
                return await _unitOfWork.Commit();
            }
            catch (DomainException ex)
            {
                throw new AppException(ex.Message);
            }
        }

        public async Task<bool> UpdateDepartment(DepartmentRequest request)
        {
            try
            {
                var entity = await _unitOfWork.Repository<Departments>().FirstOrDefault(
                    filter: m => m.DepartmentId == request.DepartmentId);
                var aggregate = DepartmentAggregate.FromEntity(entity);
                aggregate.Update(
                    request.DepartmentName,
                    request.ParentId,
                    request.Note,
                    request.IsActive,
                    request.IsDelete);

                _unitOfWork.Repository<Departments>().Update(aggregate.Root);
                return await _unitOfWork.Commit();
            }
            catch (DomainException ex)
            {
                throw new AppException(ex.Message);
            }
        }

        public async Task<bool> DeleteDepartment(Guid departmentId)
        {
            try
            {
                var entity = await _unitOfWork.Repository<Departments>().FirstOrDefault(
                    filter: m => m.DepartmentId == departmentId);
                var aggregate = DepartmentAggregate.FromEntity(entity);
                aggregate.SoftDelete();

                _unitOfWork.Repository<Departments>().Update(aggregate.Root);
                return await _unitOfWork.Commit();
            }
            catch (DomainException ex)
            {
                throw new AppException(ex.Message);
            }
        }

        public async Task<object> GetListDepartmentByParentId(Guid? parentId)
        {
            const string hierarchySqlRoot = @"
WITH RECURSIVE DepartmentHierarchy AS (
    SELECT ""DepartmentId"", ""DepartmentName"", ""DepartmentCode"", ""ParentId"", ""Note"", 1 AS Level
    FROM ""Departments""
    WHERE {0}
    UNION ALL
    SELECT d.""DepartmentId"", d.""DepartmentName"", d.""DepartmentCode"", d.""ParentId"", d.""Note"", dh.Level + 1 AS Level
    FROM ""Departments"" d
    INNER JOIN DepartmentHierarchy dh ON dh.""DepartmentId"" = d.""ParentId""
)
SELECT * FROM DepartmentHierarchy;";

            string whereClause;
            object[] parameters;

            if (parentId is null)
            {
                whereClause = @"""ParentId"" IS NULL";
                parameters = null;
            }
            else
            {
                whereClause = @"""ParentId"" = @parentId";
                parameters = new object[]
                {
                    new NpgsqlParameter("@parentId", NpgsqlDbType.Uuid) { Value = parentId.Value }
                };
            }

            var query = string.Format(hierarchySqlRoot, whereClause);
            return await _unitOfWork.Repository<object>().RawSqlQuery(query, MapDepartmentTree, parameters);
        }

        public List<DepartmentTree> BuildDepartmentTree(List<DepartmentTree> departments, Guid? parentId)
        {
            var result = new List<DepartmentTree>();
            foreach (var department in departments.Where(d => d.ParentId == parentId))
            {
                var subDepartments = BuildDepartmentTree(departments, department.DepartmentId);
                department.SubDepartments.AddRange(subDepartments);
                result.Add(department);
            }

            return result;
        }

        private static DepartmentTree MapDepartmentTree(DbDataReader x) => new()
        {
            DepartmentId = Convert.IsDBNull(x[0]) ? Guid.Empty : (Guid)x[0],
            DepartmentName = Convert.IsDBNull(x[1]) ? null : (string)x[1],
            DepartmentCode = Convert.IsDBNull(x[2]) ? null : (string)x[2],
            ParentId = Convert.IsDBNull(x[3]) ? Guid.Empty : (Guid)x[3],
            Note = Convert.IsDBNull(x[4]) ? null : x[4] as string,
            Level = Convert.IsDBNull(x[5]) ? 0 : (int?)x[5],
        };
    }
}
