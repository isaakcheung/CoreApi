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
        /// <returns>
        /// 回傳標準 ApiResult，Data 欄位為 <see cref="UserDto"/>，包含指定使用者詳細資料。
        /// </returns>
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
        /// <returns>
        /// 回傳標準 ApiResult，Data 欄位為 <see cref="List{UserDto}"/>，包含符合關鍵字的使用者清單。
        /// </returns>
        public async Task<IProcessResult> GetUserByKeyword(
            [FromQuery] string? keyword = null,
            [FromQuery] int? skip = null,
            [FromQuery] int? take = null)
        {
            var result = await _userService.GetUserByKeyword(keyword, skip, take);
            return result;
        }

        [HttpPost]
        /// <summary>
        /// 新增使用者資料
        /// </summary>
        /// <param name="user">使用者資料物件</param>
        /// <returns>
        /// 回傳標準 ApiResult，Data 欄位為 <see cref="UserDto"/>，包含新增後的使用者資料。
        /// </returns>
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
        /// <returns>
        /// 回傳標準 ApiResult，Data 欄位為 <see cref="UserDto"/>，包含更新後的使用者資料。
        /// </returns>
        public async Task<IProcessResult> UpdateUser(Guid id, [FromBody] UserDto user)
        {
            var result = await _userService.UpdateUser(user);
            return result;
        }

        [HttpDelete("{id}")]
        [TokenAuthorize]
        /// <summary>
        /// 刪除指定使用者資料
        /// </summary>
        /// <param name="id">使用者唯一識別碼</param>
        /// <returns>
        /// 回傳標準 ApiResult，Data 欄位為 <see cref="bool"/>，true 表示刪除成功。
        /// </returns>
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
        /// <returns>
        /// 回傳標準 ApiResult，Data 欄位為 JWT Token 字串，代表指定使用者的存取權杖。
        /// </returns>
        public async Task<IProcessResult> GetUserTokenById(Guid id)
        {
            return await _userService.GetUserTokenById(id);
        }
    }
}