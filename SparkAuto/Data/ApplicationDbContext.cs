using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SparkAuto.Models;
using SparkAuto.Models.Configurations;

namespace SparkAuto.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ApplicationUserConfiguration());

            base.OnModelCreating(builder);
        }

        public DbSet<ServiceType> ServiceTypes { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Car> Cars { get; set; }
        public DbSet<ServiceShoppingCar> ServiceShoppingCars { get; set; }
        public DbSet<ServiceHeader> ServiceHeaders { get; set; }
        public DbSet<ServiceDetails> ServiceDetails { get; set; }
    }
}
