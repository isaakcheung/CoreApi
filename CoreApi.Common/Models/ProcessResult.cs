using System;
using CoreApi.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace CoreApi.Common.Models
{
    // interface 已移至 CoreApi.Common.Interfaces.IProcessResult

    /// <summary>
    /// 標準 API 回應物件
    /// </summary>
    public class ProcessResult<TCodeSet, TResult> : ProcessResult<TCodeSet>, CoreApi.Common.Interfaces.IProcessData<TResult> where TCodeSet : Enum
    {
        public TResult? Data { get; set; } = default;
        public int? TotalCount { get; set; }
        public ProcessResult() : base() { }
    }

    /// <summary>
    /// 標準 API 回應物件
    /// </summary>
    public class ProcessResult<TCodeSet> : CoreApi.Common.Interfaces.IProcessResult<TCodeSet> where TCodeSet : Enum
    {
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool? IsSuccess { get; set; }
        public TCodeSet StatusCode { get; set; } = default!;
        [System.Text.Json.Serialization.JsonIgnore]
        public Exception? Exception { get; set; }
        public ProcessResult() { }
    }
}