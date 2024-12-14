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
        return View();
    }


    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var newUser = new UserModel
            {
                Name = model.Name,
                Email = model.Email,
                Password = HashPassword(model.Password),
                UserRole = model.UserRole.ToString()
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            TempData["Message"] = "User registered successfully!";
            return RedirectToAction("SuccessRegister");  
        }

        // Debugging 
        foreach (var key in ModelState.Keys)
        {
            var state = ModelState[key];
            if (state.Errors.Count > 0)
            {
                
                foreach (var error in state.Errors)
                {
                    Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                }
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
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.UserRole.ToString()), // User's role
                new Claim("UserId", user.Id.ToString()) // User's ID
            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
        }

        return View(model);
    }




}
