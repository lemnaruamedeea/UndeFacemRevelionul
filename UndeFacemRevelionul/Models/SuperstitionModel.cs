namespace UndeFacemRevelionul.Models
{
    public class SuperstitionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PartierId { get; set; }
        public bool IsCompleted { get; set; }
        public int PartyId { get; set; }
        public int Points { get; set; }
    }
}
