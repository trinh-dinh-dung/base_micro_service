using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public partial class UserDepartments
    {
        public UserDepartments()
        {
            #region Generated Constructor
            #endregion
        }

        #region Generated Properties
        public Guid UserId { get; set; }

        public Guid DepartmentId { get; set; }

        public Guid? PositionId { get; set; }

        public string Note { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public Guid? CreateBy { get; set; }

        public Guid? UpdateBy { get; set; }

        public bool? IsDelete { get; set; }

        #endregion

        #region Generated Relationships
        public virtual Departments Departments { get; set; }

        public virtual Positions Positions { get; set; }

        public virtual Users Users { get; set; }

        #endregion

    }
}
