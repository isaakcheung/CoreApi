using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace CoreApi.API.Helpers
{
    /// <summary>
    /// 共用基底 Controller，提供取得 UserId 方法
    /// </summary>
    /// <summary>
    /// API 控制器基底類別，提供共用方法。
    /// </summary>
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// 取得目前登入使用者的 UserId (Guid)
        /// </summary>
        /// <returns>UserId，若不存在則回傳 Guid.Empty</returns>
        /// <summary>
        /// 取得目前 HttpContext 使用者的 UserId。
        /// </summary>
        /// <returns>使用者唯一識別碼，若無則為 null</returns>
        protected Guid? GetUserId()
        {
            var userIdClaim = HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            return null;
        }
    }
}