// Copyright (c) RooCode
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CoreApi.Common.Extensions
{
    /// <summary>
    /// Enum 擴充方法，取得 DisplayAttribute 的顯示名稱
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// 取得 Enum 欄位的 Display 名稱，若無則回傳 Enum.ToString()
        /// </summary>
        /// <param name="enumValue">列舉值</param>
        /// <returns>DisplayAttribute Name 或 Enum.ToString()</returns>
        public static string GetDisplayName(this Enum enumValue)
        {
            var member = enumValue.GetType().GetMember(enumValue.ToString());
            if (member.Length > 0)
            {
                var attr = member[0].GetCustomAttribute<DisplayAttribute>();
                if (attr != null && !string.IsNullOrEmpty(attr.Name))
                    return attr.Name!;
            }
            return enumValue.ToString();
        }
    }
}