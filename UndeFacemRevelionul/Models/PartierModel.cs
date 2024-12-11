namespace UndeFacemRevelionul.Models
{
    public class PartierModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Points { get; set; }
        public int RankId { get; set; }
        public ICollection<PlaylistSongModel> PlaylistSongs { get; set; }

    }
}
