using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Status
{
    public enum WorkByTypeEnumMobile
    {
        /// <summary>
        /// Khu vực sản xuất
        /// </summary>
        WorkArea = 1,
        /// <summary>
        /// Trung tẩm sản xuất
        /// </summary>
        WorkCenter = 2,
        /// <summary>
        /// Đơn vị sản xuất
        /// </summary>
        WorkUnit = 3,
        /// <summary>
        /// Người lao động
        /// </summary>
        Worker = 4,
        Type = 5,
        Function = 6,
        GroupWorkDay = 7,
        WorkAreaOnlyChild = 8,
        /// <summary>
        /// Kĩ năng
        /// </summary>
        Skill = 9,
        /// <summary>
        /// Tỉnh, TP
        /// </summary>
        Provinces = 10,
        /// <summary>
        /// Quận, huyện
        /// </summary>
        District = 11,
        Position = 12,
        Department = 13,
        /// <summary>
        /// Sản phẩm
        /// </summary>
        Product = 14,
        Calendar = 15,
        BomSlot = 16,
        Sop = 17,
        ProductStatusBomCreate = 18,
        Model = 19,
        ApprovalFlowDefault = 20,
        Process = 21,
        Productionrequirement = 22,
        StepProcess = 23,
        /// <summary>
        /// Điểm kiểm tra
        /// </summary>
        Checkpoint = 24,
        /// <summary>
        /// Bài kiểm tra
        /// </summary>
        Checklist = 25,
        /// <summary>
        /// Lệnh sản xuất
        /// </summary>
        Workorder = 26,
        /// <summary>
        /// Lỗi
        /// </summary>
        Error = 27,
        /// <summary>
        /// Lệnh sản xuất
        /// </summary>
        WorkOrderCode = 28,
        StepByWU = 29,
        /// <summary>
        /// Màu
        /// </summary>
        Color = 30,
        /// <summary>
        /// Thị trường
        /// </summary>
        MarketProduct = 31,
        /// <summary>
        /// Mùa
        /// </summary>
        Season = 32,
        /// <summary>
        /// Kích cỡ
        /// </summary>
        Size = 33,
        /// <summary>
        /// Mã lớn
        /// </summary>
        Maincodeproduct = 34,
        /// <summary>
        /// Lệnh sản xuất by mã sản phẩm
        /// </summary>
        WorkorderV2 = 35,
        /// <summary>
        /// Ca làm việc
        /// </summary>
        Timeslot = 36,
        /// <summary>
        /// Mã nhỏ
        /// </summary>
        Subcodeproduct = 37,
        /// <summary>
        /// Khách hàng
        /// </summary>
        Customer = 38,
        /// <summary>
        /// Người lao động trên workcenter
        /// </summary>
        WorkerInWorkcenter = 39,
        /// <summary>
        /// Lý do nghỉ việc
        /// </summary>
        ReasonAbsence = 40,
        /// <summary>
        /// Danh sách công nhân support workcenter khac
        /// </summary>
        WorkerSupportWorkcenter = 41,
        /// <summary>
        /// Lấy tất cả workcenter
        /// </summary>
        WorkcenterAll = 42,
        WorkerWithWorkcenter = 43,//Lay tat ca cong nhan support cho chuyen may khac
        /// <summary>
        /// Bộ phận sản phẩm
        /// </summary>
        PartProduct = 44,
        /// <summary>
        /// Loại kiểm tra
        /// </summary>
        TestType = 45,
        /// <summary>
        /// Mã đơn hàng
        /// </summary>
        Ordercode = 46,
        /// <summary>
        /// Danh sách lý do thời gian chết
        /// </summary>
        LostNote = 47,
    }
    public enum TypeWorkcenterMobile
    {
        May = 1,//May
        Cat = 2,//Cat
        HoanThien = 3,//Hoan Thien
        Kho = 4,//Kho
        VanPhong = 5,//Van Phong
        KhachHang = 6,//Khach Hang
        Support = 7,//Support
    }
    public enum WorkOrderTypeMobile
    {
        released = 1,
        running = 2,
        hold = 3,
        finish = 4,
        close = 5
    }
    public enum ApproveStatusMobile
    {
        CreateNew = 1, // Tao moi
        Waitting = 2,// Cho phe duyet
        Approved = 3,// Da phe duyet
        Reject = 4,// Tu choi
        Approving = 5,// Dang phe duyet
        Using = 6, // Dang su dung 
        Expired = 7, // Het han
    }
    public enum StatusTicketWorkOrderMobile
    {
        /// <summary>
        /// Lệnh sản xuất
        /// </summary>
        ProductionOrder = 1,
        /// <summary>
        /// Phiếu đặt làm
        /// </summary>
        BallotOrder = 2,
        /// <summary>
        /// phiếu giao việc
        /// </summary>
        AssignWorkOrder = 3,
        /// <summary>
        /// lệnh giao việc
        /// </summary>
        JobAssignmentOrder = 4,
    }
    public enum ListFilterDeliveryBTP
    {
        /// <summary>
        /// ID trung tam san xuat
        /// </summary>
        Workcenterid = 1,
        /// <summary>
        /// Mã lệnh sản xuất
        /// </summary>
        Workordercode = 2,
        /// <summary>
        /// Mã to
        /// </summary>
        Maincodeproduct = 3,
        /// <summary>
        /// Mã màu
        /// </summary>
        Colorcode = 4,
        /// <summary>
        /// Mã size
        /// </summary>
        Sizecode = 5,
    }
    public enum FilterGoodsLaundryType
    {
        /// <summary>
        /// ID trung tam san xuat
        /// </summary>
        Workcenterid = 1,
        /// <summary>
        /// Mã lệnh sản xuất
        /// </summary>
        Workordercode = 2,
        /// <summary>
        /// Mã to
        /// </summary>
        Maincodeproduct = 3,
        /// <summary>
        /// Mã nhỏ
        /// </summary>
        Subcodeproduct = 4,
        /// <summary>
        /// Mã màu
        /// </summary>
        Colorcode = 5,
        /// <summary>
        /// Mã mùa
        /// </summary>
        Seasoncode = 6,
        /// <summary>
        /// Mã thị trường
        /// </summary>
        Marketcode = 7,
        /// <summary>
        /// Mã size
        /// </summary>
        Sizecode = 8,
    }
    public enum FilterRecordManufacturingType
    {
        /// <summary>
        /// ID trung tam san xuat
        /// </summary>
        Workcenterid = 1,
        /// <summary>
        /// Mã lệnh sản xuất
        /// </summary>
        Workordercode = 2,
        /// <summary>
        /// Mã to
        /// </summary>
        Maincodeproduct = 3,
        /// <summary>
        /// Mã nhỏ
        /// </summary>
        Subcodeproduct = 4,
        /// <summary>
        /// Mã màu
        /// </summary>
        Colorcode = 5,
        /// <summary>
        /// Mã mùa
        /// </summary>
        Seasoncode = 6,
        /// <summary>
        /// Mã thị trường
        /// </summary>
        Marketcode = 7,
        /// <summary>
        /// Mã size
        /// </summary>
        Sizecode = 8,
    }
    public enum TypeTrainingMobile
    {
        HV = 1,//Hoc viec
        TT = 2,//Thuc tap
        TV = 3,//Thoi vu
    }
    public enum TypeQAQCMobile
    {
        QA = 1,
        QC = 2,

    }
    public enum ConfigGenTypeMobile
    {
        Productionrequirement = 1,
        Model = 2,
        Product = 3,
        GenTemPackage = 4,
        GenTemPallet = 5,
        GenTemCatton = 6,
        GenTemBox = 7,
        GenWorkorder = 8,
        GenCheckpoint = 9,
        GenCodeItem = 10,
        GenChecklist = 11,
        GenLotQC = 12,
        /// <summary>
        /// gen code khoan san luong
        /// </summary>
        GenOutputBond = 13,
    }
    public enum TypeFilterQAQC
    {
        /// <summary>
        /// Lệnh sản xuất
        /// </summary>
        Workordercode = 1,
        /// <summary>
        /// Mã to
        /// </summary>
        Maincodeproduct = 2,
        /// <summary>
        /// Mã đơn hàng
        /// </summary>
        Ordercode = 3,
    }
    public enum TypeRecordOuputMobile
    {
        /// <summary>
        /// Cắt
        /// </summary>
        Cut = 1,
        /// <summary>
        /// Nhận BTP
        /// </summary>
        ReceiveBTP = 2,
        /// <summary>
        /// Vào chuyền
        /// </summary>
        Inline = 3,
        /// <summary>
        /// Ra chuyền
        /// </summary>
        Outline = 4,
        /// <summary>
        /// Đi giặt
        /// </summary>
        Laundry = 5,
        /// <summary>
        /// Nhận giặt
        /// </summary>
        AfterWash = 6,
        /// <summary>
        /// Hoàn thiện
        /// </summary>
        Finished = 7,
        /// <summary>
        /// Nhập kho hoàn thiện
        /// </summary>
        WarehouseFinished = 8,
    }
    public enum TypeRecordNhuaMobile
    {
        /// <summary>
        /// Đúc
        /// </summary>
        Casting = 1,
    }
    public enum DropdownEnumMaterial
    {
        Unit = 1,
        TypeMaterial = 2,
        Material = 3,
        configgenCode = 4,
        locationMotherActive = 5,
        Location = 6,
        PackageType = 7,
        Partner = 8,
        PO = 9,
        Petitioner = 10,
        LocationSystem = 11,
        LocationLevel = 12,
        Inventorycheckinfo = 13,
        MaterialVer2 = 14,
        MaterialVer3 = 15,
        Color = 16,
        Model = 17,
        Customer = 18,
        CustomerProductCode = 19,
        /// <summary>
        /// mã đơn hàng
        /// </summary>
        Ordercode = 20,
        /// <summary>
        /// mã sản phẩm nội bộ
        /// </summary>
        Materialcode = 21,
        /// <summary>
        /// mã sản phẩm khách hàng
        /// </summary>
        Productcode = 22,
        /// <summary>
        /// danh sách khách hàng trong bảng parner
        /// </summary>
        CustomerPatner = 23,
        /// <summary>
        /// danh sách mã sản phẩm nội bộ theo sản phẩm khách hàng
        /// </summary>
        MaterialCodeOfCustomerCode = 24,
        /// <summary>
        /// danh sách mã sản phẩm nội bộ
        /// </summary>
        ProductCodeInteral = 25,
    }

    public enum DepartmentLevelMobile
    {
        /// <summary>
        /// "Ban giám đốc"
        /// </summary>
        cap_1 = 1,
        /// <summary>
        /// Phòng, xí nghiệp
        /// </summary>
        cap_2 = 2,
        /// <summary>
        /// Đơn vị thuộc phòng, phân xưởng thuộc xí nghiệp
        /// </summary>
        cap_3 = 3,
        /// <summary>
        /// Các tổ thuộc phân xưởng
        /// </summary>
        cap_4 = 4,
    }

    public enum UserWorkOrderStatus
    {
        /// <summary>
        ///  Lệnh do user khởi tạo,
        /// </summary>
        UserCreate = 1,
        /// <summary>
        /// Lệnh cần user phê duyệt
        /// </summary>
        UserApprove = 2
    }

    public enum BallotClassificationMobile
    {
        /// <summary>
        /// Lệnh sản xuất
        /// </summary>
        WorkOrder = 1,
        /// <summary>
        /// Phiếu giao việc
        /// </summary>
        TheDeliveryNote = 2,
        /// <summary>
        /// Phiếu đặt làm
        /// </summary>
        OrderForm = 3,
    }

    public static class ApprovalTypeApprovalMobile
    {
        /// <summary>
        /// phe duyet tài lieu sx
        /// </summary>
        public const string SOP = "SOP";
        /// <summary>
        /// phe duyet dinh muc
        /// </summary>
        public const string BOM = "BOM";
        /// <summary>
        /// phe duyet quy trinh
        /// </summary>
        public const string PROCESS = "PROCESS";
        /// <summary>
        ///  phe duyet lenh sx
        /// </summary>
        public const string WORKORDER = "WORKORDER";
        /// <summary>
        ///  cai dat dinh muc
        /// </summary>
        public const string SETTING_BOM = "SETTING_BOM";
        /// <summary>
        ///  cai dat dinh muc
        /// </summary>
        public const string RECORD_QUANTITY = "RECORD_QUANTITY";
    }
}
