using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CoreApi.Hangfire.Jobs
{
    /// <summary>
    /// Hangfire 任務：查詢使用者關鍵字
    /// </summary>
    public class GetUserByKeywordJob
    {
        public const string JobId = "UserQueryJob";
        public const string CronExpression = "*/20 * * * *"; // 每 20 分鐘

        private readonly IServiceProvider _serviceProvider;
        private CoreApi.Service.UserService? _userService => _serviceProvider.GetService(typeof(CoreApi.Service.UserService)) as CoreApi.Service.UserService;

        public GetUserByKeywordJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Hangfire 執行入口，呼叫 UserService 查詢
        /// </summary>
        [DisplayName("查詢使用者關鍵字")]
        public async Task ExecuteAsync()
        {
            if (_userService is not null)
            {
                await _userService.GetUserByKeyword(null, null, null);
            }
        }
    }
}