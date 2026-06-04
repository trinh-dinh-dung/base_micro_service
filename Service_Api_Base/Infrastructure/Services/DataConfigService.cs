using Application.Abstractions.Persistence;
using Application.Common.Appsetting;
using Application.IServices.DataConfig;
using Dapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class DataConfigService : IDataConfig
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _redisDb;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHostEnvironment _hostingEnvironment;
        private readonly IDbConnectionFactory _connectionFactory;

        public DataConfigService(
            IUnitOfWork unitOfWork,
            IHostEnvironment hostingEnvironment,
            IOptions<Appsettings> appsettings,
            IDbConnectionFactory connectionFactory)
        {
            _unitOfWork = unitOfWork;
            _hostingEnvironment = hostingEnvironment;
            _connectionFactory = connectionFactory;

            var settings = appsettings.Value;
            if (!string.IsNullOrEmpty(settings.UrlRedisCahe))
            {
                try
                {
                    _redis = ConnectionMultiplexer.Connect($"{settings.UrlRedisCahe},password={settings.RedisPass}");
                    _redisDb = _redis.GetDatabase();
                }
                catch (Exception)
                {
                    _redis = null;
                    _redisDb = null;
                }
            }
        }

        public string GetConnectStringByConnectName(string key)
        {
            var connectString = string.Empty;
            if (_redisDb != null)
                connectString = _redisDb.StringGet(key);

            if (!string.IsNullOrEmpty(connectString))
                return connectString;

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            var paramCreate = new DynamicParameters();
            paramCreate.Add("@key_connectName", key, DbType.String, ParameterDirection.Input);

            var sql = @"SELECT ""ConnectionString"" FROM ""TenantConnectionString"" WHERE ""ConnectionName"" = @key_connectName";
            var fromDb = connection.QueryFirstOrDefault<string>(sql, paramCreate);
            if (string.IsNullOrEmpty(fromDb))
                return string.Empty;

            connectString = fromDb;
            _redisDb?.StringSet(key, connectString, TimeSpan.FromMinutes(24));
            return connectString;
        }

        public Task<string> GetUserIDByUserName(string userName)
        {
            // Entity Users không còn trong schema hiện tại
            return Task.FromResult(string.Empty);
        }
    }
}
