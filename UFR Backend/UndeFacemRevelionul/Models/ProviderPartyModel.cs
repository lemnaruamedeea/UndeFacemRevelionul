using System.IO;
using UndeFacemRevelionul.Models;

public class ProviderPartyModel
{
    public int ProviderId { get; set; }
    public ProviderModel Provider { get; set; } // Navigation property to Provider

    public int PartyId { get; set; }
    public List<ProviderPartyModel> ProviderParties { get; set; }
    public PartyModel Party { get; set; } // Navigation property to Party
}

