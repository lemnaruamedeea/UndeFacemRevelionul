namespace UndeFacemRevelionul.Models
{
    public class SongModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string SongPath { get; set; }

        public ICollection<PlaylistSongModel> PlaylistSongs { get; set; }
    }
}
