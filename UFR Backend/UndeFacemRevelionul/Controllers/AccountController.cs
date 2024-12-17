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

            if (user != null)
            {
                if ((user.UserRole == "Partier" || user.UserRole == "Provider") && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    if (user.BlockedUntil != null && user.BlockedUntil > DateTime.Now)
                    {
                        var remainingHours = (user.BlockedUntil.Value - DateTime.Now).TotalHours;
                        return Content($"Contul tău a fost blocat pentru încă: {Math.Ceiling(remainingHours)} ore.");
                    }
                    else
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, user.Name),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.Role, user.UserRole.ToString()), // User's role
                            new Claim("UserId", user.Id.ToString()), // User's ID
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
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

                    }
                }

                else if (user.UserRole == "Admin" && model.Password == user.Password)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.UserRole.ToString()), // User's role
                        new Claim("UserId", user.Id.ToString()), // User's ID
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

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

    // Step 1: GET Edit Action
    [HttpGet]
    public IActionResult Edit()
    {
        var userId = GetCurrentUserId(); // Get the logged-in user's ID
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return NotFound();
        }

        var model = new UserEditViewModel
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };

        return View(model);
    }


    // Step 2: POST Edit Action
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAsync(UserEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogError(error.ErrorMessage);
            }
        }
        if (ModelState.IsValid)
        {
             var userId = GetCurrentUserId(); // Get the logged-in user's ID
             var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                _logger.LogError($"User with Id {model.Id} not found.");
                return NotFound();
            }
            if (user != null)
            {
                // Update the basic user information
                user.Name = model.Name;
                user.Email = model.Email;

                // Optionally update the profile picture if it's provided
                if (model.ProfilePicture != null)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", model.ProfilePicture.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        model.ProfilePicture.CopyTo(stream);
                    }
                    user.ProfilePicturePath = "/uploads/" + model.ProfilePicture.FileName;
                }

                // If the password is being changed, validate and update it
                if (!string.IsNullOrEmpty(model.CurrentPassword) && !string.IsNullOrEmpty(model.NewPassword) && model.NewPassword == model.ConfirmNewPassword)
                {
                    if (BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password))
                    {
                        user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                    }
                    else
                    {
                        ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                        return View(model);
                    }
                }
                else if (!string.IsNullOrEmpty(model.NewPassword) || !string.IsNullOrEmpty(model.ConfirmNewPassword))
                {
                    ModelState.AddModelError("NewPassword", "New password and confirmation do not match.");
                    return View(model);
                }

                // Save changes to the database
                _context.SaveChanges();
                // Update the authentication cookie with the new name
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name), // Update the user's name
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UserRole.ToString()),
                new Claim("UserId", user.Id.ToString()), // User's ID
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false // This means the session will expire when the browser is closed
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                TempData["SuccessMessage"] = "Account updated successfully!";
                return RedirectToAction("Edit");


            }
        }

        // If the model state is invalid, return to the form with validation errors
        return View(model);
    }

    // Step 3: POST Delete Action
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete()
    {
        var userId = GetCurrentUserId(); // Get the logged-in user's ID
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return NotFound();
        }

        // Optionally remove related data (Partiers/Providers)
        if (user.UserRole == "Partier")
        {
            var partier = _context.Partiers.FirstOrDefault(p => p.UserId == userId);
            if (partier != null) _context.Partiers.Remove(partier);
        }
        else if (user.UserRole == "Provider")
        {
            var provider = _context.Providers.FirstOrDefault(p => p.UserId == userId);
            if (provider != null) _context.Providers.Remove(provider);
        }

        // Delete the user from the Users table
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        // Sign out the user and redirect to registration
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Message"] = "Account deleted successfully.";
        return RedirectToAction("Register", "Account");
    }




}