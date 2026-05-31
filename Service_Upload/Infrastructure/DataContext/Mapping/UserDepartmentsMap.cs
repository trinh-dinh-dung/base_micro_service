using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataContext.Mapping
{
    public partial class UserDepartmentsMap
        : IEntityTypeConfiguration<Domain.Entities.UserDepartments>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Domain.Entities.UserDepartments> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("UserDepartments", "public");

            // key
            builder.HasKey(t => new { t.UserId, t.DepartmentId });

            // properties
            builder.Property(t => t.UserId)
                .IsRequired()
                .HasColumnName("UserId")
                .HasColumnType("uuid");

            builder.Property(t => t.DepartmentId)
                .IsRequired()
                .HasColumnName("DepartmentId")
                .HasColumnType("uuid");

            builder.Property(t => t.PositionId)
                .HasColumnName("PositionId")
                .HasColumnType("uuid");

            builder.Property(t => t.Note)
                .HasColumnName("Note")
                .HasColumnType("character varying(1000)")
                .HasMaxLength(1000);

            builder.Property(t => t.IsActive)
                .HasColumnName("IsActive")
                .HasColumnType("boolean")
                .HasDefaultValueSql("true");

            builder.Property(t => t.CreateDate)
                .HasColumnName("CreateDate")
                .HasColumnType("date");

            builder.Property(t => t.UpdateDate)
                .HasColumnName("UpdateDate")
                .HasColumnType("date");

            builder.Property(t => t.CreateBy)
                .HasColumnName("CreateBy")
                .HasColumnType("uuid");

            builder.Property(t => t.UpdateBy)
                .HasColumnName("UpdateBy")
                .HasColumnType("uuid");

            builder.Property(t => t.IsDelete)
                .HasColumnName("IsDelete")
                .HasColumnType("boolean")
                .HasDefaultValueSql("false");

            // relationships
            builder.HasOne(t => t.Users)
                .WithMany(t => t.UserDepartments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("UserDepartments_UserId_fkey");

            builder.HasOne(t => t.Departments)
                .WithMany(t => t.UserDepartments)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("UserDepartments_DepartmentId_fkey");

            builder.HasOne(t => t.Positions)
                .WithMany(t => t.UserDepartments)
                .HasForeignKey(d => d.PositionId)
                .HasConstraintName("UserDepartments_PositionId_fkey");

            #endregion
        }

        #region Generated Constants
        public struct Table
        {
            public const string Schema = "public";
            public const string Name = "UserDepartments";
        }

        public struct Columns
        {
            public const string UserId = "UserId";
            public const string DepartmentId = "DepartmentId";
            public const string PositionId = "PositionId";
            public const string Note = "Note";
            public const string IsActive = "IsActive";
            public const string CreateDate = "CreateDate";
            public const string UpdateDate = "UpdateDate";
            public const string CreateBy = "CreateBy";
            public const string UpdateBy = "UpdateBy";
            public const string IsDelete = "IsDelete";
        }
        #endregion
    }
}
