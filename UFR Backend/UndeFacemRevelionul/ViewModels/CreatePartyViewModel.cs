using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UndeFacemRevelionul.Models;

namespace UndeFacemRevelionul.ViewModels
{
    public class CreatePartyViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "Max. 100 characters for party name.")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Party date")]
        public DateTime Date { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Budget value has to be over 0.")]
        [Display(Name = "Total budget")]
        public float TotalBudget { get; set; }

        [Display(Name = "Location")]
        public int? LocationId { get; set; } // Locația este opțională

        [Display(Name = "Menu")]
        public int? FoodMenuId { get; set; } // Meniul este opțional

        
    }
}
