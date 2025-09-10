using CoreApi.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using CoreApi.Service;
using CoreApi.Common.Helpers;
using CoreApi.Common.Interfaces;
using CoreApi.Entity.Entities;

namespace CoreApi.Service.Helper
{
    /// <summary>
    /// Service DI 註冊靜態啟動類別
    /// </summary>
    public static class ServiceStartUp
    {
        /// <summary>
        /// 註冊 Service 層與 API 相關 DI
        /// </summary>
        public static void StartUp(IServiceCollection services, IConfiguration configuration)
        {
            // Service
            services.AddScoped<IUserService, UserService>();

            // Repository/Helper
            services.AddScoped<CoreApi.Repository.Interfaces.IUserRepository, CoreApi.Repository.Repositories.UserRepository>();
            services.AddSingleton<SqlConnectionHelper>();
            services.AddScoped<IUserInfoHelper, UserInfoHelper>();

            // DbContext
            services.AddDbContext<CoreApi.Entity.Entities.ReadWriteCoreApiDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString(CoreApi.Common.Constants.DbConnectionKeys.ReadWriteConnection)));
            services.AddDbContext<CoreApi.Entity.Entities.ReadOnlyCoreApiDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString(CoreApi.Common.Constants.DbConnectionKeys.ReadOnlyConnection)));

            // API
            services.AddHttpContextAccessor();
            services.AddControllers(options =>
            {
                options.Filters.Add<CoreApi.Common.Helpers.ProcessResultFilter>();
            });
        }
    }
}