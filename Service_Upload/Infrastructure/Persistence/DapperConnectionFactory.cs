using Application.Abstractions.Persistence;
using Application.Common.Appsetting;
using Microsoft.Extensions.Options;
using Npgsql;
using System.Data;

namespace Infrastructure.Persistence
{
    public class DapperConnectionFactory : IDbConnectionFactory
    {
        private readonly IOptions<Appsettings> _settings;

        public DapperConnectionFactory(IOptions<Appsettings> settings)
        {
            _settings = settings;
        }

        public string ConnectionString => _settings.Value.ConfigurationConnectString;

        public IDbConnection CreateConnection() => new NpgsqlConnection(ConnectionString);
    }
}
