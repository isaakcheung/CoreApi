using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CoreApi.Common.Helpers
{
    /// <summary>
    /// 提供 Dapper 讀寫分離連線管理
    /// </summary>
    public class SqlConnectionHelper
    {
        private readonly IConfiguration _configuration;

        public SqlConnectionHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 取得唯讀資料庫連線
        /// </summary>
        public async Task<IDbConnection> GetReadConnectionAsync()
        {
            var connStr = _configuration.GetConnectionString(Constants.DbConnectionKeys.ReadOnlyConnection);
            var conn = new SqlConnection(connStr);
            await conn.OpenAsync();
            return conn;
        }

        /// <summary>
        /// 取得可寫入資料庫連線
        /// </summary>
        public async Task<IDbConnection> GetWriteConnectionAsync()
        {
            var connStr = _configuration.GetConnectionString(Constants.DbConnectionKeys.ReadWriteConnection);
            var conn = new SqlConnection(connStr);
            await conn.OpenAsync();
            return conn;
        }
    }
}