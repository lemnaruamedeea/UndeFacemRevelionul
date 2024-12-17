using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UndeFacemRevelionul.ContextModels;
using UndeFacemRevelionul.Models;

namespace UndeFacemRevelionul.Controllers;



public class ProviderController : Controller
{


    private readonly RevelionContext _context;
    private readonly ILogger<AccountController> _logger;

    public ProviderController(RevelionContext context, ILogger<AccountController> logger)
    {
        _context = context;
        _logger = logger;
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

    public IActionResult Dashboard()
    {
        var currentUserId = GetCurrentUserId();  // Obține ID-ul utilizatorului logat

        // Căutăm provider-ul asociat utilizatorului logat
        var provider = _context.Providers.FirstOrDefault(p => p.UserId == currentUserId);

        if (provider == null)
        {
            // Dacă nu există provider pentru utilizator, redirecționează sau afișează un mesaj
            Console.WriteLine("Nu avem provider :(");
        }

        // Obține locațiile și meniurile asociate provider-ului
        var locations = _context.Locations.Where(l => l.ProviderId == provider.Id).ToList();
        var menus = _context.FoodMenus.Where(m => m.ProviderId == provider.Id).ToList();

        // Folosim ViewBag pentru a trimite datele către View
        ViewBag.ProviderId = provider.Id;
        ViewBag.Locations = locations;
        ViewBag.Menus = menus;

        return View();
    }




    // CREATE Location
    [HttpGet]
    public IActionResult CreateLocation(int providerId)
    {
        return View(new LocationModel { ProviderId = providerId });
    }

    [HttpPost]
    public IActionResult CreateLocation(LocationModel location)
    {
        if (ModelState.IsValid)
        {
            _context.Locations.Add(location);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        // Dacă modelul nu este valid, loghează erorile
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogError($"Error in field: {error.ErrorMessage}");
            }
        } 

        return View(location);
    }

    // EDIT Location
    [HttpGet]
    public IActionResult EditLocation(int id)
    {
        var location = _context.Locations.FirstOrDefault(l => l.Id == id);
        if (location == null)
        {
            return NotFound("Location not found.");
        }

        return View(location);
    }

    [HttpPost]
    public IActionResult EditLocation(LocationModel location)
    {
        if (ModelState.IsValid)
        {
            _context.Locations.Update(location);
            _context.SaveChanges();
            return RedirectToAction("ProviderDashboard");
        }

        return View(location);
    }

    // DELETE Location
    [HttpPost]
    public IActionResult DeleteLocation(int id)
    {
        var location = _context.Locations.FirstOrDefault(l => l.Id == id);
        if (location == null)
        {
            return NotFound("Location not found.");
        }

        _context.Locations.Remove(location);
        _context.SaveChanges();

        return RedirectToAction("ProviderDashboard");
    }

    [HttpGet]
    public IActionResult EditAccount()
    {
        var currentUserId = GetCurrentUserId();
        var user = _context.Users.FirstOrDefault(u => u.Id == currentUserId);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        return View(user);
    }

    [HttpPost]
    public IActionResult EditAccount(UserModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = _context.Users.FirstOrDefault(u => u.Id == model.Id);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        user.Name = model.Name;
        user.Email = model.Email;
        user.Password = model.Password; // Asigură-te că parolele sunt criptate
        user.ProfilePicturePath = model.ProfilePicturePath;

        _context.SaveChanges();

        return RedirectToAction("Dashboard");
    }

    [HttpPost]
    public IActionResult DeleteAccount()
    {
        var currentUserId = GetCurrentUserId();
        var user = _context.Users.FirstOrDefault(u => u.Id == currentUserId);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        _context.Users.Remove(user);
        _context.SaveChanges();

        // Oferă utilizatorului posibilitatea de a fi redirecționat către o pagină de logout
        return RedirectToAction("Logout", "Account");
    }


}
