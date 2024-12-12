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
        // Navigation property to tasks related to this party
        public ICollection<TaskModel> Tasks { get; set; }

        // Navigation property to superstitions related to this party
        public ICollection<SuperstitionModel> Superstitions { get; set; }

    }
}
