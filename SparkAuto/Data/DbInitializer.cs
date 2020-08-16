using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SparkAuto.Models;
using SparkAuto.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SparkAuto.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Initialize()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (_context.Roles.AnyAsync(x => x.Name == StaticDetails.AdminEndUser).GetAwaiter().GetResult())
            {
                return;
            }

            _roleManager.CreateAsync(new IdentityRole(StaticDetails.AdminEndUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(StaticDetails.CustomerEndUser)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                Name = "Web Master",
                EmailConfirmed = true,
                PhoneNumber = "0996266715",
            }, "Admin123*").GetAwaiter().GetResult();

            _userManager.AddToRoleAsync(_context.Users.FirstOrDefaultAsync(x => x.Email == "admin@gmail.com").GetAwaiter().GetResult(), StaticDetails.AdminEndUser).GetAwaiter().GetResult();
        }
    }
}
