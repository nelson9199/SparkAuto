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
using SparkAuto.Models.ViewModel;
using SparkAuto.Utility;

namespace SparkAuto.Pages.Services
{
    [Authorize(Roles = StaticDetails.AdminEndUser)]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _contex;

        public CreateModel(ApplicationDbContext contex)
        {
            _contex = contex;
        }

        [BindProperty]
        public CarServiceViewModel CarServiceVM { get; set; }

        public async Task<IActionResult> OnGetAsync(int carId)
        {
            CarServiceVM = new CarServiceViewModel()
            {
                Car = await _contex.Cars.Include(x => x.ApplicationUser).FirstOrDefaultAsync(c => c.Id == carId),
                ServiceHeader = new Models.ServiceHeader()
            };

            List<string> listServiceTypeInShoppingCar = await _contex.ServiceShoppingCars
                .Include(x => x.ServiceType)
                .Where(x => x.CarId == carId)
                .Select(x => x.ServiceType.Name).ToListAsync();

            IQueryable<ServiceType> listServiceType = from s in _contex.ServiceTypes
                                                      where !(listServiceTypeInShoppingCar.Contains(s.Name))
                                                      select s;

            CarServiceVM.ServiceTypes = listServiceType.ToList();

            CarServiceVM.ServiceShoppingCars = await _contex.ServiceShoppingCars
                .Include(x => x.ServiceType)
                .Where(x => x.CarId == carId).ToListAsync();

            CarServiceVM.ServiceHeader.TotalPrice = 0;

            foreach (var item in CarServiceVM.ServiceShoppingCars)
            {
                CarServiceVM.ServiceHeader.TotalPrice += item.ServiceType.Price;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostServiceCompletedAsync()
        {
            if (ModelState.IsValid)
            {
                CarServiceVM.ServiceHeader.DateAdded = DateTime.Now;
                CarServiceVM.ServiceShoppingCars = await _contex.ServiceShoppingCars.Include(x => x.ServiceType).Where(x => x.CarId == CarServiceVM.Car.Id).ToListAsync();

                foreach (var item in CarServiceVM.ServiceShoppingCars)
                {
                    CarServiceVM.ServiceHeader.TotalPrice += item.ServiceType.Price;
                }

                CarServiceVM.ServiceHeader.CarId = CarServiceVM.Car.Id;

                await _contex.ServiceHeaders.AddAsync(CarServiceVM.ServiceHeader);
                await _contex.SaveChangesAsync();

                foreach (var item in CarServiceVM.ServiceShoppingCars)
                {
                    ServiceDetails serviceDetails = new ServiceDetails()
                    {
                        //Cuando añado el serviceHeader en la base de datos se me crea un Id automatico que la propiedad CarServiceVM.ServiceHeader va a sergir persistiendo hasta que se modifique por eso puedo sacar el Id aquí
                        ServiceHeaderId = CarServiceVM.ServiceHeader.Id,
                        ServiceName = item.ServiceType.Name,
                        ServicePrice = item.ServiceType.Price,
                        ServiceTypeId = item.ServiceTypeId
                    };

                    await _contex.ServiceDetails.AddAsync(serviceDetails);
                }

                _contex.ServiceShoppingCars.RemoveRange(CarServiceVM.ServiceShoppingCars);

                await _contex.SaveChangesAsync();

                return RedirectToPage("../Cars/Index", new { userId = CarServiceVM.Car.UserId });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddToCartAsync()
        {
            ServiceShoppingCar objServiceCart = new ServiceShoppingCar()
            {
                CarId = CarServiceVM.Car.Id,
                ServiceTypeId = CarServiceVM.ServiceDetails.ServiceTypeId,
            };

            await _contex.ServiceShoppingCars.AddAsync(objServiceCart);
            await _contex.SaveChangesAsync();

            return RedirectToPage("Create", new { carId = CarServiceVM.Car.Id });
        }

        public async Task<IActionResult> OnPostRemoveFromCartAsync(int serviceTypeId)
        {
            ServiceShoppingCar objServiceCart = await _contex.ServiceShoppingCars
                .FirstOrDefaultAsync(x => x.CarId == CarServiceVM.Car.Id && x.ServiceTypeId == serviceTypeId);

            _contex.ServiceShoppingCars.Remove(objServiceCart);
            await _contex.SaveChangesAsync();

            return RedirectToPage("Create", new { carId = CarServiceVM.Car.Id });
        }
    }
}
