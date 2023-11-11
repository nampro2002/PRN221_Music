namespace Group5_MusicPlayer.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Song> Songs { get; set; }
    }
}
