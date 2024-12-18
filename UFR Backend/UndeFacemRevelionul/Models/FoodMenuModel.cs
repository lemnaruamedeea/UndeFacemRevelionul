namespace UndeFacemRevelionul.Models
{
    public class FoodMenuModel
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        // Navigation property
        public ProviderModel Provider { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Rating { get; set; }
        public float Price { get; set; }
        public string? MenuFilePath { get; set; }
        public ICollection<PartyModel>? Parties { get; set; }  // Back-reference

    }
}