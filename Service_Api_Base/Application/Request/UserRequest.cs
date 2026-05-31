using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Request
{
    public class UserRequest
    {

        public UserRequest()
        {
            ListDepartmentOfUser = new List<DepartmentOfUser>();
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

        public List<DepartmentOfUser> ListDepartmentOfUser { get; set; }

    }

    public class DepartmentOfUser
    {
        /// <summary>
        /// phong ban của user
        /// </summary>
        public Guid DepartmentId { get; set; }

        /// <summary>
        /// chuc vu cua user
        /// </summary>
        public Guid PositionId { get; set; }
    }

}
