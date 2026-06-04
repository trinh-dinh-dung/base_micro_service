using Application.GetMap;
using Application.Request;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IDepartmentService
    {
        Task<bool> GetPaging(DepartmentRequest request, CancellationToken cancellationToken = default);
        Task<bool> CreateDepartment(DepartmentRequest request, CancellationToken cancellationToken = default);
        Task<bool> UpdateDepartment(DepartmentRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteDepartment(Guid departmentId, CancellationToken cancellationToken = default);
        Task<object> GetListDepartmentByParentId(Guid? parentId, CancellationToken cancellationToken = default);
        Task<List<DepartmentAuditLogRow>> GetDepartmentAuditLogs(Guid departmentId, int limit = 20, string language = "vi", CancellationToken cancellationToken = default);
        Task<int> SyncAuditMetadata(AuditMetadataSyncRequest request, CancellationToken cancellationToken = default);
        List<DepartmentTree> BuildDepartmentTree(List<DepartmentTree> departments, Guid? parentId);
    }
}
