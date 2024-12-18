using UndeFacemRevelionul.Models;

namespace UndeFacemRevelionul.ViewModels
{
    public class SelectLocationViewModel
    {
        public int PartyId { get; set; } // ID-ul petrecerii
        public List<LocationModel> Locations { get; set; } // Lista locațiilor disponibile
        public int TotalPoints { get; set; } // Totalul punctelor petrecăreților
        public float? DiscountedPrice { get; set; } // Prețul redus pentru locația curentă, dacă există reducere
    }
}
