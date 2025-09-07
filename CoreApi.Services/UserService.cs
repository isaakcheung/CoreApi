using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Service.Interfaces;
using CoreApi.Service.Models;
using CoreApi.Common.Models;
using CoreApi.Common.Enums;
using Microsoft.Extensions.Configuration;
using CoreApi.Repository.Interfaces;
using CoreApi.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CoreApi.Service
{
    /// <summary>
    /// 使用者服務
    /// 層級：邏輯層
    /// 用途：負責處理使用者相關的業務邏輯，透過延遲注入取得 Repository 與組態設定。
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IServiceProvider _serviceProvider;
        private IUserRepository _userRepository => _serviceProvider.GetRequiredService<IUserRepository>();
        private IConfiguration _configuration => _serviceProvider.GetRequiredService<IConfiguration>();
        private IUserInfoHelper _userInfoHelper => _serviceProvider.GetRequiredService<IUserInfoHelper>();

        public UserService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 根據使用者唯一識別碼取得使用者資料
        /// </summary>
        public async Task<CoreApi.Common.Interfaces.IProcessResult> GetUser(Guid id)
        {
            try
            {
                var entity = await _userRepository.GetByIdAsync(id);
                if (entity == null)
                    return CoreApi.Common.Helpers.GeneralProcessResultFactory.Success<UserDto>(null, "查無資料", GeneralResultStatusEnum.Fail);
                var dto = new UserDto(entity);
                return CoreApi.Common.Helpers.GeneralProcessResultFactory.Success(dto, "查詢成功", GeneralResultStatusEnum.Success);
            }
            catch (Exception ex)
            {
                return CoreApi.Common.Helpers.GeneralProcessResultFactory.Fail("查詢失敗", exception: ex);
            }
        }

        /// <summary>
        /// 依關鍵字查詢使用者資料
        /// </summary>
        public async Task<CoreApi.Common.Interfaces.IProcessResult> GetUserByKeyword(string keyword)
        {
            try
            {
                var pageList = await _userRepository.GetUserByKeywordAsync(keyword);
                var dtos = pageList.Select(x => new UserDto(x)).ToList();
                var dtoPageList = new PageList<UserDto>(dtos, pageList.TotalCount);
                return CoreApi.Common.Helpers.GeneralProcessResultFactory.Success(dtoPageList, "查詢成功", GeneralResultStatusEnum.Success, totalCount: pageList.TotalCount);
            }
            catch (Exception ex)
            {
                return CoreApi.Common.Helpers.GeneralProcessResultFactory.Fail("查詢失敗", exception: ex);
            }
        }

        /// <summary>
        /// 新增使用者資料
        /// </summary>
        public async Task<CoreApi.Common.Interfaces.IProcessResult> CreateUser(UserDto user)
        {
            try
            {
                var entity = user.ToEntity();
                entity.IsActive = true; // 建立時強制啟用
                entity.CreateBy = _userInfoHelper.UserId ?? Guid.Empty;
                entity.UpdateBy = _userInfoHelper.UserId ?? Guid.Empty;
                await _userRepository.AddAsync(entity);
                return CoreApi.Common.Helpers.GeneralProcessResultFactory.Success(true, "建立成功", GeneralResultStatusEnum.Success);
            }
            catch (Exception ex)
            {
                return CoreApi.Common.Helpers.GeneralProcessResultFactory.Fail("建立失敗", exception: ex);
            }
        }

        /// <summary>
        /// 更新使用者資料
        /// </summary>
        public async Task<CoreApi.Common.Interfaces.IProcessResult> UpdateUser(UserDto user)
        {
            try
            {
                // 先取得原始資料
                var origin = await _userRepository.GetByIdAsync(user.Id ?? Guid.Empty);
                if (origin == null)
                    return CoreApi.Common.Helpers.GeneralProcessResultFactory.Success<bool>(false, "查無資料", GeneralResultStatusEnum.Fail);

                // 轉型：先用 UserDto 產生新 Entity，再保留原始 CreateTime/CreateBy
                var updated = user.ToEntity(origin);
                // 強制更新 UpdateTime 與 UpdateBy
                updated.UpdateTime = DateTime.UtcNow;
                updated.UpdateBy = _userInfoHelper.UserId ?? Guid.Empty;

                await _userRepository.UpdateAsync(updated);
                return CoreApi.Common.Helpers.GeneralProcessResultFactory.Success(true, "更新成功", GeneralResultStatusEnum.Success);
            }
            catch (Exception ex)
            {
                return CoreApi.Common.Helpers.GeneralProcessResultFactory.Fail("更新失敗", exception: ex);
            }
        }

        /// <summary>
        /// 刪除指定使用者資料
        /// </summary>
        public async Task<CoreApi.Common.Interfaces.IProcessResult> DeleteUser(Guid id)
        {
            try
            {
                var entity = await _userRepository.GetByIdAsync(id);
                if (entity == null)
                    return CoreApi.Common.Helpers.GeneralProcessResultFactory.Success<bool>(false, "查無資料", GeneralResultStatusEnum.Fail);

                entity.IsActive = false;
                // 強制更新 UpdateTime 與 UpdateBy
                entity.UpdateTime = DateTime.UtcNow;
                entity.UpdateBy = _userInfoHelper.UserId ?? Guid.Empty;
                await _userRepository.UpdateAsync(entity);
                return CoreApi.Common.Helpers.GeneralProcessResultFactory.Success(true, "刪除成功", GeneralResultStatusEnum.Success);
            }
            catch (Exception ex)
            {
                return CoreApi.Common.Helpers.GeneralProcessResultFactory.Fail("刪除失敗", exception: ex);
            }
        }

        /// <summary>
        /// 產生指定使用者Id的 JWT Token，sub 欄位為 UserId
        /// </summary>
        public async Task<string> GenerateJwtTokenAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            var claims = new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Name ?? ""),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
                new System.Security.Claims.Claim("sub", user.Id.ToString())
            };

            var issuer = _configuration?["JwtSettings:Issuer"] ?? "yourIssuer";
            var audience = _configuration?["JwtSettings:Audience"] ?? "yourAudience";
            var signKey = _configuration?["JwtSettings:SignKey"] ?? "yourSignKey";
            var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(signKey));
            var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}