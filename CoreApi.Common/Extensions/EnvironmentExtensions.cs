using Microsoft.AspNetCore.Hosting;

namespace CoreApi.Common.Extensions
{
    public static class EnvironmentExtensions
    {
        public static bool IsLocal(this IWebHostEnvironment env)
        {
            return env.EnvironmentName?.Equals("Local", System.StringComparison.OrdinalIgnoreCase) == true;
        }

        public static bool IsDevelop(this IWebHostEnvironment env)
        {
            return env.EnvironmentName?.Equals("Dev", System.StringComparison.OrdinalIgnoreCase) == true;
        }

        public static bool IsUat(this IWebHostEnvironment env)
        {
            return env.EnvironmentName?.Equals("UAT", System.StringComparison.OrdinalIgnoreCase) == true;
        }

        public static bool IsProd(this IWebHostEnvironment env)
        {
            return env.EnvironmentName?.Equals("Prod", System.StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}