using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using System;

namespace Domain.Aggregates.Department
{
    /// <summary>
    /// Aggregate root: Phòng ban.
    /// </summary>
    public sealed class DepartmentAggregate
    {
        public Departments Root { get; }

        private DepartmentAggregate(Departments root) =>
            Root = root ?? throw new DomainException("Phòng ban không hợp lệ.");

        public static DepartmentAggregate Create(EntityCode code, string name, Guid? parentId, string note)
        {
            ValidateName(name);

            var entity = new Departments
            {
                DepartmentId = Guid.NewGuid(),
                DepartmentCode = code.Value,
                DepartmentName = name.Trim(),
                ParentId = parentId,
                Note = note,
                IsDelete = false,
                IsActive = true,
                CreateDate = DateOnly.FromDateTime(DateTime.UtcNow),
            };

            return new DepartmentAggregate(entity);
        }

        public static DepartmentAggregate FromEntity(Departments entity)
        {
            if (entity is null)
                throw new DomainException("Không tồn tại phòng ban.");

            return new DepartmentAggregate(entity);
        }

        public void Update(string name, Guid? parentId, string note, bool? isActive, bool? isDelete)
        {
            ValidateName(name);

            Root.DepartmentName = name.Trim();
            Root.ParentId = parentId;
            Root.Note = note;
            Root.IsActive = isActive ?? Root.IsActive;
            Root.IsDelete = isDelete ?? Root.IsDelete;
            Root.UpdateDate = DateOnly.FromDateTime(DateTime.UtcNow);
        }

        public void SoftDelete()
        {
            Root.IsDelete = true;
            Root.UpdateDate = DateOnly.FromDateTime(DateTime.UtcNow);
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Tên phòng ban không được để trống.");
        }
    }
}
