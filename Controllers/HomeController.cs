using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using dotnet_mac_sample.Models;

namespace dotnet_mac_sample.Controllers
{
    public class HomeController : Controller
    {
        private readonly CoffeeContext _context;

        private DateTime CreatedAt = DateTime.Now;

        public HomeController(CoffeeContext dbContext)
        {
            this._context = dbContext;

            if (this._context.CoffeeShops.Count() == 0)
            {
                this._context.CoffeeShops.Add(new CoffeeShop
                {
                    Address = "123 Fake Street",
                    Rating = 5,
                    Name = "Billy's Coffee"
                });
                this._context.SaveChanges();
            }
        }
        public IActionResult Index()
        {
            ViewData["Shops"] = this._context.CoffeeShops.ToList();

            return View(this._context.CoffeeShops.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> Add(CoffeeShop newShop)
        {
            if (newShop != null){
                this._context.Add(newShop);
                await this._context.SaveChangesAsync();
                return this.RedirectToAction(nameof(this.Index));
            } else {
                return this.RedirectToAction(nameof(this.Index));
            }
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
