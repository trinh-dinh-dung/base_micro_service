using Application.GetMap;
using Application.Request;
using System;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IUserService
    {
        Task<bool> CreateUser(UserRequest request);
        Task<bool> UpdateUser(UserRequest request);
        Task<bool> DeleteUser(Guid userId);
        Task<UserInfoGetMap> GetUserById(Guid userId);
    }
}
