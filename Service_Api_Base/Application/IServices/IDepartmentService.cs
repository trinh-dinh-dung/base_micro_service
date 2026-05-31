using Application.GetMap;
using Application.Request;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IDepartmentService
    {
        Task<bool> GetPaging(DepartmentRequest request);
        Task<bool> CreateDepartment(DepartmentRequest request);
        Task<bool> UpdateDepartment(DepartmentRequest request);
        Task<bool> DeleteDepartment(Guid departmentId);
        Task<object> GetListDepartmentByParentId(Guid? parentId);
        List<DepartmentTree> BuildDepartmentTree(List<DepartmentTree> departments, Guid? parentId);
    }
}
