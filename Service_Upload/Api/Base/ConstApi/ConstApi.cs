namespace Api.Base.ConstApi
{
    public class ConstApi
    {
        public const string CreateAccountLogin = "api/identity/account-manage/create-account-login-systemmes";
        public const string AccountResetPassword = "api/identity/account-manage/create-account-resetpass-systemmes";
        public const string GetALlLocation = "/api/material-management-service/location/get-all-location";
        public const string CreateLocation = "/api/material-management-service/location/create-location";
        public const string UpdateLocation = "/api/material-management-service/location/update-location";
        public const string GetAllMachine = "/api/maintenance-management-service/machine/get-all-dropdown";
        public const string GetAllMachineInWorkUnit = "/api/maintenance-management-service/call-service/get-list-detail-machine-in-work-unit";
        public const string GetListMachineTypeNameInStepProcess = "/api/maintenance-management-service/call-service/get-list-machine-type-name-in-step-process";
        public const string getDetailMachineByCode = "api/maintenance-management-service/call-service/get-detail-machine-by-machinecode";
        public const string getLocationLevelId = "api/material-management-service/call-api/get-location-id/";
        public const string updateMaterialByProduct = "api/material-management-service/call-api/update-material-by-product/";
        public const string updateMaterialByWorker = "api/material-management-service/call-api/update-material-by-worker/";
        public const string scanCodeToSetup = "/api/maintenance-management-service/call-service/get-Worker-machine-by-code/";
        public const string getListMachineByString = "/api/maintenance-management-service/call-service/get-list-worker-machine";
        public const string apiCheckPermissionUser = "api/identity/permission/api-user-permission";
        public const string deliveryNoteDataHis = "/api/material-management-service/delivery-note/get-delivery-note-data/";
        public const string checkDataOrderExist = "/api/material-management-service/call-api/check-data-order-exist";
        public const string getQtyOrder = "/api/material-management-service/call-api/get-qty-order";
        public const string getStringMaterialByWorkcenterApi = "/api/material-management-service/call-api/get-dropdown-mobile-by-code";
        public const string checkPermissionByUserId = "/api/production-management-service/permission/check-all-permission-api-by-user";

        // api send noti
        public const string SendNotificationSignalr = "/api/hubs/chat/send-noti";

    }
}
