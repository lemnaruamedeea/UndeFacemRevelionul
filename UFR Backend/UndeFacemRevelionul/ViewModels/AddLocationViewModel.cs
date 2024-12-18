using System.ComponentModel.DataAnnotations;

namespace UndeFacemRevelionul.ViewModels
{
    public class AddLocationViewModel
    {
        [Required(ErrorMessage = "Location name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        public int ProviderId { get; set; } // ID-ul furnizorului care adaugă locația
        public float Price { get; set; }
        public int Capacity { get; set; }
        public float Rating { get; set; }
        //public DateTime Date { get; set; }
    }
}
