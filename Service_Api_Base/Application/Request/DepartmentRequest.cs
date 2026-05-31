using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Request
{
    public class DepartmentRequest
    {
        public Guid DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public string DepartmentCode { get; set; }

        public Guid? ParentId { get; set; }

        public string Note { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public Guid? CreateBy { get; set; }

        public Guid? UpdateBy { get; set; }

        public bool? IsDelete { get; set; }
    }
}
