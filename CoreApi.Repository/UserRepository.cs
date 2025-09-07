using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CoreApi.Repository.Interfaces;
using CoreApi.Entity.Entities;
using CoreApi.Common.Models;
using CoreApi.Common.Helpers;

namespace CoreApi.Repository
{
    /// <summary>
    /// 使用者資料存取層
    /// 層級：存取層
    /// 用途：負責存取資料庫中的使用者資料，實作 IUserRepository 介面，封裝 EF Core 與資料查詢邏輯。
    /// 提供 UserEntity 資料存取的 Repository 實作。
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly IServiceProvider _serviceProvider;
        private ReadWriteCoreApiDbContext _readWriteDbContext => _serviceProvider.GetRequiredService<ReadWriteCoreApiDbContext>();
        private ReadOnlyCoreApiDbContext _readOnlyDbContext => _serviceProvider.GetRequiredService<ReadOnlyCoreApiDbContext>();
        private SqlConnectionHelper _sqlConnectionHelper => _serviceProvider.GetRequiredService<SqlConnectionHelper>();

        /// <summary>
        /// 建構 UserRepository，注入服務提供者
        /// </summary>
        /// <param name="serviceProvider">DI 注入的服務提供者</param>
        public UserRepository(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// 依唯一識別碼取得使用者資料
        /// </summary>
        /// <param name="id">使用者唯一識別碼</param>
        /// <returns>使用者資料物件</returns>
        /// <summary>
        /// 依唯一識別碼取得使用者資料。
        /// </summary>
        /// <param name="id">使用者唯一識別碼</param>
        /// <returns>UserEntity 物件</returns>
        public async Task<UserEntity> GetByIdAsync(Guid id)
        {
            // 使用唯讀DbContext查詢
            return await _readOnlyDbContext.Users.FindAsync(id);
        }

        /// <summary>
        /// 新增一筆使用者資料。
        /// </summary>
        /// <param name="user">UserEntity 物件</param>
        public async Task AddAsync(UserEntity user)
        {
            // 使用DbContext新增
            await _readWriteDbContext.Users.AddAsync(user);
            await _readWriteDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 更新一筆使用者資料。
        /// </summary>
        /// <param name="user">UserEntity 物件</param>
        public async Task UpdateAsync(UserEntity user)
        {
            // 使用DbContext更新，避免更新 SeqNo（identity 欄位）
            // 僅追蹤非 SeqNo 欄位，避免 EF 產生 SeqNo 更新語句
            var entry = _readWriteDbContext.Entry(user);
            if (entry.State == EntityState.Detached)
            {
                _readWriteDbContext.Attach(user);
                entry = _readWriteDbContext.Entry(user);
            }
            await _readWriteDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 刪除指定 UserId 的使用者資料。
        /// </summary>
        /// <param name="id">使用者唯一識別碼</param>
        public async Task DeleteAsync(Guid id)
        {
            // 使用DbContext刪除
            var user = await _readWriteDbContext.Users.FindAsync(id);
            if (user != null)
            {
                _readWriteDbContext.Users.Remove(user);
                await _readWriteDbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// 依關鍵字查詢使用者清單（分頁）。
        /// </summary>
        /// <param name="keyword">查詢關鍵字</param>
        /// <returns>分頁的 UserEntity 清單</returns>
        public async Task<PageList<UserEntity>> GetUserByKeywordAsync(string keyword)
        {
            // 使用唯讀DbContext查詢
            var query = _readOnlyDbContext.Users
                .Where(u => u.Name.Contains(keyword) || u.Email.Contains(keyword));
            var list = await query.ToListAsync();
            return new PageList<UserEntity>(list, list.Count);
        }
    }
}