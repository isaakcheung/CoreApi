#nullable enable
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApi.Service.Interfaces;
using CoreApi.Service.Models;
using CoreApi.Common.Models;
using CoreApi.Common.Enums;
using CoreApi.Common.Helpers;
using CoreApi.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using CoreApi.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

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
        private Microsoft.Extensions.Logging.ILogger<UserService> _logger => _serviceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<UserService>>();

        public UserService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 根據使用者唯一識別碼取得使用者資料
        /// </summary>
        public async Task<IProcessResult> GetUser(Guid id)
        {
            try
            {
                var entity = await _userRepository.GetByIdAsync(id);
                if (entity == null)
                    throw new ArgumentException("查無資料");
                var dto = new UserDto(entity);
                return GeneralProcessResultFactory.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUser 發生例外，id={Id}", id);
                return GeneralProcessResultFactory.Fail(exception: ex);
            }
        }

        /// <summary>
        /// 依關鍵字查詢使用者資料
        /// </summary>
        public async Task<IProcessResult> GetUserByKeyword(string? keyword, int? skip = null, int? take = null)
        {
            try
            {
                var pageList = await _userRepository.GetUserByKeywordAsync(keyword, skip, take);
                var dtos = pageList.Select(x => new UserDto(x)).ToList();
                var dtoPageList = new PageList<UserDto>(dtos, pageList.TotalCount);
                return GeneralProcessResultFactory.Success(dtoPageList, totalCount: pageList.TotalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserByKeyword 發生例外，keyword={Keyword}, skip={Skip}, take={Take}", keyword, skip, take);
                return GeneralProcessResultFactory.Fail(exception: ex);
            }
        }

        /// <summary>
        /// 新增使用者資料
        /// </summary>
        public async Task<IProcessResult> CreateUser(UserDto user)
        {
            try
            {
                var entity = user.ToEntity();
                entity.IsActive = true; // 建立時強制啟用
                entity.CreateBy = _userInfoHelper.UserId ?? Guid.Empty;
                entity.UpdateBy = _userInfoHelper.UserId ?? Guid.Empty;
                await _userRepository.AddAsync(entity);
                return GeneralProcessResultFactory.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateUser 發生例外，user={User}", user);
                return GeneralProcessResultFactory.Fail(exception: ex);
            }
        }

        /// <summary>
        /// 更新使用者資料
        /// </summary>
        public async Task<IProcessResult> UpdateUser(UserDto user)
        {
            try
            {
                // 先取得原始資料
                var origin = await _userRepository.GetByIdAsync(user.Id ?? Guid.Empty);
                if (origin == null)
                    throw new ArgumentException("查無資料");

                // 轉型：先用 UserDto 產生新 Entity，再保留原始 CreateTime/CreateBy
                var updated = user.ToEntity(origin);
                // 強制更新 UpdateTime 與 UpdateBy
                updated.UpdateTime = DateTime.UtcNow;
                updated.UpdateBy = _userInfoHelper.UserId ?? Guid.Empty;

                await _userRepository.UpdateAsync(updated);
                return GeneralProcessResultFactory.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateUser 發生例外，user={User}", user);
                return GeneralProcessResultFactory.Fail(exception: ex);
            }
        }

        /// <summary>
        /// 刪除指定使用者資料
        /// </summary>
        public async Task<IProcessResult> DeleteUser(Guid id)
        {
            try
            {
                var entity = await _userRepository.GetByIdAsync(id);
                if (entity == null)
                    throw new ArgumentException("查無資料");

                entity.IsActive = false;
                // 強制更新 UpdateTime 與 UpdateBy
                entity.UpdateTime = DateTime.UtcNow;
                entity.UpdateBy = _userInfoHelper.UserId ?? Guid.Empty;
                await _userRepository.UpdateAsync(entity);
                return GeneralProcessResultFactory.Success<bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteUser 發生例外，id={Id}", id);
                return GeneralProcessResultFactory.Fail(exception: ex);
            }
        }

        /// <summary>
        /// 取得指定使用者Id的 JWT Token，sub 欄位為 UserId，回傳 IProcessResult
        /// </summary>
        public async Task<IProcessResult> GetUserTokenById(Guid userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    throw new ArgumentException("查無使用者");

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Name ?? ""),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim("sub", user.Id.ToString())
                };

                var issuer = _configuration?["JwtSettings:Issuer"] ?? "yourIssuer";
                var audience = _configuration?["JwtSettings:Audience"] ?? "yourAudience";
                var signKey = _configuration?["JwtSettings:SignKey"] ?? "yourSignKey";
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(signKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds
                );

                var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                return GeneralProcessResultFactory.Success<string>(jwt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetUserTokenById 發生例外，userId={UserId}", userId);
                return GeneralProcessResultFactory.Fail<string>(exception: ex);
            }
        }

    }
}