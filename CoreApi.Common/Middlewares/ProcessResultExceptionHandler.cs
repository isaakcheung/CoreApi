using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using CoreApi.Common.Models;
using CoreApi.Common.Helpers;

namespace CoreApi.Common.Middlewares
{
    /// <summary>
    /// 全域例外處理中介層
    /// </summary>
    public class ProcessResultExceptionMiddlewares
    {
        private readonly RequestDelegate _next;

        public ProcessResultExceptionMiddlewares(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);                
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var result = ApiResultFactory.FromException(ex);

                var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(json);
            }
        }
    }
}