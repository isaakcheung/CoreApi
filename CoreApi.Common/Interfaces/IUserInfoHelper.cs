using System;

namespace CoreApi.Common.Interfaces
{
    /// <summary>
    /// 定義取得目前使用者資訊的介面。
    /// </summary>
    public interface IUserInfoHelper
    {
        /// <summary>
        /// 取得目前使用者的 JWT Token 字串，若未登入則為 null。
        /// </summary>
        string? UserToken { get; }

        /// <summary>
        /// 取得目前使用者的唯一識別碼 (UserId)，若未登入則為 null。
        /// </summary>
        Guid? UserId { get; }

        /// <summary>
        /// 根據指定的 UserId 產生 JWT Token 字串。
        /// </summary>
        string EncodeToken(Guid userId);
    }
}