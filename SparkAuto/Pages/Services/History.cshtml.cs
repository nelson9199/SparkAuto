using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SparkAuto.Data;
using SparkAuto.Models;

namespace SparkAuto.Pages.Services
{
    [Authorize]
    public class HistoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public HistoryModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List<ServiceHeader> ServiceHeaders { get; set; }

        public string UserId { get; set; }

        public async Task OnGetAsync(int carId)
        {
            ServiceHeaders = await _context.ServiceHeaders.Include(x => x.Car).ThenInclude(x => x.ApplicationUser)
                .Where(x => x.CarId == carId).ToListAsync();

            UserId = _context.Cars.Where(x => x.Id == carId).FirstOrDefaultAsync().Result.UserId;
        }
    }
}
