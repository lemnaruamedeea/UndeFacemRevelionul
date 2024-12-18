using System.ComponentModel.DataAnnotations;

namespace UndeFacemRevelionul.ViewModels
{
    public class AddMenuViewModel
    {
        [Required(ErrorMessage = "Menu name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public int ProviderId { get; set; } // ID-ul furnizorului care adaugă locația
        public float Price { get; set; }
        public float Rating { get; set; }
        //public DateTime Date { get; set; }
        public string? MenuFilePath { get; set; }
    }
}