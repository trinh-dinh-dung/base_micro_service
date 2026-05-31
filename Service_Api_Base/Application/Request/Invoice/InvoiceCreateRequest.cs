using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Request.Invoice
{
    public class ParkingTicketRequest
    {
        /// <summary>
        /// KeyParking
        /// </summary>
        public string KeyParking { get; set; }
        /// <summary>
        /// Biển số xe
        /// </summary>
        public string LicensePlate { get; set; }
        /// <summary>
        /// Loại xe (xe máy, ô tô...)
        /// </summary>
        public int VehicleType { get; set; }
        /// <summary>
        /// Thời gian vào bãi xe
        /// </summary>
        public DateTime TimeIn { get; set; }
        /// <summary>
        /// Thời gian ra
        /// </summary>
        public DateTime TimeOut { get; set; }
        /// <summary>
        /// Phí gửi xe
        /// </summary>
        public decimal Fee { get; set; }
        /// <summary>
        /// Mô tả vé gửi xe
        /// </summary>
        public string Description { get; set; }
    }
}
