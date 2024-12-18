using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UndeFacemRevelionul.ContextModels;
using UndeFacemRevelionul.Models;
using UndeFacemRevelionul.ViewModels;

namespace UndeFacemRevelionul.Controllers
{
    public class ProviderController : Controller
    {
        private readonly RevelionContext _context;
        private readonly ILogger<ProviderController> _logger;

        public ProviderController(RevelionContext context, ILogger<ProviderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private int GetCurrentUserId()
        {
            if (!User.Identity.IsAuthenticated)
            {
                throw new InvalidOperationException("User is not authenticated.");
            }

            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                throw new InvalidOperationException("User does not have a valid ID claim.");
            }

            return int.Parse(userIdClaim.Value);
        }

        // DASHBOARD
        [HttpGet]
        public IActionResult Dashboard()
        {
            var currentUserId = GetCurrentUserId();
            var provider = _context.Providers.FirstOrDefault(p => p.UserId == currentUserId);

            if (provider == null)
            {
                return NotFound("Provider not found.");
            }

            ViewBag.ProviderId = provider.Id;
            ViewBag.Locations = _context.Locations.Where(l => l.ProviderId == provider.Id).ToList();
            ViewBag.Menus = _context.FoodMenus.Where(m => m.ProviderId == provider.Id).ToList();

            return View(provider);
        }

        // ADD LOCATION
        [HttpGet]
        public IActionResult AddLocation()
        {
            var currentUserId = GetCurrentUserId();
            var provider = _context.Providers.FirstOrDefault(p => p.UserId == currentUserId);

            if (provider == null)
                return NotFound("Provider not found.");

            ViewBag.ProviderId = provider.Id;
            return View();
        }

        [HttpPost]
        public IActionResult AddLocation(AddLocationViewModel location)
        {

            if (ModelState.IsValid)
            {
                //var providerId = GetCurrentUserId();
                var providerId = _context.Providers.Where(p => p.UserId == GetCurrentUserId()).Select(nume => nume.Id).FirstOrDefault();
                //location.ProviderId = providerId;
                _context.Locations.Add(new LocationModel
                {
                    Name = location.Name,
                    Address = location.Address,
                    Description = location.Description,
                    ProviderId = providerId,
                    Price = location.Price,
                    Capacity = location.Capacity,
                    Rating = location.Rating,
                    Date = DateTime.Now,
                  
                });
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }

            return View(location);
        }

        // EDIT LOCATION
        [HttpGet]
        public IActionResult EditLocation(int id)
        {
            // Căutăm locația după ID
            var location = _context.Locations.Find(id);
            if (location == null)
                return NotFound();

            // Creăm și trimitem un EditLocationViewModel la view
            var viewModel = new EditLocationViewModel
            {
                Id = location.Id,
                Name = location.Name,
                Address = location.Address,
                Price = (decimal)location.Price,
                Capacity = location.Capacity,
                Description = location.Description,
                Rating = location.Rating,
                Date = location.Date
            };

            return View(viewModel); // Trimiterea modelului la view
        }

        [HttpPost]
        public IActionResult EditLocation(EditLocationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Dacă modelul nu este valid, rămânem pe aceeași pagină
            }

            // Căutăm locația în baza de date folosind ID-ul
            var location = _context.Locations.Find(model.Id);
            if (location == null)
            {
                return NotFound(); // Dacă locația nu a fost găsită
            }

            // Actualizăm locația cu noile date
            location.Name = model.Name;
            location.Address = model.Address;
            location.Price = (float)model.Price;
            location.Capacity = model.Capacity;
            location.Description = model.Description;
            location.Rating = (float)model.Rating;
            location.Date = model.Date;

            // Salvăm modificările în baza de date
            _context.SaveChanges();

            // Redirecționăm către pagina Dashboard
            return RedirectToAction("Dashboard");
        }

        // DELETE LOCATION
        [HttpPost]
        public IActionResult DeleteLocation(int id)
        {
            var location = _context.Locations.Find(id);
            if (location == null)
                return NotFound();

            _context.Locations.Remove(location);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        // ADD MENU
        // ADD MENU
        [HttpGet]
        public IActionResult AddMenu()
        {
            var currentUserId = GetCurrentUserId();
            var provider = _context.Providers.FirstOrDefault(p => p.UserId == currentUserId);

            if (provider == null)
                return NotFound("Provider not found.");

            ViewBag.ProviderId = provider.Id;
            return View();
        }

        [HttpPost]
        public IActionResult AddMenu(AddMenuViewModel menu)
        {
            if (ModelState.IsValid)
            {
                //var providerId = GetCurrentUserId();
                var providerId = _context.Providers.Where(p => p.UserId == GetCurrentUserId()).Select(nume => nume.Id).FirstOrDefault();
                _context.FoodMenus.Add(new FoodMenuModel
                {

                    Name = menu.Name,
                    ProviderId = providerId,
                    Price = menu.Price,
                    Description = menu.Description,
                    Rating = menu.Rating,
                    MenuFilePath = "n/a"
                });
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }

            return View(menu);
        }

        // EDIT MENU
        [HttpGet]
        public IActionResult EditMenu(int id)
        {
            var menu = _context.FoodMenus.Find(id);
            if (menu == null)
                return NotFound();

            // Convertește FoodMenuModel în EditMenuViewModel
            var viewModel = new EditMenuViewModel
            {
                Id = menu.Id,
                ProviderId = menu.ProviderId,
                Name = menu.Name,
                Description = menu.Description,
                Price = (decimal)menu.Price, // Aici facem conversia la decimal
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditMenu(int id, EditMenuViewModel updatedMenu)
        {
            if (id != updatedMenu.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                // Căutăm meniul în baza de date
                var menu = _context.FoodMenus.Find(id);
                if (menu == null)
                    return NotFound();

                // Actualizăm meniul cu datele din EditMenuViewModel
                menu.Name = updatedMenu.Name;
                menu.Description = updatedMenu.Description;
                menu.Price = (float)updatedMenu.Price; // Conversia la float
                

                _context.Entry(menu).State = EntityState.Modified;
                _context.SaveChanges();

                return RedirectToAction("Dashboard");
            }

            return View(updatedMenu);
        }

        // DELETE MENU
        [HttpPost]
        public IActionResult DeleteMenu(int id)
        {
            var menu = _context.FoodMenus.Find(id);
            if (menu == null)
                return NotFound();

            _context.FoodMenus.Remove(menu);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        // EDIT PROVIDER INFO
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var provider = _context.Providers.Find(id);
            if (provider == null)
                return NotFound();

            return View(provider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ProviderModel provider)
        {
            if (id != provider.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Entry(provider).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }

            return View(provider);
        }

        // DELETE PROVIDER
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var provider = _context.Providers.Find(id);
            if (provider == null)
                return NotFound();

            _context.Providers.Remove(provider);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
    }
}
