using System.IO;

namespace UndeFacemRevelionul.Models
{
    public class PartyPartierModel
    {
        public int PartyId { get; set; }
        public PartyModel Party { get; set; }

        public int PartierId { get; set; }
        public PartierModel Partier { get; set; }
    }
}
