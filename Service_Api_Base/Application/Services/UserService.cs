using Application.Abstractions.Persistence;
using Application.Exceptions;
using Application.GetMap;
using Application.IServices;
using Application.Request;
using Domain.Aggregates.User;
using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateUser(UserRequest request)
        {
            try
            {
                var exists = await _unitOfWork.Repository<Users>().FirstOrDefault(
                    filter: m => m.UserCode == request.UserCode && m.IsDelete != true);
                if (exists != null)
                    throw new DomainException("Mã người dùng đã tồn tại");

                var code = EntityCode.Create(request.UserCode, "Mã người dùng");
                var aggregate = UserAggregate.Create(code, request.UserName, request.Note);
                await _unitOfWork.Repository<Users>().Insert(aggregate.Root);

                if (request.ListDepartmentOfUser?.Count > 0)
                {
                    var assignments = request.ListDepartmentOfUser
                        .Select(item => UserAggregate.CreateDepartmentAssignment(
                            aggregate.Root.UserId,
                            item.DepartmentId,
                            item.PositionId))
                        .ToList();

                    await _unitOfWork.Repository<UserDepartments>().InsertRange(assignments);
                }

                return await _unitOfWork.Commit();
            }
            catch (DomainException ex)
            {
                throw new AppException(ex.Message);
            }
        }

        public async Task<bool> UpdateUser(UserRequest request)
        {
            try
            {
                var entity = await _unitOfWork.Repository<Users>().FirstOrDefault(
                    filter: m => m.UserId == request.UserId);
                var aggregate = UserAggregate.FromEntity(entity);
                aggregate.Update(request.UserName, request.Note, request.IsActive, request.IsDelete);
                aggregate.UpdateCode(EntityCode.Create(request.UserCode, "Mã người dùng"));
                _unitOfWork.Repository<Users>().Update(aggregate.Root);

                if (request.ListDepartmentOfUser?.Count > 0)
                {
                    var listUserDepartments = _unitOfWork.Repository<UserDepartments>()
                        .Get(filter: m => m.UserId == request.UserId);

                    foreach (var existing in listUserDepartments)
                    {
                        if (!request.ListDepartmentOfUser.Any(ud => ud.DepartmentId == existing.DepartmentId))
                        {
                            UserAggregate.SoftDeleteDepartmentAssignment(existing);
                            _unitOfWork.Repository<UserDepartments>().Update(existing);
                        }
                    }

                    foreach (var item in request.ListDepartmentOfUser)
                    {
                        var existing = listUserDepartments.FirstOrDefault(
                            ud => ud.DepartmentId == item.DepartmentId);

                        if (existing is null)
                        {
                            await _unitOfWork.Repository<UserDepartments>().Insert(
                                UserAggregate.CreateDepartmentAssignment(
                                    request.UserId, item.DepartmentId, item.PositionId));
                        }
                        else
                        {
                            UserAggregate.UpdateDepartmentAssignment(existing, item.PositionId);
                            _unitOfWork.Repository<UserDepartments>().Update(existing);
                        }
                    }
                }

                return await _unitOfWork.Commit();
            }
            catch (DomainException ex)
            {
                throw new AppException(ex.Message);
            }
        }

        public async Task<bool> DeleteUser(Guid userId)
        {
            try
            {
                var entity = await _unitOfWork.Repository<Users>().FirstOrDefault(filter: m => m.UserId == userId);
                var aggregate = UserAggregate.FromEntity(entity);
                aggregate.SoftDelete();
                _unitOfWork.Repository<Users>().Update(aggregate.Root);
                return await _unitOfWork.Commit();
            }
            catch (DomainException ex)
            {
                throw new AppException(ex.Message);
            }
        }

        public async Task<UserInfoGetMap> GetUserById(Guid userId)
        {
            return await _unitOfWork.Repository<Users>()
                .Get(
                    filter: m => m.UserId == userId,
                    include: x => x
                        .Include(x => x.UserDepartments).ThenInclude(x => x.Departments)
                        .Include(m => m.UserDepartments).ThenInclude(m => m.Positions))
                .Select(u => new UserInfoGetMap
                {
                    UserId = userId,
                    UserName = u.UserName,
                    UserCode = u.UserCode,
                    CreateDate = u.CreateDate,
                    UpdateDate = u.UpdateDate,
                    IsActive = u.IsActive,
                    IsDelete = u.IsDelete,
                    ListDepartmentOfUser = u.UserDepartments.Select(x => new DepartmentOfUserGetMap
                    {
                        DepartmentId = x.DepartmentId,
                        DepartmentCode = x.Departments != null ? x.Departments.DepartmentCode : null,
                        DepartmentName = x.Departments != null ? x.Departments.DepartmentName : null,
                        PosititonId = x.PositionId,
                        PosititonCode = x.Positions != null ? x.Positions.PositionCode : null,
                        PositionName = x.Positions != null ? x.Positions.PositionName : null
                    }).ToList()
                }).FirstOrDefaultAsync();
        }
    }
}
