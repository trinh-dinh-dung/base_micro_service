using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Status
{
    public static class TenantCommon
    {

        public class TypeTenantConection
        {
            public const string KeyNameDB_EVOMES_MAINTENANCEMANAGEMENT = "maintenance";
            public const string KeyNameDB_EVOMES_MATERIAL_MANAGEMENT = "material";
            public const string KeyNameDB_EVOMES_PRODUCTION_MANAGEMENT = "production";

            public const string KeyNameDB_MesIdentityPermission = "identity";
            public const string KeyNameDB_EVOMES_NOTIFICATION = "notification";
        }

        public enum TenantStatus
        {
            New = 1,
            Confirm = 2,
            Operate = 3,
            Expired = 4,
            Cancel = 5,
        }
        public enum TenantDbStatus
        {
            New = 1,
            Ready = 2,
            Error = 3
        }
        public static string ExecutionDirectoryPathName(string file)
        {
            string dirPath = System.IO.Directory.GetCurrentDirectory();
            return Path.GetFullPath(Path.Combine(dirPath, file));
        }
        public enum BusinessServiceTypeSendRabbitMq
        {
            SopService = 1,
            MaintainanceManagementService = 2,
            RegesterMesCloud = 3
        }
        public enum TypeRegesterMesCloud
        {
            SendEmailInfomationAccount = 1
        }
    }

    public static class EmailCommon
    {
        public class KeyEmailTemplate
        {
            public const string Email_Regester_Success = "Email_Regester_Success";
        }

    }
}
