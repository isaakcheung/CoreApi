using System;

namespace CoreApi.Common.Models
{
    /// <summary>
    /// 標準 API 回傳格式
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool? IsSuccess { get; set; }

        /// <summary>
        /// 狀態碼
        /// </summary>
        public int StatusCode { get; set; }
        /// <summary>
        /// 狀態描述
        /// </summary>
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// 訊息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 回傳資料
        /// </summary>
        public object? Data { get; set; }
        /// <summary>
        /// 例外資訊
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public Exception? Exception { get; set; }

        /// <summary>
        /// 總筆數
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull)]
        public int? TotalCount { get; set; }

        /// <summary>
        /// 由 Exception 產生 ApiResult
        /// </summary>

        /// <summary>
        /// 由 IProcessResult 產生 ApiResult
        /// </summary>
    }
}