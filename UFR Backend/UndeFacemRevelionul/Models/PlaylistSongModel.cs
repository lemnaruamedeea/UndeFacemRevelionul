namespace UndeFacemRevelionul.Models
{
    public class PlaylistSongModel
    {
            public int PlaylistId { get; set; }
            public PlaylistModel Playlist { get; set; }

            public int SongId { get; set; }
            public SongModel Song { get; set; }

            public int PartierId { get; set; }
            public PartierModel Partier { get; set; }
        

    }
}
