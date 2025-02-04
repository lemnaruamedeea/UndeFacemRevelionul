namespace UndeFacemRevelionul.ViewModels
{
    public class SuperstitionCompletionViewModel
    {
        public int SuperstitionId { get; set; }
        public int PartyId { get; set; }

        // Câmpul pentru fișierul de imagine
        public IFormFile Image { get; set; }
    }

}
