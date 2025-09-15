using Microsoft.AspNetCore.Html;
using System.Dynamic;

public static class SeoConfigs
{
    public static List<string> ScraperIdentify { get; set; } = new List<string>() {
        "google","facebook"
    };
    public static List<SeoConfig> Configs { get; set; } = new List<SeoConfig>() {

        new ApiTemplateSeoConfig(
            routePath : "/Login",  //route path 
            headTemplate : "Login/Head", // seo header template
            bodyTemplate: "Login/Body", // seo body template
            apiUrl:"https://localhost:7126/seodata/seo_login.json" //api url
            ),
        new ApiTemplateSeoConfig(
            routePath:"/Products",  // route path
            apiUrl:"https://localhost:7126/seodata/seo_products.json"  //api url
            ),
        new ApiTemplateSeoConfig(
            routePath:"/Products/{id}", // route path
            mappingUrl:(context,apiUrl)=>{
                return $"https://localhost:7126/seodata/seo_products_{context.GetRouteValue("id")}.json";
            }) //mapping query
    };
}