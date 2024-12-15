using System.Collections.Generic;
using System.Threading;
using UndeFacemRevelionul.Models;

namespace UndeFacemRevelionul.ViewModels
{
    public class PartyDetailsViewModel
    {
        
            public PartyModel Party { get; set; }  // Detalii despre petrecere
            public string CurrentUserName { get; set; }  // Numele utilizatorului logat
            public string CurrentUserEmail { get; set; }  // Email-ul utilizatorului logat
            public List<PartierModel> PartyUsers { get; set; }  // Utilizatorii partierului
        

    }
}
