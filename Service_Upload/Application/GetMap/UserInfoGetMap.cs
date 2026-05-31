using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.GetMap
{
    public class UserInfoGetMap
    {
        public UserInfoGetMap()
        {
            ListDepartmentOfUser = new List<DepartmentOfUserGetMap>();
        }
        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public string UserCode { get; set; }

        public string Note { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public Guid? CreateBy { get; set; }

        public Guid? UpdateBy { get; set; }

        public bool? IsDelete { get; set; }

        public List<DepartmentOfUserGetMap> ListDepartmentOfUser { get; set; }
    }

    public class DepartmentOfUserGetMap
    {
        public Guid DepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public Guid? PosititonId { get; set; }
        public string PosititonCode { get; set; }
        public string PositionName { get; set; }
    }
}
