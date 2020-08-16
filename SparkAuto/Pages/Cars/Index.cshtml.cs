using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SparkAuto.Data;
using SparkAuto.Models.ViewModel;

namespace SparkAuto.Pages.Cars
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CarAndCustomerViewModel CarAndCustomerVM { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId = null)
        {
            if (userId == null)
            {
                var clamisIdentity = (ClaimsIdentity)User.Identity;
                //Con el Claim de tipo NameIdentifier se puede obenter el Id de la entidad requerida, en este caso el UserId
                var claim = clamisIdentity.FindFirst(ClaimTypes.NameIdentifier);
                userId = claim.Value;
            }

            CarAndCustomerVM = new CarAndCustomerViewModel()
            {
                Cars = await _context.Cars.Where(c => c.UserId == userId).ToListAsync(),

                UserObj = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == userId)
            };

            return Page();
        }
    }
}
