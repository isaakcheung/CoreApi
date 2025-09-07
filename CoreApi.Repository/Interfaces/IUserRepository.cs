using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApi.Entity.Entities;
using CoreApi.Common.Models;

namespace CoreApi.Repository.Interfaces
{
    /// <summary>
    /// 使用者資料存取介面
    /// 層級：存取層
    /// 用途：定義存取資料庫中使用者資料的操作合約，供 Repository 層實作。
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// 依關鍵字查詢使用者資料
        /// </summary>
        /// <param name="keyword">查詢關鍵字</param>
        /// <returns>分頁的使用者資料集合</returns>
        Task<PageList<UserEntity>> GetUserByKeywordAsync(string keyword);

        /// <summary>
        /// 依唯一識別碼取得使用者資料
        /// </summary>
        /// <param name="id">使用者唯一識別碼</param>
        /// <returns>使用者資料物件</returns>
        Task<UserEntity> GetByIdAsync(Guid id);

        /// <summary>
        /// 新增使用者資料
        /// </summary>
        /// <param name="user">使用者資料實體</param>
        Task AddAsync(UserEntity user);

        /// <summary>
        /// 更新使用者資料
        /// </summary>
        /// <param name="user">使用者資料實體</param>
        Task UpdateAsync(UserEntity user);

        /// <summary>
        /// 刪除指定使用者資料
        /// </summary>
        /// <param name="id">使用者唯一識別碼</param>
        Task DeleteAsync(Guid id);
    }
}