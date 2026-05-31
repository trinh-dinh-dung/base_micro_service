using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Models
{
    public class PagingModel<T>
    {
        /// <summary>
        /// Danh sách item
        /// </summary>
        public IEnumerable<T> Items { get; set; }
        /// <summary>
        /// Tổng item trong db
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// Số trang
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// Số bản ghi trên trang
        /// </summary>
        public int PageSize { get; set; }
        public PagingModel(IEnumerable<T> items, int totalCount, int pageIndex, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
    }
}
