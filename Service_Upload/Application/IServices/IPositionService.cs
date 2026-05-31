using Application.Request;
using System;
using System.Threading.Tasks;

namespace Application.IServices
{
    public interface IPositionService
    {
        Task<bool> CreatePosition(PositionRequest request);
        Task<bool> UpdatePosition(PositionRequest request);
        Task<bool> DeletePosition(Guid positionId);
        Task<object> GetListAllPosition();
    }
}
