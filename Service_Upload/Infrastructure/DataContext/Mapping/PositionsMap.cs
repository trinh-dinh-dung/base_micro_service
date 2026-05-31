using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataContext.Mapping
{
    public partial class PositionsMap
        : IEntityTypeConfiguration<Domain.Entities.Positions>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Domain.Entities.Positions> builder)
        {
            #region Generated Configure
            // table
            builder.ToTable("Positions", "public");

            // key
            builder.HasKey(t => t.PositionId);

            // properties
            builder.Property(t => t.PositionId)
                .IsRequired()
                .HasColumnName("PositionId")
                .HasColumnType("uuid");

            builder.Property(t => t.PositionName)
                .IsRequired()
                .HasColumnName("PositionName")
                .HasColumnType("character varying(255)")
                .HasMaxLength(255);

            builder.Property(t => t.PositionCode)
                .IsRequired()
                .HasColumnName("PositionCode")
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
            public const string Name = "Positions";
        }

        public struct Columns
        {
            public const string PositionId = "PositionId";
            public const string PositionName = "PositionName";
            public const string PositionCode = "PositionCode";
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
