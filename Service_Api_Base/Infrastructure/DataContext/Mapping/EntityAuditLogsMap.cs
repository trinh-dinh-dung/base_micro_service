using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataContext.Mapping
{
    public partial class EntityAuditLogsMap
        : IEntityTypeConfiguration<Domain.Entities.EntityAuditLogs>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Domain.Entities.EntityAuditLogs> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("EntityAuditLogs", "dbo");

            // key
            builder.HasKey(t => t.Id);

            // properties
            builder.Property(t => t.Id)
                .IsRequired()
                .HasColumnName("Id")
                .HasColumnType("uniqueidentifier")
                .HasDefaultValueSql("(newid())");

            builder.Property(t => t.EntityName)
                .IsRequired()
                .HasColumnName("EntityName")
                .HasColumnType("nvarchar(200)")
                .HasMaxLength(200);

            builder.Property(t => t.Action)
                .IsRequired()
                .HasColumnName("Action")
                .HasColumnType("nvarchar(20)")
                .HasMaxLength(20);

            builder.Property(t => t.EntityKeys)
                .HasColumnName("EntityKeys")
                .HasColumnType("nvarchar(max)");

            builder.Property(t => t.OldValues)
                .HasColumnName("OldValues")
                .HasColumnType("nvarchar(max)");

            builder.Property(t => t.NewValues)
                .HasColumnName("NewValues")
                .HasColumnType("nvarchar(max)");

            builder.Property(t => t.PageCode)
                .HasColumnName("PageCode")
                .HasColumnType("nvarchar(100)")
                .HasMaxLength(100);

            builder.Property(t => t.TraceId)
                .HasColumnName("TraceId")
                .HasColumnType("nvarchar(100)")
                .HasMaxLength(100);

            builder.Property(t => t.RequestPath)
                .HasColumnName("RequestPath")
                .HasColumnType("nvarchar(500)")
                .HasMaxLength(500);

            builder.Property(t => t.RequestMethod)
                .HasColumnName("RequestMethod")
                .HasColumnType("nvarchar(10)")
                .HasMaxLength(10);

            builder.Property(t => t.UserName)
                .HasColumnName("UserName")
                .HasColumnType("nvarchar(200)")
                .HasMaxLength(200);

            builder.Property(t => t.StatusCode)
                .HasColumnName("StatusCode")
                .HasColumnType("int");

            builder.Property(t => t.ChangedAtUtc)
                .IsRequired()
                .HasColumnName("ChangedAtUtc")
                .HasColumnType("datetimeoffset")
                .HasDefaultValueSql("(sysdatetimeoffset())");

            // relationships
            #endregion
        }

        #region Generated Constants
        public readonly struct Table
        {
            public const string Schema = "dbo";
            public const string Name = "EntityAuditLogs";
        }

        public readonly struct Columns
        {
            public const string Id = "Id";
            public const string EntityName = "EntityName";
            public const string Action = "Action";
            public const string EntityKeys = "EntityKeys";
            public const string OldValues = "OldValues";
            public const string NewValues = "NewValues";
            public const string PageCode = "PageCode";
            public const string TraceId = "TraceId";
            public const string RequestPath = "RequestPath";
            public const string RequestMethod = "RequestMethod";
            public const string UserName = "UserName";
            public const string StatusCode = "StatusCode";
            public const string ChangedAtUtc = "ChangedAtUtc";
        }
        #endregion
    }
}
