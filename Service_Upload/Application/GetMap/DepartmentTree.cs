using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.GetMap
{
    public class DepartmentTree
    {
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentCode { get; set; }
        public Guid ParentId { get; set; }
        public string Note { get; set; }
        public int? Level { get; set; }
        public List<DepartmentTree> SubDepartments { get; set; } = new List<DepartmentTree>();
    }
}
