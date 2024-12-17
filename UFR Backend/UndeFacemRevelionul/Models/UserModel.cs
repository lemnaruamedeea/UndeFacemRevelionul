using System.ComponentModel.DataAnnotations.Schema;
using UndeFacemRevelionul.Logic;
namespace UndeFacemRevelionul.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserRole { get; set; }  // Role as a string (Partier, Provider, etc.)
        public string? ProfilePicturePath { get; set; }

        [NotMapped]
        public IFormFile? ProfilePicture { get; set; }

        // Navigation properties
        public ICollection<ProviderModel> Providers { get; set; }
        public ICollection<PartierModel> Partiers { get; set; }
    }
}