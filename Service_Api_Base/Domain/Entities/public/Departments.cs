using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public partial class Departments
    {
        public Departments()
        {
            #region Generated Constructor
            UserDepartments = new HashSet<UserDepartments>();
            #endregion
        }

        #region Generated Properties
        public Guid DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public string DepartmentCode { get; set; }

        public string Note { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public Guid? CreateBy { get; set; }

        public Guid? UpdateBy { get; set; }

        public bool? IsDelete { get; set; }

        public Guid? ParentId { get; set; }

        #endregion

        #region Generated Relationships
        public virtual ICollection<UserDepartments> UserDepartments { get; set; }

        #endregion

    }
}
