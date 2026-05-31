using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Request
{
    public class PositionRequest
    {
        public Guid PositionId { get; set; }

        public string PositionName { get; set; }

        public string PositionCode { get; set; }

        public string Note { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public Guid? CreateBy { get; set; }

        public Guid? UpdateBy { get; set; }

        public bool? IsDelete { get; set; }
    }
}
