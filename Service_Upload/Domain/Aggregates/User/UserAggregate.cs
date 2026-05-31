using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using System;

namespace Domain.Aggregates.User
{
    /// <summary>
    /// Aggregate root: Người dùng.
    /// </summary>
    public sealed class UserAggregate
    {
        public Users Root { get; }

        private UserAggregate(Users root) =>
            Root = root ?? throw new DomainException("Người dùng không hợp lệ.");

        public static UserAggregate Create(EntityCode code, string name, string note)
        {
            ValidateName(name);

            var entity = new Users
            {
                UserId = Guid.NewGuid(),
                UserCode = code.Value,
                UserName = name.Trim(),
                Note = note,
                IsDelete = false,
                IsActive = true,
                CreateDate = DateTime.UtcNow,
            };

            return new UserAggregate(entity);
        }

        public static UserAggregate FromEntity(Users entity)
        {
            if (entity is null)
                throw new DomainException("Không tồn tại người dùng.");

            return new UserAggregate(entity);
        }

        public void Update(string name, string note, bool? isActive, bool? isDelete)
        {
            ValidateName(name);

            Root.UserName = name.Trim();
            Root.Note = note;
            Root.IsActive = isActive;
            Root.IsDelete = isDelete;
            Root.UpdateDate = DateTime.UtcNow;
        }

        public void UpdateCode(EntityCode code) => Root.UserCode = code.Value;

        public void SoftDelete()
        {
            Root.IsDelete = true;
            Root.UpdateDate = DateTime.UtcNow;
        }

        public static UserDepartments CreateDepartmentAssignment(Guid userId, Guid departmentId, Guid? positionId)
        {
            return new UserDepartments
            {
                UserId = userId,
                DepartmentId = departmentId,
                PositionId = positionId,
                IsActive = true,
                IsDelete = false,
                CreateDate = DateTime.UtcNow,
            };
        }

        public static void UpdateDepartmentAssignment(UserDepartments assignment, Guid? positionId)
        {
            if (assignment is null)
                throw new DomainException("Phân công phòng ban không hợp lệ.");

            assignment.PositionId = positionId;
            assignment.IsActive = true;
            assignment.IsDelete = false;
            assignment.UpdateDate = DateTime.UtcNow;
        }

        public static void SoftDeleteDepartmentAssignment(UserDepartments assignment)
        {
            if (assignment is null)
                throw new DomainException("Phân công phòng ban không hợp lệ.");

            assignment.IsDelete = true;
            assignment.UpdateDate = DateTime.UtcNow;
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Tên người dùng không được để trống.");
        }
    }
}
