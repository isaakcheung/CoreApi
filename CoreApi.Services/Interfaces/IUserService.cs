using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApi.Service.Models;
using CoreApi.Common.Models;
using CoreApi.Common.Enums;

namespace CoreApi.Service.Interfaces
{
    /// <summary>
    /// 使用者服務介面
    /// 層級：邏輯層
    /// 用途：定義使用者相關業務邏輯的服務合約，供 Service 層及 API 層依賴注入。
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 根據使用者唯一識別碼取得使用者資料
        /// </summary>
        /// <param name="id">使用者唯一識別碼</param>
        /// <returns>使用者資料物件</returns>
        Task<CoreApi.Common.Interfaces.IProcessResult> GetUser(Guid id);
        /// <summary>
        /// 依關鍵字查詢使用者資料
        /// </summary>
        /// <param name="keyword">查詢關鍵字</param>
        /// <returns>符合條件的使用者資料集合</returns>
        Task<CoreApi.Common.Interfaces.IProcessResult> GetUserByKeyword(string keyword);
        /// <summary>
        /// 新增使用者資料
        /// </summary>
        /// <param name="user">使用者資料物件</param>
        Task<CoreApi.Common.Interfaces.IProcessResult> CreateUser(UserDto user);
        /// <summary>
        /// 更新使用者資料
        /// </summary>
        /// <param name="user">使用者資料物件</param>
        Task<CoreApi.Common.Interfaces.IProcessResult> UpdateUser(UserDto user);
        /// <summary>
        /// 刪除指定使用者資料
        /// </summary>
        /// <param name="id">使用者唯一識別碼</param>
        Task<CoreApi.Common.Interfaces.IProcessResult> DeleteUser(Guid id);
        /// <summary>
        /// 產生指定使用者Id的 JWT Token
        /// </summary>
        /// <param name="userId">使用者唯一識別碼</param>
        /// <returns>JWT Token 字串</returns>
        Task<string> GenerateJwtTokenAsync(Guid userId);
    }
}