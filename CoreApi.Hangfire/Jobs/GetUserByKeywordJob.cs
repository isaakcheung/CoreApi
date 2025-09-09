using System;
using System.ComponentModel;
using System.Threading.Tasks;
using CoreApi.Service;

namespace CoreApi.Hangfire.Jobs
{
    /// <summary>
    /// Hangfire 任務：查詢使用者關鍵字
    /// </summary>
    public class GetUserByKeywordJob
    {
        /// <summary>
        /// 任務識別名稱，供 Hangfire 註冊與管理
        /// </summary>
        public const string JobId = "UserQueryJob";

        /// <summary>
        /// Cron 排程表達式，每 20 分鐘執行一次
        /// </summary>
        public const string CronExpression = "*/20 * * * *";

        /// <summary>
        /// 延遲注入服務解析器
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 延遲取得 UserService 實例
        /// </summary>
        private UserService? _userService => _serviceProvider.GetService(typeof(UserService)) as UserService;

        /// <summary>
        /// 建構式，注入 IServiceProvider
        /// </summary>
        /// <param name="serviceProvider">DI 容器服務提供者</param>
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