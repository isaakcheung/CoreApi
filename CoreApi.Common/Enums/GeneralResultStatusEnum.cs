using System.ComponentModel.DataAnnotations;

namespace CoreApi.Common.Enums
{
    /// <summary>
    /// 一般結果狀態列舉
    /// </summary>
    public enum GeneralResultStatusEnum
    {
        [Display(Name = "成功")]
        Success = 1,
        [Display(Name = "失敗")]
        Fail = 2
    }
}