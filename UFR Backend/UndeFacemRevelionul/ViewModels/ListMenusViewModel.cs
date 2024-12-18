using UndeFacemRevelionul.Models;

namespace UndeFacemRevelionul.ViewModels
{
    public class ListMenusViewModel
    {
        public int PartyId { get; set; } // ID-ul petrecerii
        public List<FoodMenuModel> Menus { get; set; } // Lista meniurilor
    }

}

