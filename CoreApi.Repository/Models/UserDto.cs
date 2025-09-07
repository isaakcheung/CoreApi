using System;
using CoreApi.Entity.Entities;

namespace CoreApi.Repository.Models
{
    /// <summary>
    /// 使用者資料傳輸物件 (DTO)
    /// 層級：應用層/邏輯層
    /// 用途：用於 API、Service、Repository 間交換使用者資料，隔離 Entity 結構，強化資料安全與維護性。
    /// </summary>
    /// <summary>
    /// 對應資料存取層的使用者資料傳輸物件（DTO）。
    /// </summary>
    public class UserDto
    {
        /// <summary>流水號</summary>
        public int? SeqNo { get; set; }
        /// <summary>使用者唯一識別碼</summary>
        public Guid? Id { get; set; }
        /// <summary>使用者名稱</summary>
        public string Name { get; set; }
        /// <summary>電子郵件</summary>
        public string Email { get; set; }
        /// <summary>密碼雜湊值</summary>
        public string PasswordHash { get; set; }
        /// <summary>建立者代號</summary>
        public Guid? CreateBy { get; set; }
        /// <summary>建立時間</summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>最後修改者代號</summary>
        public Guid? UpdateBy { get; set; }
        /// <summary>最後修改時間</summary>
        public DateTime? UpdateTime { get; set; }
        /// <summary>是否啟用</summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// 預設建構式
        /// </summary>
        public UserDto() { }

        /// <summary>
        /// 由 UserEntity 轉型建構 UserDto
        /// </summary>
        /// <param name="entity">來源 UserEntity 物件</param>
        public UserDto(UserEntity entity)
        {
            SeqNo = entity.SeqNo;
            Id = entity.Id;
            Name = entity.Name;
            Email = entity.Email;
            PasswordHash = entity.PasswordHash;
            CreateBy = entity.CreateBy;
            CreateTime = entity.CreateTime;
            UpdateBy = entity.UpdateBy;
            UpdateTime = entity.UpdateTime;
            IsActive = entity.IsActive;
        }
    }
}