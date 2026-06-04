using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataContext.Mapping
{
    public partial class DepartmentsMap
        : IEntityTypeConfiguration<Domain.Entities.Departments>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Domain.Entities.Departments> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("Departments", "dbo");

            // key
            builder.HasKey(t => t.DepartmentId);

            // properties
            builder.Property(t => t.DepartmentId)
                .IsRequired()
                .HasColumnName("DepartmentId")
                .HasColumnType("uniqueidentifier")
                .HasDefaultValueSql("(newid())");

            builder.Property(t => t.DepartmentName)
                .IsRequired()
                .HasColumnName("DepartmentName")
                .HasColumnType("nvarchar(255)")
                .HasMaxLength(255);

            builder.Property(t => t.DepartmentCode)
                .IsRequired()
                .HasColumnName("DepartmentCode")
                .HasColumnType("nvarchar(50)")
                .HasMaxLength(50);

            builder.Property(t => t.Note)
                .HasColumnName("Note")
                .HasColumnType("nvarchar(1000)")
                .HasMaxLength(1000);

            builder.Property(t => t.IsActive)
                .IsRequired()
                .HasColumnName("IsActive")
                .HasColumnType("bit")
                .HasDefaultValue(true);

            builder.Property(t => t.CreateDate)
                .HasColumnName("CreateDate")
                .HasColumnType("date");

            builder.Property(t => t.UpdateDate)
                .HasColumnName("UpdateDate")
                .HasColumnType("date");

            builder.Property(t => t.CreateBy)
                .HasColumnName("CreateBy")
                .HasColumnType("uniqueidentifier");

            builder.Property(t => t.UpdateBy)
                .HasColumnName("UpdateBy")
                .HasColumnType("uniqueidentifier");

            builder.Property(t => t.IsDelete)
                .IsRequired()
                .HasColumnName("IsDelete")
                .HasColumnType("bit")
                .HasDefaultValue(false);

            builder.Property(t => t.ParentId)
                .HasColumnName("ParentId")
                .HasColumnType("uniqueidentifier");

            // relationships
            #endregion
        }

        #region Generated Constants
        public readonly struct Table
        {
            public const string Schema = "dbo";
            public const string Name = "Departments";
        }

        public readonly struct Columns
        {
            public const string DepartmentId = "DepartmentId";
            public const string DepartmentName = "DepartmentName";
            public const string DepartmentCode = "DepartmentCode";
            public const string Note = "Note";
            public const string IsActive = "IsActive";
            public const string CreateDate = "CreateDate";
            public const string UpdateDate = "UpdateDate";
            public const string CreateBy = "CreateBy";
            public const string UpdateBy = "UpdateBy";
            public const string IsDelete = "IsDelete";
            public const string ParentId = "ParentId";
        }
        #endregion
    }
}
