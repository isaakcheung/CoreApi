// Copyright (c) RooCode
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CoreApi.Common.Interfaces;
using CoreApi.Common.Models;

namespace CoreApi.Common.Helpers
{
    /// <summary>
    /// 將 IProcessResult 統一轉換為 ApiResult 的過濾器
    /// </summary>
    public class ProcessResultFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult objectResult && objectResult.Value is IProcessResult processResult)
            {
                string? status = null;
                string? message = null;
                object? data = null;

                // 嘗試轉型取得屬性
                var type = processResult.GetType();
                var statusProp = type.GetProperty("Status");
                var messageProp = type.GetProperty("Message");
                var dataProp = type.GetProperty("Data");

                if (statusProp != null)
                    status = statusProp.GetValue(processResult)?.ToString();
                if (messageProp != null)
                    message = messageProp.GetValue(processResult)?.ToString();
                if (dataProp != null)
                    data = dataProp.GetValue(processResult);

                var apiResult = ApiResultFactory.FromResult(processResult);
                context.Result = new ObjectResult(apiResult)
                {
                    StatusCode = objectResult.StatusCode
                };
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            // 不需處理
        }
    }
}