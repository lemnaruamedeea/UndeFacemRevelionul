using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UndeFacemRevelionul.ContextModels;
using UndeFacemRevelionul.Models;

using Google.Apis.YouTube.v3;
using Google.Apis.Services;
using System.Threading.Tasks;
using System.Threading;

namespace UndeFacemRevelionul.Controllers
{
    public class PlaylistController : Controller
    {
        private readonly RevelionContext _context;

        public PlaylistController(RevelionContext context)
        {
            _context = context;
        }

        // Create Playlist
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePlaylist(int partyId)
        {
            var party = _context.Parties.FirstOrDefault(p => p.Id == partyId);
            if (party == null)
                return NotFound();

            var playlist = new PlaylistModel
            {
                Name = $"{party.Name} Playlist",
                PartyId = partyId,
                PlaylistSongs = new List<PlaylistSongModel>()
            };

            _context.Playlists.Add(playlist);
            _context.SaveChanges();

            return RedirectToAction("PartyDetails", "Partier", new { id = partyId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSongToPlaylist(int playlistId, string songTitle, string artistName)
        {
            var playlist = _context.Playlists
                .Include(pl => pl.PlaylistSongs)
                .FirstOrDefault(pl => pl.Id == playlistId);

            if (playlist == null)
                return NotFound();

            // Build the search query for the song
            string searchQuery = $"{songTitle} {artistName}";

            // Use the YouTube API to search for the song
            var youtubeUrl = await GetYouTubeVideoUrlAsync(searchQuery);

            if (youtubeUrl == null)
            {
                TempData["ErrorMessage"] = "Song not found on YouTube.";
                return RedirectToAction("PartyDetails", "Partier", new { id = playlist.PartyId });
            }

            // Create new song with YouTube URL
            var song = new SongModel
            {
                Title = songTitle,
                Artist = artistName,
                SongPath = youtubeUrl // This is the YouTube URL
            };

            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            // Add song to playlist
            var playlistSong = new PlaylistSongModel
            {
                PlaylistId = playlistId,
                SongId = song.Id,
                PartierId = GetCurrentPartierId() // Assuming this method exists to get the current partier
            };

            _context.PlaylistSongs.Add(playlistSong);
            await _context.SaveChangesAsync();

            // **Verificăm dacă playlist-ul are acum mai mult de 10 melodii**
            int songCount = playlist.PlaylistSongs.Count + 1; // Adăugăm și melodia curentă

            if (songCount > 10)
            {
                var task = _context.Tasks.FirstOrDefault(t => t.PartyId == playlist.PartyId && t.Name == "Playlist");
                var partier = _context.Partiers.FirstOrDefault(p => p.Id == task.PartierId);
                if (task != null && !task.IsCompleted)
                {
                    task.IsCompleted = true; // Bifăm task-ul
                    partier.Points += task.Points;
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("PartyDetails", "Partier", new { id = playlist.PartyId });
        }

        private async Task<string> GetYouTubeVideoUrlAsync(string searchQuery)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyCyhHqVWjKp-mOUoF05q3OjU-21KNizZJg", // Replace with your YouTube API key
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = searchQuery;
            searchListRequest.MaxResults = 1; // You can adjust this to get more results if you like

            var searchListResponse = await searchListRequest.ExecuteAsync();
            var firstVideo = searchListResponse.Items.FirstOrDefault();

            // If a video is found, return the URL
            if (firstVideo != null)
            {
                string videoId = firstVideo.Id.VideoId;
                return $"https://www.youtube.com/watch?v={videoId}";
            }

            return null;
        }

        // Delete Song from Playlist
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveSongFromPlaylist(int playlistId, int songId)
        {
            var playlistSong = _context.PlaylistSongs
                .Include(ps => ps.Playlist) // Ensure Playlist navigation is included
                .FirstOrDefault(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

            if (playlistSong == null)
            {
                TempData["ErrorMessage"] = "The song could not be found in the playlist.";
                return RedirectToAction("PartyDetails", "Partier", new { id = playlistId });
            }

            _context.PlaylistSongs.Remove(playlistSong);
            _context.SaveChanges();

            return RedirectToAction("PartyDetails", "Partier", new { id = playlistSong.Playlist.PartyId });
        }


        // Helper method to get the current partier's ID
        private int GetCurrentPartierId()
        {
            var currentUserId = int.Parse(User.FindFirst("UserId").Value);
            var partier = _context.Partiers.FirstOrDefault(p => p.UserId == currentUserId);

            if (partier == null)
                throw new InvalidOperationException("Partier not found for the current user.");

            return partier.Id;
        }
    }
}
