using System.Collections.Generic;

namespace CoreApi.Common.Models
{
    /// <summary>
    /// 分頁回應物件，繼承 List<TResult>
    /// </summary>
    public class PageList<TResult> : List<TResult>
    {
        public int TotalCount { get; set; }

        public PageList() { }

        public PageList(IEnumerable<TResult> items, int count)
            : base(items)
        {
            TotalCount = count;
        }
    }
}