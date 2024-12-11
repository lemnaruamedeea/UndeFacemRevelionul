using System.IO;

namespace UndeFacemRevelionul.Models
{
    public class PlaylistModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PartyId { get; set; }
        public PartyModel Party { get; set; }

        // Navigation property to the songs in the playlist
        public ICollection<PlaylistSongModel> PlaylistSongs { get; set; }
    }
}
