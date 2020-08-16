using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SparkAuto.Utility;

namespace SparkAuto.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                //Cuando redirecciono a una página que está dentro de un Area folder tengo que mandar como un parametro de ruta el nombre dentro de la carpeta del Area folder a donde quiero acceder
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            if (User.IsInRole(StaticDetails.AdminEndUser))
            {
                return RedirectToPage("/Users/Index");
            }

            return RedirectToPage("/Cars/Index");
        }
    }
}
