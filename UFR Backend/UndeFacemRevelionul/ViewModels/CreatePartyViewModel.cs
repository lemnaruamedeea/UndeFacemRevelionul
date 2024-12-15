using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UndeFacemRevelionul.Models;

namespace UndeFacemRevelionul.ViewModels
{
    public class CreatePartyViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Numele petrecerii poate avea maxim 100 de caractere.")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data petrecerii")]
        public DateTime Date { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Bugetul trebuie să fie un număr pozitiv.")]
        [Display(Name = "Buget total")]
        public float TotalBudget { get; set; }

        [Display(Name = "Locație")]
        public int? LocationId { get; set; } // Locația este opțională

        [Display(Name = "Meniu")]
        public int? FoodMenuId { get; set; } // Meniul este opțional

        
    }
}
