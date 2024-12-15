using System.ComponentModel.DataAnnotations;

namespace UndeFacemRevelionul.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; } 
}