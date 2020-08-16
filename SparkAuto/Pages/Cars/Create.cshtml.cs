using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SparkAuto.Data;
using SparkAuto.Models;

namespace SparkAuto.Pages.Cars
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly SparkAuto.Data.ApplicationDbContext _context;

        public CreateModel(SparkAuto.Data.ApplicationDbContext context)
        {
            _context = context;
        }


        [BindProperty]
        public Car Car { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public IActionResult OnGet(string userId = null)
        {
            Car = new Car();

            if (userId == null)
            {
                var clamisIdentity = (ClaimsIdentity)User.Identity;
                //Con el Claim de tipo NameIdentifier se puede obenter el Id de la entidad requerida, en este caso el UserId
                var claim = clamisIdentity.FindFirst(ClaimTypes.NameIdentifier);
                userId = claim.Value;
            }
            Car.UserId = userId;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Cars.Add(Car);
            await _context.SaveChangesAsync();
            StatusMessage = "Car has been added successfully";
            return RedirectToPage("Index", new { userId = Car.UserId });
        }
    }
}
