namespace Group5_MusicPlayer.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }       
        public int Role { get; set; } //is user admin or normal user or banned

        public virtual ICollection<Playlist> Playlists { get; set;}
        public virtual ICollection<Song> Songs { get; set; }
    }
}
