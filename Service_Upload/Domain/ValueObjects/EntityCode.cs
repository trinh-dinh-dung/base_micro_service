using Domain.Exceptions;
using System;

namespace Domain.ValueObjects
{
    /// <summary>
    /// Value object cho mã danh mục (phòng ban, user, chức vụ).
    /// </summary>
    public sealed class EntityCode : IEquatable<EntityCode>
    {
        public string Value { get; }

        private EntityCode(string value) => Value = value;

        public static EntityCode Create(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException($"{fieldName} không được để trống.");

            return new EntityCode(value.Trim());
        }

        public bool Equals(EntityCode other) =>
            other is not null && string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

        public override bool Equals(object obj) => obj is EntityCode other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);

        public override string ToString() => Value;

        public static implicit operator string(EntityCode code) => code.Value;
    }
}
