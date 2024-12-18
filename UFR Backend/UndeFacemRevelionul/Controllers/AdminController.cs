using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UndeFacemRevelionul.ContextModels;

namespace UndeFacemRevelionul.Controllers
{
    public class AdminController : Controller
    {
        private readonly RevelionContext _context;

        public AdminController(RevelionContext context)
        {
            _context = context;
        }

        // Metoda pentru dashboard
        public IActionResult Dashboard()
        {
            return View();
        }

        // Metode pentru liste
        public IActionResult PartiersList()
        {
            var partiers = _context.Users
                .Where(u => u.UserRole == "Partier") 
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.BlockedUntil
                })
                .ToList();

            return View(partiers);
        }


        public IActionResult ProvidersList()
        {
            var providers = _context.Users
                .Where(u => u.UserRole == "Provider")
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email
                })
                .ToList();

            return View(providers);
        }

        public IActionResult PartiesList()
        {
            var parties = _context.Parties.ToList();
            return View(parties);
        }

        public IActionResult LocationList ()
        {
            var locations = _context.Locations.ToList();
            return View(locations);
        }

        public IActionResult MenusList()
        {
            var menus = _context.FoodMenus.ToList();
            return View(menus);
        }

        [HttpGet]
        public IActionResult BlockPartier(int id, int hours)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                user.BlockedUntil = DateTime.Now.AddHours(hours);
                _context.SaveChanges();
            }
            return RedirectToAction("PartiersList");
        }

        public IActionResult UnblockPartier(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                user.BlockedUntil = null; // Resetează starea de blocare
                _context.SaveChanges();
            }
            return RedirectToAction("PartiersList");
        }


        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            // Caută utilizatorul în baza de date
            var user = _context.Users.FirstOrDefault(u => u.Id == id);

            if (user != null)
            {
                if(user.UserRole == "Provider")
                {
                    // Șterge providerul din baza de date
                    var provider = _context.Providers.FirstOrDefault(p => p.UserId == id);
                    var locations = _context.Locations.Where(s => s.ProviderId == provider.Id).ToList();
                    var menus = _context.FoodMenus.Where(s => s.ProviderId == provider.Id).ToList();
                    _context.Providers.Remove(provider);
                    _context.Locations.RemoveRange(locations);
                    _context.FoodMenus.RemoveRange(menus);
                }
                else if (user.UserRole == "Partier")
                {
                    // Șterge petrecărețul din baza de date
                    var partier = _context.Partiers.FirstOrDefault(p => p.UserId == id);
                    _context.Partiers.Remove(partier);
                }
                
                _context.Users.Remove(user);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Utilizatorul a fost șters cu succes!";
            }
            else
            {
                TempData["ErrorMessage"] = "Utilizatorul nu a fost găsit!";
            }

            // Redirecționează înapoi la lista de petrecăreți
            return RedirectToAction("PartiersList");
        }


    }

}
