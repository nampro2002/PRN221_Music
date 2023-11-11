namespace Group5_MusicPlayer.Models
{
    public class Playlist
    {
        public int PlaylistId { get; set; }
        public string Title { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<SongList> SongLists { get; set;}
    }
}
