using Microsoft.AspNetCore.Mvc;
using UndeFacemRevelionul.Models;
using UndeFacemRevelionul.ViewModels;
using UndeFacemRevelionul.ContextModels;
using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace UndeFacemRevelionul.Controllers
{
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
            var model = new RegisterViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

                // Salvăm poza de profil dacă este încărcată
                string profilePicturePath = null;
                if (model.ProfilePicture != null)
                {
                    var fileName = Guid.NewGuid().ToString() + "_" + model.ProfilePicture.FileName;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        model.ProfilePicture.CopyTo(stream);
                    }

                    profilePicturePath = "/uploads/" + fileName;
                }

                // Creăm utilizatorul
                var newUser = new UserModel
                {
                    Name = model.Name,
                    Email = model.Email,
                    Password = HashPassword(model.Password),
                    UserRole = model.UserRole.ToString(),
                    ProfilePicturePath = profilePicturePath
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                // Logica specifică rolului
                if (newUser.UserRole == "Partier")
                {
                    var partier = new PartierModel
                    {
                        UserId = newUser.Id,
                        Points = 0,
                        RankId = 1
                    };

                    _context.Partiers.Add(partier);
                }
                else if (newUser.UserRole == "Provider")
                {
                    var provider = new ProviderModel
                    {
                        UserId = newUser.Id,
                        Rating = 0,
                        Details = "No details yet",
                        Contact = newUser.Email // Setăm contactul ca email-ul utilizatorului
                    };

                    _context.Providers.Add(provider);
                }

                _context.SaveChanges();

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
                        new Claim(ClaimTypes.Role, user.UserRole.ToString()),
                        new Claim("UserId", user.Id.ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // Redirecționăm pe baza rolului
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["Message"] = "You have successfully logged out.";
            return RedirectToAction("Login", "Account");
        }
    }
}
