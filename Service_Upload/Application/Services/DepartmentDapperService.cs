using Application.Abstractions.Persistence;
using Application.Exceptions;
using Application.GetMap;
using Application.IServices;
using Application.Request;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DepartmentDapperService : IDepartmentDapperService
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DepartmentDapperService(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> CreateDepartmentAsync(DepartmentRequest request)
        {
            var connection = _connectionFactory.CreateConnection();
            try
            {
                await DbConnectionHelper.OpenConnectionAsync(connection);

                const string existsSql = @"SELECT 1 FROM ""Departments"" WHERE ""DepartmentCode"" = @DepartmentCode AND ""IsDelete"" != true LIMIT 1";
                var exists = await connection.QueryFirstOrDefaultAsync<int?>(existsSql, new { request.DepartmentCode });
                if (exists.HasValue)
                    throw new AppException("Mã phòng ban đã tồn tại");

                const string insertSql = @"
INSERT INTO ""Departments""
(""DepartmentId"", ""DepartmentName"", ""DepartmentCode"", ""ParentId"", ""Note"", ""IsActive"", ""CreateDate"", ""IsDelete"")
VALUES
(@DepartmentId, @DepartmentName, @DepartmentCode, @ParentId, @Note, @IsActive, @CreateDate, @IsDelete);";

                var row = await connection.ExecuteAsync(insertSql, new
                {
                    DepartmentId = Guid.NewGuid(),
                    request.DepartmentName,
                    request.DepartmentCode,
                    request.ParentId,
                    request.Note,
                    IsActive = request.IsActive ?? true,
                    CreateDate = DateTime.UtcNow,
                    IsDelete = false
                });

                return row > 0;
            }
            finally
            {
                DbConnectionHelper.CloseConnection(connection);
            }
        }

        public async Task<bool> UpdateDepartmentAsync(DepartmentRequest request)
        {
            var connection = _connectionFactory.CreateConnection();
            try
            {
                await DbConnectionHelper.OpenConnectionAsync(connection);

                const string existsSql = @"SELECT 1 FROM ""Departments"" WHERE ""DepartmentId"" = @DepartmentId LIMIT 1";
                var exists = await connection.QueryFirstOrDefaultAsync<int?>(existsSql, new { request.DepartmentId });
                if (!exists.HasValue)
                    throw new AppException("Không tồn tại phòng ban cần cập nhật!");

                const string updateSql = @"
UPDATE ""Departments""
SET ""DepartmentName"" = @DepartmentName,
    ""ParentId"" = @ParentId,
    ""Note"" = @Note,
    ""IsActive"" = @IsActive,
    ""IsDelete"" = @IsDelete,
    ""UpdateDate"" = @UpdateDate
WHERE ""DepartmentId"" = @DepartmentId;";

                var row = await connection.ExecuteAsync(updateSql, new
                {
                    request.DepartmentId,
                    request.DepartmentName,
                    request.ParentId,
                    request.Note,
                    request.IsActive,
                    request.IsDelete,
                    UpdateDate = DateTime.UtcNow
                });

                return row > 0;
            }
            finally
            {
                DbConnectionHelper.CloseConnection(connection);
            }
        }

        public async Task<bool> DeleteDepartmentAsync(Guid departmentId)
        {
            var connection = _connectionFactory.CreateConnection();
            try
            {
                await DbConnectionHelper.OpenConnectionAsync(connection);

                const string deleteSql = @"
UPDATE ""Departments""
SET ""IsDelete"" = true,
    ""UpdateDate"" = @UpdateDate
WHERE ""DepartmentId"" = @DepartmentId;";

                var row = await connection.ExecuteAsync(deleteSql, new
                {
                    DepartmentId = departmentId,
                    UpdateDate = DateTime.UtcNow
                });

                if (row == 0)
                    throw new AppException("Không tồn tại phòng ban cần xóa!");

                return true;
            }
            finally
            {
                DbConnectionHelper.CloseConnection(connection);
            }
        }

        public async Task<List<DepartmentTree>> GetListDepartmentByParentIdAsync(Guid? parentId)
        {
            var connection = _connectionFactory.CreateConnection();
            try
            {
                await DbConnectionHelper.OpenConnectionAsync(connection);

                var whereClause = parentId.HasValue ? @"""ParentId"" = @ParentId" : @"""ParentId"" IS NULL";
                var query = $@"
WITH RECURSIVE DepartmentHierarchy AS (
    SELECT
        ""DepartmentId"",
        ""DepartmentName"",
        ""DepartmentCode"",
        ""ParentId"",
        ""Note"",
        1 AS ""Level""
    FROM ""Departments""
    WHERE {whereClause}
    UNION ALL
    SELECT
        d.""DepartmentId"",
        d.""DepartmentName"",
        d.""DepartmentCode"",
        d.""ParentId"",
        d.""Note"",
        dh.""Level"" + 1
    FROM ""Departments"" d
    INNER JOIN DepartmentHierarchy dh ON dh.""DepartmentId"" = d.""ParentId""
)
SELECT * FROM DepartmentHierarchy;";

                var rows = await connection.QueryAsync<DepartmentRow>(query, new { ParentId = parentId });
                return rows.Select(x => x.ToDepartmentTree()).ToList();
            }
            finally
            {
                DbConnectionHelper.CloseConnection(connection);
            }
        }
    }
}
