using UndeFacemRevelionul.Models;

namespace UndeFacemRevelionul.ViewModels
{
    public class ListLocationsViewModel
    {
        public int PartyId { get; set; } // ID-ul petrecerii
        public List<LocationModel> Locations { get; set; } // Lista meniurilor
    }
}
