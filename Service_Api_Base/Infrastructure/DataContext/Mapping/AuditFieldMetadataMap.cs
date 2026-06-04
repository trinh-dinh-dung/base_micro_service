using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataContext.Mapping
{
    public class AuditFieldMetadataMap : IEntityTypeConfiguration<Domain.Entities.AuditFieldMetadata>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Domain.Entities.AuditFieldMetadata> builder)
        {
            builder.ToTable("AuditFieldMetadata", "dbo");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .IsRequired()
                .HasColumnName("Id")
                .HasColumnType("uniqueidentifier")
                .HasDefaultValueSql("(newid())");

            builder.Property(t => t.PageCode)
                .IsRequired()
                .HasColumnName("PageCode")
                .HasColumnType("nvarchar(100)")
                .HasMaxLength(100);

            builder.Property(t => t.FieldName)
                .IsRequired()
                .HasColumnName("FieldName")
                .HasColumnType("nvarchar(200)")
                .HasMaxLength(200);

            builder.Property(t => t.LabelVi)
                .HasColumnName("LabelVi")
                .HasColumnType("nvarchar(255)")
                .HasMaxLength(255);

            builder.Property(t => t.LabelEn)
                .HasColumnName("LabelEn")
                .HasColumnType("nvarchar(255)")
                .HasMaxLength(255);

            builder.Property(t => t.TranslationKey)
                .HasColumnName("TranslationKey")
                .HasColumnType("nvarchar(255)")
                .HasMaxLength(255);

            builder.Property(t => t.CreatedAtUtc)
                .IsRequired()
                .HasColumnName("CreatedAtUtc")
                .HasColumnType("datetimeoffset")
                .HasDefaultValueSql("(sysdatetimeoffset())");

            builder.Property(t => t.UpdatedAtUtc)
                .IsRequired()
                .HasColumnName("UpdatedAtUtc")
                .HasColumnType("datetimeoffset")
                .HasDefaultValueSql("(sysdatetimeoffset())");

            builder.HasIndex(t => new { t.PageCode, t.FieldName })
                .IsUnique()
                .HasDatabaseName("UX_AuditFieldMetadata_PageCode_FieldName");
        }
    }
}