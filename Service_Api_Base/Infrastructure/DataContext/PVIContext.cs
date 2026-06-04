using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Infrastructure.DataContext
{
    public partial class PVIContext : DbContext
    {
        public PVIContext(DbContextOptions<PVIContext> options)
            : base(options)
        {
        }

        #region Generated Properties
        public virtual DbSet<Domain.Entities.Departments> Departments { get; set; } = null!;

        public virtual DbSet<Domain.Entities.EntityAuditLogs> EntityAuditLogs { get; set; } = null!;

        public virtual DbSet<Domain.Entities.AuditFieldMetadata> AuditFieldMetadata { get; set; } = null!;

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Generated Configuration
            modelBuilder.ApplyConfiguration(new Infrastructure.DataContext.Mapping.DepartmentsMap());
            modelBuilder.ApplyConfiguration(new Infrastructure.DataContext.Mapping.EntityAuditLogsMap());
            modelBuilder.ApplyConfiguration(new Infrastructure.DataContext.Mapping.AuditFieldMetadataMap());
            #endregion
        }
    }
}
