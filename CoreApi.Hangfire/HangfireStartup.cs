using CoreApi.Hangfire.Jobs;
using CoreApi.Service;
using CoreApi.Common.Extensions;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CoreApi.Hangfire
{
    /// <summary>
    /// Hangfire 任務啟動類別
    /// </summary>
    public static class HangfireStartUp
    {
        /// <summary>
        /// 註冊所有 Hangfire 排程任務，並提供環境工廠
        /// </summary>
        /// <param name="app">WebApplication 實例</param>
        public static void StartUp(WebApplication app)
        {
            // 取得 IWebHostEnvironment 實例

            // 可依據 env.EnvironmentName 做環境判斷與初始化
            // 例如：if (env.IsDevelopment()) {...}
            // 註冊 UserService
            RecurringJob.AddOrUpdate<GetUserByKeywordJob>(
                GetUserByKeywordJob.JobId,
                x => x.ExecuteAsync(),
                GetUserByKeywordJob.CronExpression
            );

            var env = app.Services.GetRequiredService<IWebHostEnvironment>();

            if (env.IsLocal())
            {
                StartupForDev();
            }
            if (env.IsDevelop())
            {
                StartupForDev();
            }
            else if (env.IsUat())
            {
                StartupForUat();
            }
            else if (env.IsProd())
            {
                StartupForProd();
            }

        }

        public static void StartupForLocal() { }
        public static void StartupForDev() { }
        public static void StartupForUat() { }
        public static void StartupForProd() { }
        
    }
}