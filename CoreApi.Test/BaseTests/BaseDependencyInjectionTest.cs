using System;
using Microsoft.Extensions.Logging;
using Moq;
using CoreApi.Service;
using Microsoft.Extensions.DependencyInjection;
using CoreApi.Service.Interfaces;
using CoreApi.Repository.Interfaces;
using CoreApi.Common.Interfaces;
using CoreApi.Entity.Entities;
using Microsoft.Extensions.Configuration;

namespace CoreApi.Test.BaseTests
{
    /// <summary>
    /// DI 測試基底，提供 IServiceProvider 與 Mock 註冊支援
    /// </summary>
    public abstract class BaseDependencyInjectionTest
    {
        protected readonly ServiceCollection _service;
        protected IServiceProvider _serviceProvider => _service.BuildServiceProvider();

        // 所有 mock 服務 protected readonly 欄位
        protected readonly Mock<IUserService> _userServiceMock = new();
        protected readonly Mock<IUserRepository> _userRepoMock = new();
        protected readonly Mock<IUserInfoHelper> _userInfoHelperMock = new();
        protected readonly Mock<IProcessResult> _processResultMock = new();
        protected readonly Mock<ReadWriteCoreApiDbContext> _rwDbContextMock = new();
        protected readonly Mock<ReadOnlyCoreApiDbContext> _roDbContextMock = new();
        protected readonly Mock<IConfiguration> _configMock = new();
        protected readonly Mock<ILogger<UserService>> _userLoggerMock = new();

        protected BaseDependencyInjectionTest()
        {
            _service = new ServiceCollection();
            // 於建構式注入所有 mock 物件
            RegisterDefaultMocks(_service);
        }

        /// <summary>
        /// 註冊預設 Mock，供 API/Hangfire 測試覆寫
        /// </summary>
        /// <param name="services">ServiceCollection</param>
        protected virtual void RegisterDefaultMocks(ServiceCollection service)
        {
            // 預設 mock 物件註冊
            service.AddSingleton(_userServiceMock.Object);
            service.AddSingleton(_userRepoMock.Object);
            service.AddSingleton(_userInfoHelperMock.Object);
            service.AddSingleton(_processResultMock.Object);
            service.AddSingleton(_rwDbContextMock.Object);
            service.AddSingleton(_roDbContextMock.Object);
            service.AddSingleton(_configMock.Object);
            service.AddSingleton(_userLoggerMock.Object);
            // 其他註冊型別請依需求補齊
            // 注入 ILogger<UserService> mock，避免 UserService 測試時 null 例外
            // 已於欄位宣告與註冊 _userLoggerMock，無需重複建立 userLoggerMock 變數
        }

        /// <summary>
        /// 取得指定型別的 Mock 實例
        /// </summary>
        /// <typeparam name="T">型別</typeparam>
        /// <returns>Mock<T></returns>
        protected Mock<T> GetMock<T>() where T : class
        {
            return Mock.Get(_serviceProvider.GetRequiredService<T>());
        }
    }
}