using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Application.Abstractions.Persistence
{
    public static class DbConnectionHelper
    {
        public static async Task OpenConnectionAsync(IDbConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            if (connection.State == ConnectionState.Open)
                return;

            if (connection is DbConnection dbConnection)
            {
                await dbConnection.OpenAsync();
                return;
            }

            connection.Open();
        }

        public static void CloseConnection(IDbConnection connection)
        {
            if (connection == null)
                return;

            if (connection.State == ConnectionState.Open || connection.State == ConnectionState.Connecting)
                connection.Close();

            connection.Dispose();
        }
    }
}
