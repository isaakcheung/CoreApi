using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using CoreApi.Common.Helpers;
using CoreApi.Common.Models;
using CoreApi.Common.Enums;

namespace CoreApi.API.Filters
{
    /// <summary>
    /// 檢查 ClaimsPrincipal 中是否具有 UserId (NameIdentifier) 的自訂授權屬性
    /// </summary>
    public class TokenAuthorizeAttribute : Attribute, IAuthorizationFilter
    {

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            var userId = user?.Claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new JsonResult(new ApiResult()
                {
                    IsSuccess = false,
                    StatusCode = (int)GeneralResultStatusEnum.Fail,
                    Status = "Fail",
                    Message = "TokenAuthorize 無法授權"
                })
                { StatusCode = 200 };
            }
        }
    }
}
