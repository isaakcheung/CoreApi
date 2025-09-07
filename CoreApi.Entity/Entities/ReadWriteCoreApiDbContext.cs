using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

using CoreApi.Entity.Entities;

namespace CoreApi.Entity.Entities
{
    /// <summary>
    /// CoreApi Entity DbContext，註冊 UserEntity
    /// </summary>
    /// <summary>
    /// 讀寫專用 DbContext，提供資料庫寫入與查詢功能。
    /// </summary>
    public class ReadWriteCoreApiDbContext : DbContext
    {
        /// <summary>
        /// 建構子，注入 DbContextOptions
        /// </summary>
        /// <param name="options">DbContext 選項</param>
        public ReadWriteCoreApiDbContext(DbContextOptions<ReadWriteCoreApiDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Migration 工具設計時使用的無參數建構式
        /// </summary>
        public ReadWriteCoreApiDbContext()
        {
        }

        /// <summary>
        /// 使用者資料表 DbSet
        /// </summary>
        public DbSet<UserEntity> Users { get; set; }

        /// <summary>
        /// 設定資料庫連線字串（僅於未設定時自動載入 appsettings.json）
        /// </summary>
        /// <param name="optionsBuilder">DbContextOptionsBuilder</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
                var connectionString = config.GetConnectionString("ReadWriteConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        /// <summary>
        /// 註冊所有 IEntityTypeConfiguration，套用資料表結構設定
        /// </summary>
        /// <param name="modelBuilder">ModelBuilder</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 自動套用所有 IEntityTypeConfiguration
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReadWriteCoreApiDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}