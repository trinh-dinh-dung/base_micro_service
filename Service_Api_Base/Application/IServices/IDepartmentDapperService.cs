using Application.GetMap;
using Application.Request;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IDepartmentDapperService
    {
        Task<bool> CreateDepartmentAsync(DepartmentRequest request);
        Task<bool> UpdateDepartmentAsync(DepartmentRequest request);
        Task<bool> DeleteDepartmentAsync(Guid departmentId);
        Task<List<DepartmentTree>> GetListDepartmentByParentIdAsync(Guid? parentId);
    }
}
