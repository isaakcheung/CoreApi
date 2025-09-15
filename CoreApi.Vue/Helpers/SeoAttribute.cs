using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Filters;
using Razor.Templating.Core;
using System.Dynamic;

namespace CoreApi.Vuetify.Helpers
{
    public class SeoAttribute : Attribute, IActionFilter
    {
        public bool IsScraper { 
            get {
                var isScraper = false;
                var userAgent = Context.HttpContext.Request.Headers.UserAgent.ToString().ToLower();
                foreach (var identify in SeoConfigs.ScraperIdentify)
                {
                    if (isScraper) continue;
                    isScraper = userAgent.Contains(identify);   
                }
                return isScraper;
            } 
        }
        public ActionExecutingContext Context { get; set; }
        public HtmlString SeoHead
        {
            get
            {
                if (Config != null) return Config.SeoHead;
                return new HtmlString("");
            }
        }

        public HtmlString Head { 
            get { 
                if (Config!=null) return Config.Head;
                return new HtmlString("");
            } 
        }
        public HtmlString Body
        {
            get
            {
                if (Config != null) return Config.Body;
                return new HtmlString("");
            }
        }
        public SeoConfig Config { get; set; }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            this.Context = context;
            Config = SeoConfigs.Configs.FirstOrDefault(c=>c.Route.ToLower()==$"/{context.ActionDescriptor.AttributeRouteInfo.Template}".ToLower());
            if (Config == null) return;
            
            Config.PrepareSeoData(context.HttpContext).Wait();
            Config.PrepareHeadContent(context.HttpContext);
            Config.PrepareBodyContent(context.HttpContext);
            Config.PrepareSeoHeadContent(context.HttpContext,this);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public static SeoAttribute GetSEOFilter(IList<FilterDescriptor> Filters) 
        {

            var seoAttribute = default(SeoAttribute);
            try
            {
                seoAttribute = (SeoAttribute)Filters.FirstOrDefault(c => c.Filter is SeoAttribute)?.Filter;
                return seoAttribute;
            }
            catch
            {
                return null;
            }
        }

    }
}
