namespace UndeFacemRevelionul.ViewModels
{
    public class EditMenuViewModel
    {
        public int Id { get; set; }  // ID-ul meniului
        public int ProviderId { get; set; } // ID-ul providerului (dacă este necesar)

        public string Name { get; set; } // Numele meniului
        public string Description { get; set; } // Descrierea meniului
        public decimal Price { get; set; } // Prețul meniului
        

    }
}
