namespace UndeFacemRevelionul.Models
{
    public class ProviderModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }   // Foreign Key to UserModel
        public float Rating { get; set; }
        public string Details { get; set; }
        public string Contact { get; set; }

        // Navigation property
        public UserModel User { get; set; }  // Each provider is linked to one user
    }
}