using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using CoreApi.Service;
using CoreApi.Service.Models;
using CoreApi.Repository.Interfaces;
using CoreApi.Common.Interfaces;
using CoreApi.Entity.Entities;
using CoreApi.Common.Models;
using CoreApi.Common.Enums;
using CoreApi.Test.BaseTests;


namespace CoreApi.Test.Services
{
    public class UserServiceTest : BaseDependencyInjectionTest
    {
        // mock 欄位已由 BaseDITest 提供

        public UserServiceTest() : base()
        {
            // 如需額外 mock，於此註冊，勿重複注入 BaseDITest 已註冊的服務
        }

        [Fact]
        public async Task 取得使用者_找到資料()
        {
            var userId = Guid.NewGuid();
            _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((UserEntity?)new UserEntity { Id = userId, Name = "Test" });
            var service = new UserService(_serviceProvider);

            var result = await service.GetUser(userId);

            Xunit.Assert.NotNull(result);
        }

        [Fact]
        public async Task 取得使用者_找不到資料()
        {
            var userId = Guid.NewGuid();
            _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((UserEntity?)null);
            var service = new UserService(_serviceProvider);

            var result = await service.GetUser(userId);

            Xunit.Assert.NotNull(result);
        }

        [Fact]
        public async Task 新增使用者_呼叫AddAsync()
        {
            var userDto = new UserDto { Name = "Test" };
            _userInfoHelperMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
            var service = new UserService(_serviceProvider);

            var result = await service.CreateUser(userDto);

            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<UserEntity>()), Times.Once);
        }
        [Fact]
        public async Task 更新使用者_成功回傳結果()
        {
            var userDto = new UserDto { Id = Guid.NewGuid(), Name = "Update" };
            var originEntity = new UserEntity { Id = userDto.Id.Value, Name = "Origin" };
            _userRepoMock.Setup(r => r.GetByIdAsync(userDto.Id.Value)).ReturnsAsync((UserEntity?)originEntity);
            _userInfoHelperMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
            _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<UserEntity>())).Returns(Task.CompletedTask);
            var service = new UserService(_serviceProvider);

            var result = await service.UpdateUser(userDto);

            Xunit.Assert.NotNull(result);
        }

        [Fact]
        public async Task 更新使用者_找不到資料()
        {
            var userDto = new UserDto { Id = Guid.NewGuid(), Name = "Update" };
            _userRepoMock.Setup(r => r.GetByIdAsync(userDto.Id.Value)).ReturnsAsync((UserEntity?)null);
            var service = new UserService(_serviceProvider);

            var result = await service.UpdateUser(userDto);

            Xunit.Assert.NotNull(result);
        }

        [Fact]
        public async Task 刪除使用者_成功回傳結果()
        {
            var userId = Guid.NewGuid();
            var entity = new UserEntity { Id = userId, Name = "Delete" };
            _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((UserEntity?)entity);
            _userInfoHelperMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
            _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<UserEntity>())).Returns(Task.CompletedTask);
            var service = new UserService(_serviceProvider);

            var result = await service.DeleteUser(userId);

            Xunit.Assert.NotNull(result);
        }

        [Fact]
        public async Task 刪除使用者_找不到資料()
        {
            var userId = Guid.NewGuid();
            _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((UserEntity?)null);
            var service = new UserService(_serviceProvider);

            var result = await service.DeleteUser(userId);

            Xunit.Assert.NotNull(result);
        }

        [Fact]
        public async Task 關鍵字查詢_有結果()
        {
            var keyword = "test";
            var entities = new List<UserEntity> { new UserEntity { Id = Guid.NewGuid(), Name = "test" } };
            var paged = new PageList<UserEntity>(entities, 1);
            _userRepoMock.Setup(r => r.GetUserByKeywordAsync(keyword, It.IsAny<int?>(), It.IsAny<int?>())).ReturnsAsync(paged);
            var service = new UserService(_serviceProvider);

            var result = await service.GetUserByKeyword(keyword);

            Xunit.Assert.NotNull(result);
        }

        [Fact]
        public async Task 關鍵字查詢_無結果()
        {
            var keyword = "none";
            var paged = new PageList<UserEntity>(new List<UserEntity>(), 0);
            _userRepoMock.Setup(r => r.GetUserByKeywordAsync(keyword, It.IsAny<int?>(), It.IsAny<int?>())).ReturnsAsync(paged);
            var service = new UserService(_serviceProvider);

            var result = await service.GetUserByKeyword(keyword);

            Xunit.Assert.NotNull(result);
        }

        [Fact]
        public async Task 產生JwtToken_使用者存在()
        {
            var userId = Guid.NewGuid();
            var entity = new UserEntity { Id = userId, Name = "jwt" };
            _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((UserEntity?)entity);
            _configMock.Setup(c => c["JwtSettings:Issuer"]).Returns("issuer");
            _configMock.Setup(c => c["JwtSettings:Audience"]).Returns("audience");
            _configMock.Setup(c => c["JwtSettings:SignKey"]).Returns("12345678901234567890123456789012");
            var service = new UserService(_serviceProvider);

            var result = await service.GetUserTokenById(userId);
            var processResult = result as ProcessResult<GeneralResultStatusEnum, string>;
            Xunit.Assert.False(string.IsNullOrEmpty(processResult?.Data));
        }

        [Fact]
        public async Task 產生JwtToken_找不到使用者()
        {
            var userId = Guid.NewGuid();
            _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((UserEntity?)null);
            var service = new UserService(_serviceProvider);

            var result = await service.GetUserTokenById(userId);
            var processResult = result as ProcessResult<GeneralResultStatusEnum, string>;
            Xunit.Assert.Null(processResult?.Data);
            Xunit.Assert.Equal(GeneralResultStatusEnum.Fail.ToString(), processResult?.Status);
        }

    }
}