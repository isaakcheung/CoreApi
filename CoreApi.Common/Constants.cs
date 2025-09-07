namespace CoreApi.Common
{
    /// <summary>
    /// 全域常數定義
    /// 層級：共用層
    /// 用途：集中管理系統常數，依 AppSetting 層級建立對應類別與屬性，供各層引用。
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// JWT 設定相關常數
        /// 層級：AppSetting.JwtSettings
        /// 用途：集中管理 JWT 驗證相關設定鍵值
        /// </summary>
        public static class JwtSettings
        {
            /// <summary>
            /// JWT 發行者設定鍵值（Issuer）
            /// 用途：指定 JWT Token 的發行者，對應 appsettings:JwtSettings:Issuer
            /// </summary>
            public static string Issuer => $"{nameof(JwtSettings)}:{nameof(Issuer)}";
            /// <summary>
            /// JWT 受眾設定鍵值（Audience）
            /// 用途：指定 JWT Token 的受眾，對應 appsettings:JwtSettings:Audience
            /// </summary>
            public static string Audience => $"{nameof(JwtSettings)}:{nameof(Audience)}";
            /// <summary>
            /// JWT 簽章金鑰設定鍵值（SignKey）
            /// 用途：指定 JWT Token 的簽章金鑰，對應 appsettings:JwtSettings:SignKey
            /// </summary>
            public static string SignKey => $"{nameof(JwtSettings)}:{nameof(SignKey)}";
        }

        /// <summary>
        /// UserInfoHelper Claim 常數
        /// </summary>
        public static class UserInfoHelper
        {
            public const string UserId = nameof(UserId);
            public const string UserToken = nameof(UserToken);
        }

        /// <summary>
        /// 資料庫連線字串鍵值常數
        /// 層級：AppSetting.ConnectionStrings
        /// 用途：集中管理資料庫連線字串設定鍵值
        /// </summary>
        public static class DbConnectionKeys
        {
            /// <summary>
            /// 讀寫資料庫連線字串設定鍵值，對應 appsettings:ConnectionStrings:ReadWriteConnection
            /// </summary>
            public const string ReadWriteConnection = nameof(ReadWriteConnection);

            /// <summary>
            /// 唯讀資料庫連線字串設定鍵值，對應 appsettings:ConnectionStrings:ReadOnlyConnection
            /// </summary>
            public const string ReadOnlyConnection = nameof(ReadOnlyConnection);
        }
    }
}