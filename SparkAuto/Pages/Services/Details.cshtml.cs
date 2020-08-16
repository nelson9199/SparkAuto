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
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public ServiceHeader ServiceHeader { get; set; }
        public List<ServiceDetails> ServiceDetails { get; set; }

        public async Task OnGetAsync(int serviceId)
        {
            ServiceHeader = await _context.ServiceHeaders
                .Include(x => x.Car)
                .ThenInclude(x => x.ApplicationUser)
                .FirstOrDefaultAsync(x => x.Id == serviceId);

            ServiceDetails = await _context.ServiceDetails.Where(x => x.ServiceHeaderId == serviceId).ToListAsync();
        }
    }
}
