using Microsoft.AspNetCore.Mvc;
using UndeFacemRevelionul.Models;
using UndeFacemRevelionul.ViewModels;
using UndeFacemRevelionul.Logic;
using UndeFacemRevelionul.ContextModels;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace UndeFacemRevelionul.Controllers;

public class AccountController : Controller
{

    private readonly RevelionContext _context;
    private readonly ILogger<AccountController> _logger;

    public AccountController(RevelionContext context, ILogger<AccountController> logger)
    {
        _context = context;
        _logger = logger;
    }

   [HttpGet]
    public IActionResult Register()
    {
        // Asigură-te că modelul este transmis corect
        var model = new RegisterViewModel();
        return View(model);
    }


    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Validăm că rolul nu este 'Admin'
            if (model.UserRole.ToString() == "Admin")
            {
                ModelState.AddModelError(string.Empty, "Nu aveți permisiunea să alegeți rolul Admin.");
                return View(model);
            }

            // Dacă utilizatorul a încărcat o poză de profil, o salvăm
            string profilePicturePath = null;
            //if (model.ProfilePicture != null)
            //{
            //    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", model.ProfilePicture.FileName);
            //    using (var stream = new FileStream(filePath, FileMode.Create))
            //    {
            //        model.ProfilePicture.CopyTo(stream);
            //    }
            //    profilePicturePath = "/uploads/" + model.ProfilePicture.FileName;
            //}

            
            var newUser = new UserModel
            {
                Name = model.Name,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                UserRole = model.UserRole.ToString(),
                ProfilePicturePath = profilePicturePath
            };

            // Salvăm utilizatorul în baza de date
            _context.Users.Add(newUser);
            _context.SaveChanges();

            if (newUser.UserRole == "Partier")
            {
                var partier = new PartierModel
                {
                    UserId = newUser.Id,
                    Points = 0,  // Puncte inițiale, poți să le modifici cum dorești
                    RankId = 1  // Poți adăuga un RankId de bază (de exemplu, 1 pentru "nou")
                };

                // Adaugă-l în tabelul PartierModel
                _context.Partiers.Add(partier);
                _context.SaveChanges();
            }

            if (newUser.UserRole == "Provider")
            {
                var provider = new ProviderModel
                {
                    UserId = newUser.Id,
                    Rating = 0,  // Puncte inițiale, poți să le modifici cum dorești
                    Details = "Add details",  // Poți adăuga un RankId de bază (de exemplu, 1 pentru "nou")
                    Contact = "Add contact info",

                };

                // Adaugă-l în tabelul PartierModel
                _context.Providers.Add(provider);
                _context.SaveChanges();
            }

            TempData["Message"] = "User registered successfully!";
            return RedirectToAction("Login");
        }
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogError(error.ErrorMessage);
            }
        }

        // Dacă modelul nu este valid, afișăm din nou formularul cu erori
        return View(model);
    }


    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);  
    }

    [HttpGet]
    public IActionResult SuccessRegister()
    {
        
        ViewBag.Message = TempData["Message"];
        return View(); 
    }

    [HttpGet]
    public IActionResult Login()
    {
        // Creează un model gol pentru prima afișare a paginii
        var model = new LoginViewModel();
        return View(model);
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

            if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UserRole.ToString()), // User's role
                new Claim("UserId", user.Id.ToString()) // User's ID
            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                if (user.UserRole == "Partier")
                {
                    return RedirectToAction("Dashboard", "Partier");
                }
                else if (user.UserRole == "Provider")
                {
                    return RedirectToAction("Dashboard", "Provider");
                }
                else if (user.UserRole == "Admin")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
            }
        
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken] // Protecție CSRF
    public async Task<IActionResult> Logout()
    {
        
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        TempData["Message"] = "You have successfully logged out.";

        return RedirectToAction("Login", "Account");
    }




}
