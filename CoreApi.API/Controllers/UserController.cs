using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApi.Service.Interfaces;
using CoreApi.Service.Models;
using CoreApi.Common.Models;
using CoreApi.Common.Interfaces;
using CoreApi.Common.Enums;
using CoreApi.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using CoreApi.Common.Helpers;
using CoreApi.API.Filters;

namespace CoreApi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    /// <summary>
    /// 使用者 API 控制器
    /// 層級：應用層
    /// 用途：負責處理使用者資料的建立、查詢、更新、刪除等 RESTful API 請求。
    /// 依延遲注入模式取得 IUserService，符合 Clean Architecture。
    /// </summary>
    public class UserController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private IUserService _userService => _serviceProvider.GetRequiredService<IUserService>();
        private IUserInfoHelper _userInfoHelper => _serviceProvider.GetRequiredService<IUserInfoHelper>();
    
        public UserController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpGet("{id}")]
        
        /// <summary>
        /// 取得指定使用者資料
        /// </summary>
        /// <param name="id">使用者唯一識別碼</param>
        /// <returns>使用者資料物件，若無則回傳 404</returns>
        public async Task<IProcessResult> GetUser(Guid id)
        {
            var result = await _userService.GetUser(id);
            return result;
        }

        [HttpGet("Search")]
        
        /// <summary>
        /// 依關鍵字查詢使用者資料
        /// </summary>
        /// <param name="keyword">查詢關鍵字</param>
        /// <returns>符合條件的使用者資料集合</returns>
        public async Task<IProcessResult> GetUserByKeyword([FromQuery] string keyword, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _userService.GetUserByKeyword(keyword);
            return result;
        }

        [HttpPost]
        /// <summary>
        /// 新增使用者資料
        /// </summary>
        /// <param name="user">使用者資料物件</param>
        /// <returns>建立成功回傳 201，並回傳新資料</returns>
        public async Task<IProcessResult> CreateUser([FromBody] UserDto user)
        {
            var result = await _userService.CreateUser(user);
            return result;
        }

        [HttpPut("{id}")]
        [TokenAuthorize]
        /// <summary>
        /// 更新指定使用者資料
        /// </summary>
        /// <param name="id">使用者唯一識別碼</param>
        /// <param name="user">使用者資料物件</param>
        /// <returns>成功回傳 204，失敗回傳 400</returns>
        public async Task<IProcessResult> UpdateUser(Guid id, [FromBody] UserDto user)
        {
            var result2 = await _userService.UpdateUser(user);
            return result2;
        }

        [HttpDelete("{id}")]
        [TokenAuthorize]
        /// <summary>
        /// 刪除指定使用者資料
        /// </summary>
        /// <param name="id">使用者唯一識別碼</param>
        /// <returns>成功回傳 204</returns>
        public async Task<IProcessResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUser(id);
            return result;
        }

        
        [HttpGet("{id}/token")]
        /// <summary>
        /// 取得指定使用者的 JWT Token
        /// </summary>
        /// <param name="id">使用者唯一識別碼</param>
        /// <returns>JWT Token</returns>
        public async Task<ApiResult> GetUserTokenById(Guid id)
        {
            try
            {
                var token = await _userService.GenerateJwtTokenAsync(id);
                return new ApiResult
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Status = "Success",
                    Message = "Token 產生成功",
                    Data = token
                };
            }
            catch (Exception ex)
            {
                return ApiResultFactory.FromException(ex);
            }
        }
    }
}