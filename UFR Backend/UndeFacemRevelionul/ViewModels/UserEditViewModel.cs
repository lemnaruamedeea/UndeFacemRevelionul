using System.ComponentModel.DataAnnotations;
using UndeFacemRevelionul.Logic;

namespace UndeFacemRevelionul.ViewModels;


public class UserEditViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public IFormFile? ProfilePicture { get; set; }

    // Add fields for password change
    public string CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
    public string? ConfirmNewPassword { get; set; }
}