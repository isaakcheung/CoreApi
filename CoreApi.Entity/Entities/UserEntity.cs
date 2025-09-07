using System;

namespace CoreApi.Entity.Entities
{
    /// <summary>
    /// 使用者資料表 Entity
    /// 層級：存取層
    /// 用途：對應資料庫使用者資料表，僅供 Repository 層資料存取與映射，包含基本欄位與維護欄位。
    /// </summary>
    public class UserEntity
    {
        /// <summary>流水號</summary>
        public int SeqNo { get; set; }
        /// <summary>使用者唯一識別碼</summary>
        public Guid Id { get; set; }
        /// <summary>建立者代號</summary>
        public Guid CreateBy { get; set; }
        /// <summary>建立時間</summary>
        public DateTime CreateTime { get; set; }
        /// <summary>最後修改者代號</summary>
        public Guid UpdateBy { get; set; }
        /// <summary>最後修改時間</summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>使用者名稱</summary>
        public string Name { get; set; }
        /// <summary>電子郵件</summary>
        public string Email { get; set; }
        /// <summary>密碼雜湊值</summary>
        public string PasswordHash { get; set; }
        /// <summary>是否啟用</summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 預設建構式
        /// </summary>
        public UserEntity() { }

        // 移除跨層依賴建構式，避免 Entity 依賴 Service 層 DTO
    }
}