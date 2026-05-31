using Application.Common.Query;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.Request
{
    public class BomRequest : PagingQuery
    {
        /// <summary>
        /// ID phòng ban giao việc
        /// </summary>
        public string ApprovalRoomId { get; set; }
        /// <summary>
        /// Mã sản phẩm
        /// </summary>
        public string Productcode { get; set; }
        /// <summary>
        /// ID người phê duyệt
        /// </summary>
        public string ApproverId { get; set; }
        /// <summary>
        /// Sản phẩm sử dụng
        /// </summary>
        public string PeriodOfUse { get; set; }
        /// <summary>
        /// Kiểu thương mại
        /// </summary>
        public string ServiceProduct { get; set; }
        /// <summary>
        /// Phân loại chức năng
        /// </summary>
        public string FunctionalClassification { get; set; }
        /// <summary>
        /// Trạng thái phê duyệt
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// Trạng thái hiệu lực
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        /// Id phân loại sản phẩm
        /// </summary>
        public string TypeProductId { get; set; }
    }

    public class BomCreate
    {
        public string Bomcode { get; set; }
        public string Bomname { get; set; }
        public string Productcode { get; set; }
        public string Productname { get; set; }

        //public string Slotname { get; set; }
        //public string Slotdescription { get; set; }

        public string Codespnvl { get; set; }
        public string CodespnvlDes { get; set; }
        public double? Consumptionamount { get; set; }
        public string Unit { get; set; }
        public double? Qtyproductcreate { get; set; }
        public int? Cavity { get; set; }
        public int? CycleTime { get; set; }
        public float? MachineRunningTime { get; set; }
        public float? CapaDay { get; set; }
        public float? ProductWeight { get; set; }
        public float? StemWeight { get; set; }
        public float? TotalWeight { get; set; }
        public Guid Detailid { get; set; }
    }

    public class DataCreateVerBom
    {
        public List<VersionBomCreate> ListVerDetail { get; set; }
        public ConditionChangeVersion ChangeVerType { get; set; }
    }

    public class VersionBomCreate
    {
        public string Bomcode { get; set; }
        public string Bomname { get; set; }
        public string Productcode { get; set; }
        public string Productname { get; set; }

        public string Slotname { get; set; }
        public string Slotdescription { get; set; }

        public string Codespnvl { get; set; }
        public string CodespnvlDes { get; set; }
        public double? Consumptionamount { get; set; }
        public string Unit { get; set; }
        public double? Qtyproductcreate { get; set; }

    }

    public class ConditionChangeVersion
    {
        public ConditionChangeVersion()
        {
            ListTypeAccessories = new List<Typeaccessories>();
        }
        public int? Conditiontype { get; set; }
        public DateTime? Timechange { get; set; }
        public List<Typeaccessories> ListTypeAccessories { get; set; }

    }

    public class Typeaccessories
    {
        public string controlverid { get; set; }
        public string codespnvl { get; set; }
        public int qty { get; set; }
    }

    public class CreatBomRequest
    {
        /// <summary>
        /// ID định mức
        /// </summary>
        public string BomId { get; set; }
        /// <summary>
        /// Mã định mức
        /// </summary>
        public string Bomcode { get; set; }
        /// <summary>
        /// Tên định mức
        /// </summary>
        public string Bomname { get; set; }
        /// <summary>
        /// Mã sản phẩm
        /// </summary>
        public string Productcode { get; set; }
        /// <summary>
        /// Tên sản phẩm
        /// </summary>
        public string Productname { get; set; }
        /// <summary>
        /// Ngày hiệu lực
        /// </summary>
        public string EffectiveDate { get; set; }
        /// <summary>
        /// Hệ số công nghệ
        /// </summary>
        public decimal? TechnologyCoefficient { get; set; }
        /// <summary>
        /// Phiên bản mã định mức
        /// </summary>
        public int? BomVersion { get; set; }
        /// <summary>
        /// Loại tính toán đơn giá (1: Theo quy chế, 2: Theo sản phẩm)
        /// </summary>
        public int? TypePrice { get; set; }
        /// <summary>
        /// Có các bước chế tạo hay không
        /// </summary>
        public bool? HasProcess { get; set; }
        /// <summary>
        /// Đơn giá tháng
        /// </summary>
        public decimal? MonthlyUnitPrice { get; set; }
        /// <summary>
        /// ID quy trình công nghệ
        /// </summary>
        public string TechnologicalProcessId { get; set; }
        /// <summary>
        /// Danh sách list bộ phận cấu thành sản phẩm
        /// </summary>
        public List<ProductChildRequest> ListProductChild { get; set; }
        /// <summary>
        /// (1: định mức sản phẩm, 2: định mức theo bộ)
        /// </summary>
        public int? TypeBomProduct { get; set; }
        /// <summary>
        /// Tổng chi phí
        /// </summary>
        public decimal? TotalCost { get; set; }
        /// <summary>
        /// Tổng thời gian
        /// </summary>
        public decimal? TotalTime { get; set; }

        /// <summary>
        /// Danh sách bước chế tạo (Độ dài bằng 0 khi có trạng thái "HasProcess"=false)
        /// </summary>
        public List<BomLevelRequest> ListBomLevel { get; set; }
        /// <summary>
        /// Danh sách chi tiết định mức
        /// </summary>
        public List<BomversiondetailRequest> ListBomversiondetail { get; set; }

    }
    public class ProductChildRequest
    {
        /// <summary>
        /// ID bản ghi
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Mã sản phẩm cấu thành
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// ID định mức đang còn hiệu lực
        /// </summary>
        public string BomId { get; set; }
        /// <summary>
        /// Thứ tự sản phẩm
        /// </summary>
        public int? SortOrder { get; set; }
        /// <summary>
        /// Đơn vị sản xuất (xí ghiệp)
        /// </summary>
        public string DepartmentId { get; set; }
        /// <summary>                                 
        /// Số lượng             
        /// </summary>                                
        public int? Qty { get; set; }
    }
    public class BomLevelRequest
    {
        /// <summary>
        /// ID bản ghi
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// ID định mức
        /// </summary>
        public string BomId { get; set; }
        /// <summary>
        /// Số thứ tự
        /// </summary>
        public int? OrderLevel { get; set; }
        /// <summary>
        /// Tên bước
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Cấp độ thứ bao nhiêu
        /// </summary>
        public int? Level { get; set; }
        /// <summary>
        /// ID bước cha
        /// </summary>
        public string ParentId { get; set; }
    }
    public class BomversiondetailRequest
    {
        /// <summary>
        /// Dòng thứ bao nhiêu (thứ tự)
        /// </summary>
        public int? Line { get; set; }
        /// <summary>
        /// ID bản ghi chi tiết
        /// </summary>
        public string DetailId { get; set; }
        /// <summary>
        /// ID định mức
        /// </summary>
        public string BomId { get; set; }
        /// <summary>
        /// Mã định mức
        /// </summary>
        public string BomCode { get; set; }
        /// <summary>
        /// ID bước (Có thể bỏ trống nếu định mức có thuộc tính "HasProcess"=false)
        /// </summary>
        public string BomLevelId { get; set; }
        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// Số lượng
        /// </summary>
        public double? Qtyproductcreate { get; set; }
        /// <summary>
        /// Mô tả
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Chặng
        /// </summary>
        public bool? ControlStage { get; set; }
        /// <summary>
        /// ID môi trường
        /// </summary>
        public string EnviromentId { get; set; }
        /// <summary>
        /// ID cấp độ  công việc
        /// </summary>
        public string WorkLevelId { get; set; }
        /// <summary>
        /// Định mức giờ sx/ ĐVT
        /// </summary>
        public decimal? HourlyNorms { get; set; }
        /// <summary>
        /// Đơn giá giờ sx/ ĐVT
        /// </summary>
        public decimal? UnitPrice { get; set; }
        /// <summary>
        /// Định mức giờ KCS/ ĐVT
        /// </summary>
        public decimal? HourlyNormsKsc { get; set; }
        /// <summary>
        /// Đơn giá giờ KCS/ ĐVT
        /// </summary>
        public decimal? UnitPriceKcs { get; set; }
        /// <summary>
        /// Hệ số
        /// </summary>
        public decimal? Coefficient { get; set; }
        /// <summary>
        /// Trạng thái xóa của bản ghi
        /// </summary>
        public bool? IsDelete { get; set; }
        /// <summary>
        /// Đơn giá sản phẩm theo môi trường và cấp độ công việc nếu tính đon giá theo quy chế
        /// </summary>
        public decimal? UnitPriceFrame { get; set; }
        /// <summary>
        /// Phần trăm giới hạn
        /// </summary>
        public decimal? PercentLimit { get; set; }
        /// <summary>
        /// Đơn vị quy đổi
        /// </summary>
        public string UnitConvertTo { get; set; }
        /// <summary>
        /// Số lượng quy đổi
        /// </summary>
        public decimal? UnitQuantityTo { get; set; }

        /// <summary>
        /// có nhân với hệ số không
        /// </summary>
        public bool? IsMultiplyCoefficient { get; set; }
        /// <summary>
        /// Đơn giá sản xuất/đon vị tính GNSL
        /// </summary>
        public decimal? UnitPriceRecord { get; set; }
        /// <summary>
        /// Đơn vị tính KCS GNSL
        /// </summary>
        public string RecordUnit { get; set; }
        /// <summary>
        /// Đơn giá KCS/ đơn vị tính GNSL
        /// </summary>
        public decimal? UnitPriceKcsRecord { get; set; }

        /// <summary>
        /// cùng điều kiện thao tác
        /// </summary>
        public string LineGroup { get; set; }


        /// <summary>
        /// Thứ tự
        /// </summary>
        public int No { get; set; }

    }

    public class BomImportExcel
    {
        public IFormFile File { get; set; }
    }
    public class BomLevelTemp
    {
        public Guid Id { get; set; }

        public Guid? BomId { get; set; }

        public string Name { get; set; }

        public int? Level { get; set; }

        public Guid? ParentId { get; set; }

        public Guid? Createdby { get; set; }

        public DateTime? Createddate { get; set; }

        public Guid? Updatedby { get; set; }

        public DateTime? Updateddate { get; set; }

        public bool? Isdelete { get; set; }

        public int? OrderLevel { get; set; }

        public string Code { get; set; }

        public string NoShort { get; set; }
    }

    public class BomversiondetailTemp
    {
        public Guid DetailId { get; set; }

        public Guid? Slotid { get; set; }

        public Guid? Bomversionid { get; set; }

        public string Codespnvl { get; set; }

        public double? Consumptionamount { get; set; }

        public string Unit { get; set; }

        public double? Qtyproductcreate { get; set; }

        public int? Status { get; set; }

        public string Versionname { get; set; }

        public string Seasoncode { get; set; }

        public string Color { get; set; }

        public string Marketcode { get; set; }

        public string Sizecode { get; set; }

        public string Note { get; set; }

        public string BomCode { get; set; }

        public string Description { get; set; }

        public bool? ControlStage { get; set; }

        public string EnviromentId { get; set; }

        public string WorkLevelId { get; set; }

        public decimal? HourlyNorms { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? HourlyNormsKsc { get; set; }

        public decimal? UnitPriceKcs { get; set; }

        public decimal? Coefficient { get; set; }

        public Guid? BomLevelId { get; set; }

        public Guid? BomId { get; set; }

        public Guid? Createdby { get; set; }

        public DateTime? Createddate { get; set; }

        public Guid? Updatedby { get; set; }

        public DateTime? Updateddate { get; set; }

        public bool? Isdelete { get; set; }

        public int? Line { get; set; }

        public decimal? Percentlimit { get; set; }

        public string UnitConvertTo { get; set; }

        public decimal? UnitQuantityTo { get; set; }

        public bool? IsFromExcel { get; set; }

        public bool? IsMultiplyCoefficient { get; set; }

        public string LineGroup { get; set; }

        public decimal? UnitPriceRecord { get; set; }

        public string RecordUnit { get; set; }

        public decimal? UnitPriceKcsRecord { get; set; }

        public int? No { get; set; }

        public string StepCode { get; set; }

        public string ShortNo { get; set; }
    }
}
