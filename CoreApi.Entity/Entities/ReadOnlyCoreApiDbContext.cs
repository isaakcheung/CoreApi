using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CoreApi.Common;

namespace CoreApi.Entity.Entities
{
    /// <summary>
    /// 讀取專用 DbContext，連線字串使用 ReadOnlyConnection
    /// </summary>
    /// <summary>
    /// 唯讀專用 DbContext，僅提供資料查詢功能，連線字串使用 ReadOnlyConnection。
    /// </summary>
    public class ReadOnlyCoreApiDbContext : ReadWriteCoreApiDbContext
    {
        /// <summary>
        /// 建構子，注入 DbContextOptions
        /// </summary>
        /// <param name="options">DbContext 選項</param>
        public ReadOnlyCoreApiDbContext(DbContextOptions<ReadOnlyCoreApiDbContext> options)
            : base()
        {
        }

        /// <summary>
        /// Migration 工具設計時使用的無參數建構式
        /// </summary>
        public ReadOnlyCoreApiDbContext()
        {
        }

        /// <summary>
        /// 設定唯讀資料庫連線字串（僅於未設定時自動載入 appsettings.json）
        /// </summary>
        /// <param name="optionsBuilder">DbContextOptionsBuilder</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
                var connectionString = config.GetConnectionString(Constants.DbConnectionKeys.ReadOnlyConnection);
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}