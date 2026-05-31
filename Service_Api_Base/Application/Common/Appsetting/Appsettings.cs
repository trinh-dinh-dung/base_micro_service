using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Appsetting
{
    public class Appsettings
    {
        public string DefaultConnection { get; set; }
        public string ConfigurationConnectString { get; set; }
        public string Api_Authority { get; set; }
        public string Api_Gateway { get; set; }
        //Rabbitmq config
        public string Rabbitmq_UserName { get; set; }
        public string Rabbitmq_Password { get; set; }
        public string Rabbitmq_VirtualHost { get; set; }
        public string Rabbitmq_HostName { get; set; }
        public int Rabbitmq_Port { get; set; }
        //Rabbitmq queue name
        public string Rabbitmq_Queue_Sop { get; set; }
        public string Rabbitmq_Queue_Log_Service { get; set; }
        public string Rabbitmq_Queue_Servce_Business { get; set; }
        public string MT_ENV { get; set; }
        //redis
        public int DBRedisCache { get; set; }
        public string PermissionKey { get; set; }
        public string DataProtectionKeys { get; set; }
        public string RedisUser { get; set; }
        public string RedisPass { get; set; }
        public string UrlRedisCahe { get; set; }
        //check permisson
        public bool? isCheckPermission { get; set; }
        public string Realm { get; set; }
        public string AuthServerUrl { get; set; }
        public string Resource { get; set; }
        
    }
}
