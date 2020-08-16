using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SparkAuto.Data;
using SparkAuto.Models;
using SparkAuto.Models.ViewModel;
using SparkAuto.Utility;

namespace SparkAuto.Pages.Users
{
    [Authorize(Roles = StaticDetails.AdminEndUser)]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UsersListViewModel UsersListViewModel { get; set; }

        public async Task<IActionResult> OnGetAsync(int productPage = 1, string searchEmail = null, string searchName = null, string searchPhone = null)
        {
            UsersListViewModel = new UsersListViewModel
            {
                ApplicationUsers = await _context.ApplicationUsers.ToListAsync()
            };

            StringBuilder param = new StringBuilder();
            param.Append("/Users?productPage=:");
            param.Append("&searchName=");

            if (searchName != null)
            {
                param.Append(searchName);
            }
            param.Append("&searchEmail=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }
            param.Append("&searchPhone");
            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }

            if (searchEmail != null)
            {
                UsersListViewModel.ApplicationUsers = await _context.ApplicationUsers.Where(x => x.Email.ToLower().Contains(searchEmail.ToLower())).ToListAsync();
            }
            if (searchName != null)
            {
                UsersListViewModel.ApplicationUsers = await _context.ApplicationUsers.Where(x => x.Name.ToLower().Contains(searchName.ToLower())).ToListAsync();
            }

            if (searchPhone != null)
            {
                UsersListViewModel.ApplicationUsers = await _context.ApplicationUsers.Where(x => x.PhoneNumber.ToLower().Contains(searchPhone.ToLower())).ToListAsync();
            }

            var count = UsersListViewModel.ApplicationUsers.Count;

            UsersListViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = StaticDetails.PaginationUserPageSize,
                TotalItems = count,
                UrlParam = param.ToString()
            };

            UsersListViewModel.ApplicationUsers = UsersListViewModel.ApplicationUsers.OrderBy(p => p.Email)
                    .Skip((productPage - 1) * StaticDetails.PaginationUserPageSize)
                    .Take(2).ToList();

            return Page();
        }
    }
}
