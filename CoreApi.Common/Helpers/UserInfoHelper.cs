using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using CoreApi.Common;
using System.IdentityModel.Tokens.Jwt;

using CoreApi.Common.Interfaces;

namespace CoreApi.Common.Helpers
{
    /// <summary>
    /// 提供目前使用者資訊的輔助類別，實作 IUserInfoHelper。
    /// UserToken、UserId 皆從延遲注入的 IHttpContextAccessor 取得。
    /// </summary>
    /// <summary>
    /// 提供使用者資訊與 JWT 處理相關功能的輔助類別。
    /// </summary>
    public class UserInfoHelper : IUserInfoHelper
    {
        private readonly IServiceProvider _serviceProvider;

        private IHttpContextAccessor HttpContextAccessor =>
            _serviceProvider.GetRequiredService<IHttpContextAccessor>();

        private IConfiguration Configuration =>
            _serviceProvider.GetRequiredService<IConfiguration>();

        /// <summary>
        /// 取得目前使用者的 Token（從 Authorization Bearer 解析）。
        /// </summary>
        /// <summary>
        /// 取得目前使用者的 JWT Token。
        /// </summary>
        public string? UserToken
        {
            get
            {
                var user = HttpContextAccessor.HttpContext?.User;
                return user?.FindFirst(Constants.UserInfoHelper.UserToken)?.Value;
            }
        }

        /// <summary>
        /// 取得目前使用者的 UserId（從 Claims 解析）。
        /// </summary>
        /// <summary>
        /// 取得目前使用者的唯一識別碼 (UserId)。
        /// </summary>
        public Guid? UserId
        {
            get
            {
                var user = HttpContextAccessor.HttpContext?.User;
                var claim = user?.FindFirst(Constants.UserInfoHelper.UserId);
                if (claim != null && Guid.TryParse(claim.Value, out var userId))
                {
                    return userId;
                }
                return null;
            }
        }

        public UserInfoHelper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 驗證 JWT Token 後，將 sub 及 Token 以 UserToken claim 置入 context.User
        /// </summary>
        /// <param name="context">TokenValidatedContext</param>
        /// <summary>
        /// 從 TokenValidatedContext 解析並準備使用者資訊。
        /// </summary>
        /// <param name="context">JWT 驗證完成後的上下文</param>
        public static Task PrepareUserInfo(TokenValidatedContext context)
        {
            var principal = context.Principal;
            var identity = principal?.Identity as ClaimsIdentity;
            var token = context.SecurityToken as JwtSecurityToken;
            if (identity != null && token != null)
            {
                // 取出 sub claim
                var sub = token.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                if (!string.IsNullOrEmpty(sub))
                {
                    identity.AddClaim(new Claim(Constants.UserInfoHelper.UserId, sub));
                }
                // 將原始 Token 也加入 claim
                identity.AddClaim(new Claim(Constants.UserInfoHelper.UserToken, token.RawData));
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 產生 JWT Token，僅需傳入 UserId
        /// </summary>
        /// <summary>
        /// 產生指定 UserId 的 JWT Token。
        /// </summary>
        /// <param name="userId">使用者唯一識別碼</param>
        /// <returns>JWT Token 字串</returns>
        public string EncodeToken(Guid userId)
        {
            var issuer = Configuration[Constants.JwtSettings.Issuer] ?? string.Empty;
            var audience = Configuration[Constants.JwtSettings.Audience] ?? string.Empty;
            var signKey = Configuration[Constants.JwtSettings.SignKey] ?? string.Empty;

            var claims = new[]
            {
                new Claim("sub", userId.ToString()),
                new Claim(Constants.UserInfoHelper.UserId, userId.ToString())
            };

            var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(signKey));
            var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        /// <summary>
        /// 提供 JwtBearerOptions 設定的 Action，統一搬移 Jwt 驗證參數
        /// </summary>
        /// <summary>
        /// 產生 JwtBearerOptions 設定委派，供 JWT 驗證中介軟體使用。
        /// </summary>
        /// <param name="configuration">組態設定</param>
        /// <returns>JwtBearerOptions 設定委派</returns>
        public static Action<JwtBearerOptions> JwtBearerOptionsAction(IConfiguration configuration)
        {
            return options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration[Constants.JwtSettings.Issuer],
                    ValidAudience = configuration[Constants.JwtSettings.Audience],
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(configuration[Constants.JwtSettings.SignKey] ?? string.Empty))
                };
                options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                {
                    OnTokenValidated = PrepareUserInfo
                };
            };
        }
    }
}