using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public partial class Departments
    {
        public Departments()
        {
            #region Generated Constructor
            #endregion
        }

        #region Generated Properties
        public Guid DepartmentId { get; set; }

        public string DepartmentName { get; set; } = null!;

        public string DepartmentCode { get; set; } = null!;

        public string? Note { get; set; }

        public bool IsActive { get; set; }

        public DateOnly? CreateDate { get; set; }

        public DateOnly? UpdateDate { get; set; }

        public Guid? CreateBy { get; set; }

        public Guid? UpdateBy { get; set; }

        public bool IsDelete { get; set; }

        public Guid? ParentId { get; set; }

        #endregion

        #region Generated Relationships
        #endregion

    }
}
