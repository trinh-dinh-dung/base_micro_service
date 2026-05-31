using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using System;

namespace Domain.Aggregates.Position
{
    /// <summary>
    /// Aggregate root: Chức vụ.
    /// </summary>
    public sealed class PositionAggregate
    {
        public Positions Root { get; }

        private PositionAggregate(Positions root) =>
            Root = root ?? throw new DomainException("Chức vụ không hợp lệ.");

        public static PositionAggregate Create(EntityCode code, string name, string note)
        {
            ValidateName(name);

            var entity = new Positions
            {
                PositionId = Guid.NewGuid(),
                PositionCode = code.Value,
                PositionName = name.Trim(),
                Note = note,
                IsDelete = false,
                IsActive = true,
                CreateDate = DateTime.UtcNow,
            };

            return new PositionAggregate(entity);
        }

        public static PositionAggregate FromEntity(Positions entity)
        {
            if (entity is null)
                throw new DomainException("Không tồn tại chức vụ.");

            return new PositionAggregate(entity);
        }

        public void Update(string name, string note, bool? isActive, bool? isDelete)
        {
            ValidateName(name);

            Root.PositionName = name.Trim();
            Root.Note = note;
            Root.IsActive = isActive;
            Root.IsDelete = isDelete;
            Root.UpdateDate = DateTime.UtcNow;
        }

        public void SoftDelete()
        {
            Root.IsDelete = true;
            Root.UpdateDate = DateTime.UtcNow;
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Tên chức vụ không được để trống.");
        }
    }
}
