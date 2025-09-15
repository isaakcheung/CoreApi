using CoreApi.Vuetify.Helpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;

namespace CoreApi.Vuetify.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ILogger<ProductsController> logger)
        {
            _logger = logger;
        }

        [Route("/Products/{id}")]
        [Route("/Products"), Seo()]
        public async Task<IActionResult> Products()
        {
            return View("Views/Products/Index.cshtml");
        }

    }
}