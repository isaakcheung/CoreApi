using CoreApi.Vuetify.Helpers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Html;
using Razor.Templating.Core;
using System.Dynamic;
using System.Net;
using System.Text.Json;

public abstract class SeoConfig
{
    public string Route { get; set; }
    public dynamic SeoData { get; set; }
    public HtmlString SeoHead { get; set; }
    public HtmlString Head { get; set; }
    public HtmlString Body { get; set; }
    public string HeadTemplate { get; set; }
    public string BodyTemplate { get; set; }
    public abstract Task<dynamic> PrepareSeoData(HttpContext Context);
    public abstract Task<HtmlString> PrepareSeoHeadContent(HttpContext Context, SeoAttribute Seo);
    public abstract Task<HtmlString> PrepareHeadContent(HttpContext Context);
    public abstract Task<HtmlString> PrepareBodyContent(HttpContext Context);


}

public class ApiTemplateSeoConfig : SeoConfig
{
    private Func<HttpContext,string, string> mappingUrl { get; set; }
    public string ApiUrl { get; set; }
    private string SeoHeadTemplate { get; set; } = "SeoHead";
    private string SeoHeadContent { get; set; }
    private string HeadContent { get; set; }
    private string BodyContent { get; set; }
    public ApiTemplateSeoConfig(
        string routePath,
        string headTemplate = null,
        string bodyTemplate = null,
        string apiUrl = null,
        Func<HttpContext,string, string> mappingUrl =null
        )
    {
        this.Route = routePath;
        this.HeadTemplate = headTemplate;
        if (string.IsNullOrEmpty(this.HeadTemplate))
            this.HeadTemplate = $"/SeoTemplates/{(routePath.StartsWith("/") ? routePath.Substring(1) : routePath)}/Head.cshtml";
        this.BodyTemplate = bodyTemplate;
        if (string.IsNullOrEmpty(this.BodyTemplate))
            this.BodyTemplate = $"/SeoTemplates/{(routePath.StartsWith("/") ? routePath.Substring(1) : routePath)}/Body.cshtml";
        this.ApiUrl = apiUrl;
        this.mappingUrl = mappingUrl;
        if (string.IsNullOrEmpty(this.ApiUrl) && this.mappingUrl == null) 
            throw new Exception("ApiUrl 或 mappingUrl 必須指定一個");
    }

    public async override Task<dynamic> PrepareSeoData(HttpContext context)
    {
        if (mappingUrl!=null)
            this.ApiUrl = mappingUrl(context,this.ApiUrl);
        try
        {
            this.SeoData = await loadConfig(ApiUrl);
        }
        catch (Exception ex)
        {
            this.SeoData = null;
        }
        return this.SeoData;
    }

    private async Task<dynamic> loadConfig(string configUrl)
    {
        using (var client = new HttpClient())
        {
            var result = await client.GetFromJsonAsync<ExpandoObject>(configUrl);
            return result;
        }
    }

    public async Task<string> GetTemplateAsync<T>(
        string seoTemplateName,
        T model,
        Dictionary<string, object> dictionary = null)
    {
        try
        {
            
            return await RazorTemplateEngine.RenderAsync(
                seoTemplateName,
                model,
                dictionary);

        }
        catch (Exception ex)
        {
            return String.Empty;
        }
    }

    public async override Task<HtmlString> PrepareSeoHeadContent(HttpContext Context,SeoAttribute Seo)
    {
        if (SeoData == null)
            this.SeoHead = new HtmlString("");
        SeoHeadContent = await this.GetTemplateAsync<SeoAttribute>(this.SeoHeadTemplate, Seo);
        this.SeoHead = new HtmlString(SeoHeadContent);
        return this.SeoHead;
    }

    public async override Task<HtmlString> PrepareHeadContent(HttpContext Context)
    {
        if (SeoData == null)
            this.Head = new HtmlString("");
        HeadContent = await this.GetTemplateAsync<dynamic>(this.HeadTemplate, SeoData);
        HeadContent = WebUtility.HtmlDecode(HeadContent);
        this.Head = new HtmlString(HeadContent);
        return this.Head;
    }

    public async override Task<HtmlString> PrepareBodyContent(HttpContext Context)
    {
        if (SeoData == null) 
            this.Body = new HtmlString(""); 
        BodyContent = await this.GetTemplateAsync<dynamic>(this.BodyTemplate, SeoData);
        BodyContent = WebUtility.HtmlDecode(BodyContent);
        this.Body = new HtmlString(BodyContent);
        return this.Body;
    }

}