using Application.Abstractions.Persistence;
using Application.Exceptions;
using Application.IServices;
using Application.Request;
using Domain.Aggregates.Position;
using Domain.Entities;
using Domain.Exceptions;
using PositionEntity = Domain.Entities.Positions;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Application.Services
{
    public class PositionService : IPositionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PositionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreatePosition(PositionRequest request)
        {
            try
            {
                var exists = await _unitOfWork.Repository<PositionEntity>().FirstOrDefault(
                    filter: m => m.PositionCode == request.PositionCode && m.IsDelete != true);
                if (exists != null)
                    throw new DomainException("Mã chức vụ đã tồn tại");

                var code = EntityCode.Create(request.PositionCode, "Mã chức vụ");
                var aggregate = PositionAggregate.Create(code, request.PositionName, request.Note);
                await _unitOfWork.Repository<PositionEntity>().Insert(aggregate.Root);
                return await _unitOfWork.Commit();
            }
            catch (DomainException ex)
            {
                throw new AppException(ex.Message);
            }
        }

        public async Task<bool> UpdatePosition(PositionRequest request)
        {
            try
            {
                var entity = await _unitOfWork.Repository<PositionEntity>().FirstOrDefault(
                    filter: m => m.PositionId == request.PositionId);
                var aggregate = PositionAggregate.FromEntity(entity);
                aggregate.Update(request.PositionName, request.Note, request.IsActive, request.IsDelete);
                _unitOfWork.Repository<PositionEntity>().Update(aggregate.Root);
                return await _unitOfWork.Commit();
            }
            catch (DomainException ex)
            {
                throw new AppException(ex.Message);
            }
        }

        public async Task<bool> DeletePosition(Guid positionId)
        {
            try
            {
                var entity = await _unitOfWork.Repository<PositionEntity>().FirstOrDefault(
                    filter: m => m.PositionId == positionId);
                var aggregate = PositionAggregate.FromEntity(entity);
                aggregate.SoftDelete();
                _unitOfWork.Repository<PositionEntity>().Update(aggregate.Root);
                return await _unitOfWork.Commit();
            }
            catch (DomainException ex)
            {
                throw new AppException(ex.Message);
            }
        }

        public async Task<object> GetListAllPosition()
        {
            return await _unitOfWork.Repository<PositionEntity>().Get(filter: m => m.IsDelete != true).ToListAsync();
        }
    }
}
