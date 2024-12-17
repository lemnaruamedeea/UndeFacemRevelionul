using UndeFacemRevelionul.Models;

namespace UndeFacemRevelionul.ViewModels
{
    public class AddMemberViewModel
    {
        public int PartyId { get; set; }
        public int SelectedPartierId { get; set; }
        public List<PartierModel> AvailablePartiers { get; set; }
    }

}
