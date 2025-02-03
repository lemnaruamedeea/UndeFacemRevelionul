using UndeFacemRevelionul.Models;

namespace UndeFacemRevelionul.ViewModels
{
    public class ListMenusViewModel
    {
        public int PartyId { get; set; } // ID-ul petrecerii
        public List<FoodMenuModel> Menus { get; set; } // Lista meniurilor
        public int TotalPoints { get; set; } // Totalul punctelor petrecăreților
        public float? DiscountedPrice { get; set; } // Prețul redus pentru meniu curent, dacă există reducere
    }

}

