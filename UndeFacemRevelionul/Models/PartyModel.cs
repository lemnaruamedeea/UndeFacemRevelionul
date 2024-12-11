namespace UndeFacemRevelionul.Models
{
    public class PartyModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int LocationId {  get; set; }
        public int FoodMenuId { get; set; }
        public float TotalBudget {  get; set; }
        public float RemainingBudget { get; set; }
        public DateTime Date {  get; set; }

        public int TotalPoints { get; set; }

        public ICollection<PartyPartierModel> PartyUsers { get; set; }

        public PlaylistModel Playlist { get; set; }

    }
}
