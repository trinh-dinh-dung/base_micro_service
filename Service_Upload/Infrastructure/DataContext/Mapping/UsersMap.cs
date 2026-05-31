using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataContext.Mapping
{
    public partial class UsersMap
        : IEntityTypeConfiguration<Domain.Entities.Users>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Domain.Entities.Users> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("Users", "public");

            // key
            builder.HasKey(t => t.UserId);

            // properties
            builder.Property(t => t.UserId)
                .IsRequired()
                .HasColumnName("UserId")
                .HasColumnType("uuid");

            builder.Property(t => t.UserName)
                .HasColumnName("UserName")
                .HasColumnType("character varying(255)")
                .HasMaxLength(255);

            builder.Property(t => t.UserCode)
                .HasColumnName("UserCode")
                .HasColumnType("character varying(50)")
                .HasMaxLength(50);

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
            #endregion
        }

        #region Generated Constants
        public struct Table
        {
            public const string Schema = "public";
            public const string Name = "Users";
        }

        public struct Columns
        {
            public const string UserId = "UserId";
            public const string UserName = "UserName";
            public const string UserCode = "UserCode";
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
