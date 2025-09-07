#nullable enable
using System;
using CoreApi.Entity.Entities;

namespace CoreApi.Service.Models
{
    /// <summary>
    /// 使用者資料傳輸物件 (DTO)，用於 API、Service、Repository 間交換使用者資料，隔離 Entity 結構，強化資料安全與維護性。
    /// </summary>
    public class UserDto
    {
        /// <summary>流水號</summary>
        public int? SeqNo { get; set; }
        /// <summary>使用者唯一識別碼</summary>
        public Guid? Id { get; set; }
        /// <summary>使用者名稱</summary>
        public string? Name { get; set; }
        /// <summary>電子郵件</summary>
        public string? Email { get; set; }
        /// <summary>密碼雜湊值</summary>
        public string? PasswordHash { get; set; }
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
        /// <param name="dto">來源 UserEntity 物件</param>
        public UserDto(UserEntity dto)
        {
            SeqNo = dto.SeqNo;
            Id = dto.Id;
            Name = dto.Name;
            Email = dto.Email;
            PasswordHash = dto.PasswordHash;
            CreateBy = dto.CreateBy;
            CreateTime = dto.CreateTime;
            UpdateBy = dto.UpdateBy;
            UpdateTime = dto.UpdateTime;
            IsActive = dto.IsActive;
        }

        /// <summary>
        /// 轉型為 UserEntity 物件，可選擇性傳入現有 UserEntity 進行更新（供資料存取層使用）。
        /// </summary>
        /// <param name="entity">可選擇性傳入現有 UserEntity</param>
        /// <returns>對應的 UserEntity 物件</returns>
        public UserEntity ToEntity(UserEntity? entity = null)
        {
            var target = entity ?? new UserEntity();
            // SeqNo 不應於更新時指定，避免 identity 欄位異常
            target.Id = this.Id ?? target.Id;
            target.Name = this.Name;
            target.Email = this.Email;
            target.PasswordHash = this.PasswordHash;
            target.CreateBy = this.CreateBy ?? target.CreateBy;
            target.CreateTime = this.CreateTime ?? target.CreateTime;
            target.UpdateBy = this.UpdateBy ?? target.UpdateBy;
            target.UpdateTime = this.UpdateTime ?? DateTime.UtcNow;
            target.IsActive = this.IsActive ?? target.IsActive;
            return target;
        }
    }
}
