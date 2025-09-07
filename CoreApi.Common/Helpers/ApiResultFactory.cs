using System;
using CoreApi.Common.Models;

namespace CoreApi.Common.Helpers
{
    /// <summary>
    /// 提供將 ProcessResult 轉換為 ApiResult 的工廠方法
    /// </summary>
    public static class ApiResultFactory
    {
        /// <summary>
        /// 將 ProcessResult 轉為 ApiResult
        /// </summary>
        /// <summary>
        /// 將任意 ProcessResult 轉為 ApiResult（避免泛型）
        /// </summary>
        public static ApiResult FromResult(object result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            var type = result.GetType();

            var statusCodeProp = type.GetProperty("StatusCode");
            var statusProp = type.GetProperty("Status");
            var messageProp = type.GetProperty("Message");
            var dataProp = type.GetProperty("Data");
            var exceptionProp = type.GetProperty("Exception");
            var totalCountProp = type.GetProperty("TotalCount");
            var isSuccessProp = type.GetProperty("IsSuccess");

            // 預設狀態碼處理
            int statusCode = 0;
            if (statusCodeProp != null)
            {
                var codeVal = statusCodeProp.GetValue(result);
                if (codeVal != null && int.TryParse(codeVal.ToString(), out var parsed))
                {
                    statusCode = parsed;
                }
            }
            bool? isSuccess = isSuccessProp?.GetValue(result) as bool?;
            // 若未指定StatusCode則依IsSuccess給預設值
            if (statusCode == 0)
            {
                var enumType = typeof(CoreApi.Common.Enums.GeneralResultStatusEnum);
                statusCode = isSuccess == true
                    ? (int)Enum.Parse(enumType, "Success")
                    : (int)Enum.Parse(enumType, "Fail");
            }

            var apiResult= new ApiResult
            {
                IsSuccess = isSuccess,
                StatusCode = statusCode,
                Status = statusProp?.GetValue(result)?.ToString() ?? string.Empty,
                Message = messageProp?.GetValue(result)?.ToString() ?? string.Empty,
                Data = dataProp?.GetValue(result),
                Exception = exceptionProp?.GetValue(result) as Exception,
                TotalCount = totalCountProp?.GetValue(result) as int?
            };
            return apiResult;
        }

        /// <summary>
        /// 將 Exception 轉為 ApiResult（自動產生 Fail ProcessResult）
        /// </summary>
        public static ApiResult FromException(Exception ex)
        {
            var failResult = CoreApi.Common.Helpers.GeneralProcessResultFactory.Fail(
                exception: ex ,
                message: ex.Message
            );
            return FromResult(failResult);
        }
    }
}