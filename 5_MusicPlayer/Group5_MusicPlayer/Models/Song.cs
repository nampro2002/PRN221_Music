namespace Group5_MusicPlayer.Models
{
    public class Song
    {
        public int SongId { get; set; }
        public string Title { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public int AuthorId { get; set; }
        public virtual User Author { get; set; }

        public string ImgPath { get; set; }
        public string AudioPath {  get; set; }
        public bool IsPrivate { get; set; }

        public virtual ICollection<SongList> SongLists { get; set;}

    }
}
