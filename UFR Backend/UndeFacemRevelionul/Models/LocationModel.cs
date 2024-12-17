namespace UndeFacemRevelionul.Models
{
    public class LocationModel
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        // Navigation property
        public ProviderModel Provider { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public float Price { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; }
        public float Rating {  get; set; }
        public DateTime Date {get; set; }
        public ICollection<PartyModel>? Parties { get; set; }  // Back-reference

    }
}
