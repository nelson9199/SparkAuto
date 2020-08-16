using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SparkAuto.Data;
using SparkAuto.Models;
using SparkAuto.Utility;

namespace SparkAuto.Pages.Users
{
    [Authorize(Roles = StaticDetails.AdminEndUser)]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext context;

        public EditModel(ApplicationDbContext context)
        {
            this.context = context;
        }

        [BindProperty]
        public ApplicationUser ApplicationUser { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationUser = await context.ApplicationUsers.FindAsync(id);

            if (ApplicationUser == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var UserDb = await context.ApplicationUsers.FindAsync(ApplicationUser.Id);

            UserDb.Name = ApplicationUser.Name;
            UserDb.PhoneNumber = ApplicationUser.PhoneNumber;
            UserDb.Addres = ApplicationUser.Addres;
            UserDb.City = ApplicationUser.City;
            UserDb.PostalCode = ApplicationUser.PostalCode;

            await context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}