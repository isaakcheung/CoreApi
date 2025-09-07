using System;
using CoreApi.Common.Enums;
using CoreApi.Common.Models;
using CoreApi.Common.Extensions;

namespace CoreApi.Common.Helpers
{
    /// <summary>
    /// 提供依據狀態碼列舉產生標準處理結果物件的工廠方法
    /// </summary>
    public class ProcessResultFactory<TCodeSet> where TCodeSet : Enum
    {
        /// <summary>
        /// 建立成功結果
        /// </summary>
        public static ProcessResult<TCodeSet, TResult> Success<TResult>(TResult data , string? message = null, TCodeSet statusCode = default, int? totalCount = null)
        {
            return new ProcessResult<TCodeSet, TResult>
            {
                IsSuccess = true,
                Status = statusCode.ToString() ?? string.Empty,
                StatusCode = statusCode,
                Message = message ?? statusCode.GetDisplayName() ?? string.Empty,
                Data = data,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 建立成功結果
        /// </summary>
        public static ProcessResult<TCodeSet> Success
        (
            string? message = null,
            TCodeSet statusCode = default,
            int? totalCount = null)
        {
            return new ProcessResult<TCodeSet>
            {
                IsSuccess = true,
                Status = statusCode.ToString() ?? string.Empty,
                StatusCode = statusCode,
                Message = message ?? statusCode.GetDisplayName() ?? string.Empty
            };
        }


        /// <summary>
        /// 建立失敗結果（含資料型別）
        /// </summary>
        public static ProcessResult<TCodeSet, TResult> Fail<TResult>(string? message = null, TCodeSet statusCode = default, Exception? exception = null)
        {
            return new ProcessResult<TCodeSet, TResult>
            {
                IsSuccess = false,
                Status = statusCode.ToString() ?? string.Empty,
                StatusCode = statusCode,
                Message = message ?? statusCode.GetDisplayName() ?? string.Empty,
                Data = default(TResult?),
                Exception = exception
            };
        }


        /// <summary>
        /// 建立失敗結果
        /// </summary>
        public static ProcessResult<TCodeSet> Fail(
            string? message = null,
            TCodeSet statusCode = default,
            Exception? exception = null)
        {
            return new ProcessResult<TCodeSet>
            {
                IsSuccess = false,
                Status = statusCode.ToString() ?? string.Empty,
                StatusCode = statusCode,
                Message = message ?? string.Empty,
                Exception = exception
            };
        }
    }

    /// <summary>
    /// 提供產生通用狀態碼標準處理結果物件的工廠方法
    /// </summary>
    public class GeneralProcessResultFactory
    {
        /// <summary>
        /// 建立成功結果
        /// </summary>
        public static ProcessResult<GeneralResultStatusEnum, TResult> Success<TResult>(TResult data , string? message = null, GeneralResultStatusEnum statusCode = default, int? totalCount = null)
        {
            var finalStatus = statusCode.Equals(default(GeneralResultStatusEnum)) ? GeneralResultStatusEnum.Success : statusCode;
            return new ProcessResult<GeneralResultStatusEnum, TResult>
            {
                IsSuccess = true,
                Status = finalStatus.ToString(),
                StatusCode = finalStatus,
                Message = message ?? finalStatus.GetDisplayName() ?? string.Empty,
                Data = data,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 建立成功結果
        /// </summary>
        public static ProcessResult<GeneralResultStatusEnum> Success
        (
            string? message = null,
            GeneralResultStatusEnum statusCode = default,
            int? totalCount = null)
        {
            var finalStatus = statusCode.Equals(default(GeneralResultStatusEnum)) ? GeneralResultStatusEnum.Success : statusCode;
            return new ProcessResult<GeneralResultStatusEnum>
            {
                IsSuccess = true,
                Status = finalStatus.ToString(),
                StatusCode = finalStatus,
                Message = message ?? finalStatus.GetDisplayName() ?? string.Empty
            };
        }


        /// <summary>
        /// 建立失敗結果（含資料型別）
        /// </summary>
        public static ProcessResult<GeneralResultStatusEnum, TResult> Fail<TResult>(string? message = null, GeneralResultStatusEnum statusCode = default, Exception? exception = null)
        {
            var finalStatus = statusCode.Equals(default(GeneralResultStatusEnum)) ? GeneralResultStatusEnum.Fail : statusCode;
            return new ProcessResult<GeneralResultStatusEnum, TResult>
            {
                IsSuccess = false,
                Status = finalStatus.ToString(),
                StatusCode = finalStatus,
                Message = message ?? finalStatus.GetDisplayName() ?? string.Empty,
                Data = default(TResult?),
                Exception = exception
            };
        }


        /// <summary>
        /// 建立失敗結果
        /// </summary>
        public static ProcessResult<GeneralResultStatusEnum> Fail(
            string? message = null,
            GeneralResultStatusEnum statusCode = default,
            Exception? exception = null)
        {
            var finalStatus = statusCode.Equals(default(GeneralResultStatusEnum)) ? GeneralResultStatusEnum.Fail : statusCode;
            return new ProcessResult<GeneralResultStatusEnum>
            {
                IsSuccess = false,
                Status = finalStatus.ToString(),
                StatusCode = finalStatus,
                Message = message ?? exception?.Message ?? finalStatus.GetDisplayName() ?? string.Empty,
                Exception = exception
            };
        }
    }
}