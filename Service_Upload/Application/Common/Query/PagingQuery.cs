using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Common.Query
{
    public class PagingQuery
    {
        public PagingQuery()
        {
            PageIndex = 1;
            PageSize = 20;
        }
        /// <summary>
        /// Từ khóa tìm kiếm
        /// </summary>
        public string SearchTerm { get; set; }
        /// <summary>
        /// Số bản ghi
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Page size is positive number only")]
        public int PageSize { get; set; }
        /// <summary>
        /// Số trang
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Page Index is positive number only")]
        public int PageIndex { get; set; }

        [JsonIgnore]
        public Guid? UserId { get; set; }

        [JsonIgnore]
        public bool IsAdmin { get; set; } = false;

        /// <summary>
        /// ID user gán với nhóm quyền
        /// </summary>
        public string UserPermissionId { get; set; }
    }
}
