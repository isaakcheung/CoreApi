using CoreApi.Vuetify.Helpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;

namespace CoreApi.Vuetify.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        [Route("/Login"), Seo()]
        public async Task<IActionResult> Login()
        {
            return View("Views/Login/Index.cshtml");
        }

    }
}