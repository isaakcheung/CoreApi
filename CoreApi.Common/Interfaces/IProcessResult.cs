namespace CoreApi.Common.Interfaces
{
    public interface IProcessResult { }

    public interface IProcessResult<TCodeSet> : IProcessResult where TCodeSet : System.Enum
    {
        string Status { get; set; }
        string Message { get; set; }
        bool? IsSuccess { get; set; }
        TCodeSet StatusCode { get; set; }
        System.Exception? Exception { get; set; }
        Type CodeSetType => typeof(TCodeSet);
    }

    public interface IProcessData<TResult>
    {
        TResult? Data { get; set; }
        int? TotalCount { get; set; }
    }
}