namespace UndeFacemRevelionul.Models
{
    public class PartierModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Points { get; set; }
        public int RankId { get; set; }
        public UserModel User { get; set; }
        public ICollection<PlaylistSongModel> PlaylistSongs { get; set; }
        // Navigation property for the many-to-many relationship with Party
        public ICollection<PartyPartierModel> PartyUsers { get; set; }
        // Navigation property for tasks assigned to this partier
        public ICollection<TaskModel> Tasks { get; set; }

        // Navigation property for superstitions assigned to this partier
        public ICollection<SuperstitionModel> Superstitions { get; set; }
    }
}
