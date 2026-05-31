using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Infrastructure.DataContext
{
    public partial class SopContext : DbContext
    {
        public SopContext(DbContextOptions<SopContext> options)
            : base(options)
        {
        }

        #region Generated Properties
        public virtual DbSet<Domain.Entities.Departments> Departments { get; set; }

        public virtual DbSet<Domain.Entities.Positions> Positions { get; set; }

        public virtual DbSet<Domain.Entities.UserDepartments> UserDepartments { get; set; }

        public virtual DbSet<Domain.Entities.Users> Users { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Generated Configuration
            modelBuilder.ApplyConfiguration(new Infrastructure.DataContext.Mapping.DepartmentsMap());
            modelBuilder.ApplyConfiguration(new Infrastructure.DataContext.Mapping.PositionsMap());
            modelBuilder.ApplyConfiguration(new Infrastructure.DataContext.Mapping.UserDepartmentsMap());
            modelBuilder.ApplyConfiguration(new Infrastructure.DataContext.Mapping.UsersMap());
            #endregion
        }
    }
}
