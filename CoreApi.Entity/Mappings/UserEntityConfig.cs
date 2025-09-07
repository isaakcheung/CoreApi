using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CoreApi.Entity.Entities;

namespace CoreApi.Entity.Mappings
{
    /// <summary>
    /// UserEntity Fluent API 設定
    /// </summary>
    /// <summary>
    /// 設定 UserEntity 與資料庫欄位的對應關係。
    /// </summary>
    public class UserEntityConfig : IEntityTypeConfiguration<UserEntity>
    {
        /// <summary>
        /// 設定 UserEntity 與資料表映射規則
        /// 包含主鍵、屬性必填、長度限制、索引等 Fluent API 設定
        /// </summary>
        /// <summary>
        /// 設定 UserEntity 的資料表結構與欄位屬性。
        /// </summary>
        /// <param name="builder">EntityTypeBuilder 實例</param>
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.SeqNo)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(u => u.Id)
                .IsRequired();

            builder.Property(u => u.CreateBy)
                .IsRequired();

            builder.Property(u => u.CreateTime)
                .IsRequired();

            builder.Property(u => u.UpdateBy)
                .IsRequired();

            builder.Property(u => u.UpdateTime)
                .IsRequired();

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(256);



            builder.Property(u => u.IsActive)
                .IsRequired();

            builder.HasIndex(u => u.Id);

        }
    }
}