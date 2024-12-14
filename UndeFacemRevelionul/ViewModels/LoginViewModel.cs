namespace UndeFacemRevelionul.ViewModels
{
    public class LoginViewModel
    {
        [Email]
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; } 
}
