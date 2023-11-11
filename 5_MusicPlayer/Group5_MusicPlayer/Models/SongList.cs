namespace Group5_MusicPlayer.Models
{
    public class SongList
    {
        public int SongListId { get; set; }
        public DateTime AddedDate { get; set; }
        public int PlaylistId { get; set; }
        public virtual Playlist Playlist { get; set; }
        public int SongId { get; set;}
        public virtual Song Song { get; set; }
    }
}
