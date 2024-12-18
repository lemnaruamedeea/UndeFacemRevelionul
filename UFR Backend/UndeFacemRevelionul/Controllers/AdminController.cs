using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UndeFacemRevelionul.ContextModels;

namespace UndeFacemRevelionul.Controllers
{
    public class AdminController : Controller
    {
        private readonly RevelionContext _context;
        private readonly ILogger<AccountController> _logger;

        public AdminController(RevelionContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
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

            //var currentUserId = GetCurrentUserId();
            if (user != null)
            {
                if (user.UserRole == "Provider")
                {
                    var provider = _context.Providers.FirstOrDefault(p => p.UserId == id);
                    if (provider != null)
                    {
                        var menus = _context.FoodMenus.Where(s => s.ProviderId == provider.Id).ToList();
                        foreach (var menu in menus)
                        {
                            var parties = _context.Parties.Where(p => p.FoodMenuId == menu.Id).ToList();
                            foreach (var party in parties)
                            {
                                party.FoodMenuId = null;  // Or set it to another valid FoodMenuId
                                _context.Parties.Update(party);
                            }
                        }
                        var locations = _context.Locations.Where(s => s.ProviderId == provider.Id).ToList();
                        foreach (var loc in locations)
                        {
                            var parties = _context.Parties.Where(p => p.LocationId == loc.Id).ToList();
                            foreach (var party in parties)
                            {
                                party.LocationId = null;  // Or set it to another valid FoodMenuId
                                _context.Parties.Update(party);
                            }
                        }

                        _context.FoodMenus.RemoveRange(menus);
                        _context.Locations.RemoveRange(locations);
                        _context.Providers.Remove(provider);
                    }
                }


                else if (user.UserRole == "Partier")
                {
                    var partier = _context.Partiers.FirstOrDefault(p => p.UserId == user.Id);
                    if (partier != null)
                    {
                        _context.Partiers.Remove(partier);
                    }
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
            return RedirectToAction("Dashboard");
        }


        private int GetCurrentUserId()
        {
            // Verifică dacă utilizatorul este autentificat
            if (!User.Identity.IsAuthenticated)
            {
                _logger.LogError("User is not authenticated.");
                throw new InvalidOperationException("User is not authenticated.");
            }

            // Adaugă log pentru a verifica ce claim-uri sunt disponibile
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            foreach (var claim in claims)
            {
                _logger.LogInformation($"Claim Type: {claim.Type}, Value: {claim.Value}");
            }

            // Căutăm Claim-ul pentru NameIdentifier
            var userIdClaim = User.FindFirst("UserId");


            if (userIdClaim == null)
            {
                _logger.LogError("User does not have a valid ID claim.");
                throw new InvalidOperationException("User does not have a valid ID claim.");
            }

            // Returnează ID-ul utilizatorului
            return int.Parse(userIdClaim.Value);
        }

    }



}
