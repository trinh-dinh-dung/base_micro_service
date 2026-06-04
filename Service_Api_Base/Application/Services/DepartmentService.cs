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
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> GetPaging(DepartmentRequest request, CancellationToken cancellationToken = default)
        {
            Expression<Func<Departments, bool>> finalEntityFilter = department => department.Note != null;
            Expression<Func<Departments, bool>> entityFilter = department => department.IsActive != false;
            Expression<Func<Departments, bool>> entityFilterOr = department => department.IsDelete != true;
            finalEntityFilter = ExpressionCombiner.And(finalEntityFilter, entityFilter);
            finalEntityFilter = ExpressionCombiner.Or(finalEntityFilter, entityFilterOr);
            await _unitOfWork.Repository<Departments>().FirstOrDefault(filter: finalEntityFilter, cancellationToken: cancellationToken);

            return await CreateDepartment(request, cancellationToken);
        }

        public async Task<bool> CreateDepartment(DepartmentRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var exists = await _unitOfWork.Repository<Departments>().FirstOrDefault(
                    filter: m => m.DepartmentCode == request.DepartmentCode.Trim() && m.IsDelete != true,
                    cancellationToken: cancellationToken);
                if (exists != null)
                    throw new DomainException("Mã phòng ban đã tồn tại");

                var code = EntityCode.Create(request.DepartmentCode, "Mã phòng ban");
                var aggregate = DepartmentAggregate.Create(
                    code,
                    request.DepartmentName,
                    request.ParentId,
                    request.Note);

                await _unitOfWork.Repository<Departments>().Insert(aggregate.Root, cancellationToken);
                return await _unitOfWork.Commit(cancellationToken);
            }
            catch (DomainException ex)
            {
                throw new AppException(ex.Message);
            }
        }

        public async Task<bool> UpdateDepartment(DepartmentRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _unitOfWork.Repository<Departments>().FirstOrDefault(
                    filter: m => m.DepartmentId == request.DepartmentId,
                    cancellationToken: cancellationToken);
                var aggregate = DepartmentAggregate.FromEntity(entity);
                aggregate.Update(
                    request.DepartmentName,
                    request.ParentId,
                    request.Note,
                    request.IsActive,
                    request.IsDelete);

                _unitOfWork.Repository<Departments>().Update(aggregate.Root);
                return await _unitOfWork.Commit(cancellationToken);
            }
            catch (DomainException ex)
            {
                throw new AppException(ex.Message);
            }
        }

        public async Task<bool> DeleteDepartment(Guid departmentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _unitOfWork.Repository<Departments>().FirstOrDefault(
                    filter: m => m.DepartmentId == departmentId,
                    cancellationToken: cancellationToken);
                var aggregate = DepartmentAggregate.FromEntity(entity);
                aggregate.SoftDelete();

                _unitOfWork.Repository<Departments>().Update(aggregate.Root);
                return await _unitOfWork.Commit(cancellationToken);
            }
            catch (DomainException ex)
            {
                throw new AppException(ex.Message);
            }
        }

        public async Task<object> GetListDepartmentByParentId(Guid? parentId, CancellationToken cancellationToken = default)
        {
            const string hierarchySqlRoot = @"
WITH DepartmentHierarchy AS (
    SELECT [DepartmentId], [DepartmentName], [DepartmentCode], [ParentId], [Note], 1 AS Level
    FROM [Departments]
    WHERE {0}
    UNION ALL
    SELECT d.[DepartmentId], d.[DepartmentName], d.[DepartmentCode], d.[ParentId], d.[Note], dh.Level + 1 AS Level
    FROM [Departments] d
    INNER JOIN DepartmentHierarchy dh ON dh.[DepartmentId] = d.[ParentId]
    WHERE d.[IsDelete] != 1
)
SELECT * FROM DepartmentHierarchy;";

            string whereClause;
            object[] parameters;

            if (parentId is null)
            {
                whereClause = @"[ParentId] IS NULL AND [IsDelete] != 1";
                parameters = null;
            }
            else
            {
                whereClause = @"[ParentId] = @parentId AND [IsDelete] != 1";
                parameters = new object[]
                {
                    new SqlParameter("@parentId", SqlDbType.UniqueIdentifier) { Value = parentId.Value }
                };
            }

            var query = string.Format(hierarchySqlRoot, whereClause);
            return await _unitOfWork.Repository<Departments>().RawSqlQuery(query, MapDepartmentTree, parameters, cancellationToken);
        }

        public async Task<List<DepartmentAuditLogRow>> GetDepartmentAuditLogs(Guid departmentId, int limit = 20, string language = "vi", CancellationToken cancellationToken = default)
        {
            await EnsureAuditMetadataTableAsync(cancellationToken);

            var normalizedLimit = limit <= 0 ? 20 : Math.Min(limit, 200);
            var candidateRows = _unitOfWork.Repository<EntityAuditLogs>().Get(
                filter: x => x.EntityName == nameof(Departments)
                             && x.EntityKeys != null
                             && x.Action == "Modified"
                             && x.EntityKeys.Contains(departmentId.ToString()),
                orderBy: q => q.OrderByDescending(x => x.ChangedAtUtc),
                enableTracking: false,
                limit: normalizedLimit * 3)
                .ToListAsync(cancellationToken);

            var candidateRowList = await candidateRows;

            var pageCodes = candidateRowList
                .Select(x => x.PageCode)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var metadataRows = _unitOfWork.Repository<AuditFieldMetadata>().Get(
                filter: x => pageCodes.Contains(x.PageCode),
                enableTracking: false)
                .ToListAsync(cancellationToken);

            var metadataRowList = await metadataRows;

            var metadataLookup = metadataRowList.ToDictionary(
                x => BuildMetadataKey(x.PageCode, x.FieldName),
                x => string.Equals(language, "en", StringComparison.OrdinalIgnoreCase)
                    ? (x.LabelEn ?? x.LabelVi ?? x.FieldName)
                    : (x.LabelVi ?? x.LabelEn ?? x.FieldName),
                StringComparer.OrdinalIgnoreCase);

            return candidateRowList
                .Select(x => ToAuditRow(x, departmentId, metadataLookup))
                .Where(x => x != null)
                .Take(normalizedLimit)
                .ToList();
        }

        public async Task<int> SyncAuditMetadata(AuditMetadataSyncRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.PageCode) || request.Items == null)
            {
                return 0;
            }

            await EnsureAuditMetadataTableAsync(cancellationToken);

            var pageCode = request.PageCode.Trim();
            var changed = 0;

            foreach (var item in request.Items.Where(x => x != null && !string.IsNullOrWhiteSpace(x.FieldName)))
            {
                var sql = @"
MERGE dbo.AuditFieldMetadata AS target
USING (SELECT @PageCode AS PageCode, @FieldName AS FieldName) AS source
ON target.PageCode = source.PageCode AND target.FieldName = source.FieldName
WHEN MATCHED THEN
    UPDATE SET LabelVi = @LabelVi,
               LabelEn = @LabelEn,
               TranslationKey = @TranslationKey,
               UpdatedAtUtc = SYSDATETIMEOFFSET()
WHEN NOT MATCHED THEN
    INSERT (Id, PageCode, FieldName, LabelVi, LabelEn, TranslationKey, CreatedAtUtc, UpdatedAtUtc)
    VALUES (NEWID(), @PageCode, @FieldName, @LabelVi, @LabelEn, @TranslationKey, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET());";

                changed += await _unitOfWork.Repository<AuditFieldMetadata>().ExecuteSqlRawAsync(sql, new object[]
                {
                    new SqlParameter("@PageCode", SqlDbType.NVarChar, 100) { Value = pageCode },
                    new SqlParameter("@FieldName", SqlDbType.NVarChar, 200) { Value = item.FieldName.Trim() },
                    new SqlParameter("@LabelVi", SqlDbType.NVarChar, 255) { Value = (object?)item.LabelVi?.Trim() ?? DBNull.Value },
                    new SqlParameter("@LabelEn", SqlDbType.NVarChar, 255) { Value = (object?)item.LabelEn?.Trim() ?? DBNull.Value },
                    new SqlParameter("@TranslationKey", SqlDbType.NVarChar, 255) { Value = (object?)item.TranslationKey?.Trim() ?? DBNull.Value },
                }, cancellationToken);
            }

            return changed;
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

        private static bool IsDepartmentAuditLog(string entityKeys, Guid departmentId)
        {
            if (string.IsNullOrWhiteSpace(entityKeys))
            {
                return false;
            }

            try
            {
                using var document = JsonDocument.Parse(entityKeys);
                if (document.RootElement.ValueKind != JsonValueKind.Object)
                {
                    return false;
                }

                foreach (var property in document.RootElement.EnumerateObject())
                {
                    if (!string.Equals(property.Name, "DepartmentId", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var idText = property.Value.ValueKind == JsonValueKind.String
                        ? property.Value.GetString()
                        : property.Value.ToString();

                    return Guid.TryParse(idText, out var parsed) && parsed == departmentId;
                }

                return false;
            }
            catch (JsonException)
            {
                return entityKeys.Contains(departmentId.ToString(), StringComparison.OrdinalIgnoreCase);
            }
        }

        private static string TryExtractFieldName(string entityKeys)
        {
            if (string.IsNullOrWhiteSpace(entityKeys))
            {
                return null;
            }

            var keyMatch = Regex.Match(entityKeys, @"(?:^|;)\s*FieldName\s*=\s*([^;]+)", RegexOptions.IgnoreCase);
            if (keyMatch.Success)
            {
                return keyMatch.Groups[1].Value?.Trim();
            }

            return null;
        }

        private static (string fieldName, string oldValue, string newValue) TryExtractLegacyFieldValues(string oldValues, string newValues)
        {
            var oldValue = oldValues;
            var newValue = newValues;
            string fieldName = null;

            if (!string.IsNullOrWhiteSpace(oldValues))
            {
                var oldParts = oldValues.Split(new[] { ':' }, 2);
                if (oldParts.Length == 2)
                {
                    fieldName = oldParts[0].Trim();
                    oldValue = oldParts[1].Trim();
                }
            }

            if (!string.IsNullOrWhiteSpace(newValues))
            {
                var newParts = newValues.Split(new[] { ':' }, 2);
                if (newParts.Length == 2)
                {
                    fieldName ??= newParts[0].Trim();
                    newValue = newParts[1].Trim();
                }
            }

            return (fieldName, oldValue, newValue);
        }

        private static DepartmentAuditLogRow ToAuditRow(
            EntityAuditLogs log,
            Guid departmentId,
            Dictionary<string, string> metadataLookup)
        {
            if (!IsDepartmentAuditLog(log.EntityKeys, departmentId))
            {
                return null;
            }

            if (LooksLikeJson(log.OldValues) || LooksLikeJson(log.NewValues))
            {
                return null;
            }

            var fieldName = TryExtractFieldName(log.EntityKeys);
            var oldValue = log.OldValues;
            var newValue = log.NewValues;

            if (string.IsNullOrWhiteSpace(fieldName))
            {
                var legacy = TryExtractLegacyFieldValues(log.OldValues, log.NewValues);
                fieldName = legacy.fieldName;
                oldValue = legacy.oldValue;
                newValue = legacy.newValue;
            }

            if (string.IsNullOrWhiteSpace(fieldName))
            {
                return null;
            }

            var metadataKey = BuildMetadataKey(log.PageCode, fieldName);
            var fieldLabel = metadataLookup.TryGetValue(metadataKey, out var label)
                ? label
                : fieldName;

            return new DepartmentAuditLogRow
            {
                Id = log.Id,
                Action = log.Action,
                EntityKeys = log.EntityKeys,
                FieldName = fieldName,
                FieldLabel = fieldLabel,
                OldValue = oldValue,
                NewValue = newValue,
                PageCode = log.PageCode,
                RequestMethod = log.RequestMethod,
                RequestPath = log.RequestPath,
                UserName = log.UserName,
                TraceId = log.TraceId,
                StatusCode = log.StatusCode,
                ChangedAtUtc = log.ChangedAtUtc,
            };
        }

        private static string BuildMetadataKey(string pageCode, string fieldName)
        {
            return $"{pageCode ?? string.Empty}::{fieldName ?? string.Empty}";
        }

        private async Task EnsureAuditMetadataTableAsync(CancellationToken cancellationToken)
        {
            const string sql = @"
IF OBJECT_ID(N'dbo.AuditFieldMetadata', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AuditFieldMetadata]
    (
        [Id] UNIQUEIDENTIFIER NOT NULL CONSTRAINT [PK_AuditFieldMetadata] PRIMARY KEY DEFAULT NEWID(),
        [PageCode] NVARCHAR(100) NOT NULL,
        [FieldName] NVARCHAR(200) NOT NULL,
        [LabelVi] NVARCHAR(255) NULL,
        [LabelEn] NVARCHAR(255) NULL,
        [TranslationKey] NVARCHAR(255) NULL,
        [CreatedAtUtc] DATETIMEOFFSET NOT NULL CONSTRAINT [DF_AuditFieldMetadata_CreatedAtUtc] DEFAULT SYSDATETIMEOFFSET(),
        [UpdatedAtUtc] DATETIMEOFFSET NOT NULL CONSTRAINT [DF_AuditFieldMetadata_UpdatedAtUtc] DEFAULT SYSDATETIMEOFFSET()
    );

    CREATE UNIQUE INDEX [UX_AuditFieldMetadata_PageCode_FieldName]
    ON [dbo].[AuditFieldMetadata]([PageCode], [FieldName]);
END;";

            await _unitOfWork.Repository<Departments>().ExecuteSqlRawAsync(sql, cancellationToken: cancellationToken);
        }

        private static bool LooksLikeJson(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var trimmed = value.TrimStart();
            return trimmed.StartsWith("{", StringComparison.Ordinal) || trimmed.StartsWith("[", StringComparison.Ordinal);
        }
    }
}
