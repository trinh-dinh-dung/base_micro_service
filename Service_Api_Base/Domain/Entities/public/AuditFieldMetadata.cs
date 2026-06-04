using System;

namespace Domain.Entities
{
    public partial class AuditFieldMetadata
    {
        public Guid Id { get; set; }

        public string PageCode { get; set; } = null!;

        public string FieldName { get; set; } = null!;

        public string? LabelVi { get; set; }

        public string? LabelEn { get; set; }

        public string? TranslationKey { get; set; }

        public DateTimeOffset CreatedAtUtc { get; set; }

        public DateTimeOffset UpdatedAtUtc { get; set; }
    }
}