using Group5_MusicPlayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Group5_MusicPlayer.Data
{
    public class MusicPlayerDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<SongList> SongsList { get; set; }

        public MusicPlayerDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //User
            modelBuilder.Entity<User>().HasKey(u => u.UserId);
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            //Category
            modelBuilder.Entity<Category>().HasKey(c => c.CategoryId);

            //Song
            modelBuilder.Entity<Song>().HasKey(s => s.SongId);
            modelBuilder.Entity<Song>()
                .HasOne(s => s.Category).WithMany(c => c.Songs)
                .HasForeignKey(s => s.CategoryId).IsRequired();
            modelBuilder.Entity<Song>()
                .HasOne(s => s.Author).WithMany(u => u.Songs)
                .HasForeignKey(s => s.AuthorId).IsRequired();

            //Playlist
            modelBuilder.Entity<Playlist>().HasKey(s => s.PlaylistId);
            modelBuilder.Entity<Playlist>()
               .HasOne(p => p.User).WithMany(u => u.Playlists)
               .HasForeignKey(p => p.UserId).IsRequired();

            //Song_List Relationship
            modelBuilder.Entity<SongList>().HasKey(s => s.SongListId);
            modelBuilder.Entity<SongList>()
                .HasOne(s => s.Playlist).WithMany(p => p.SongLists)
                .HasForeignKey(s => s.PlaylistId).IsRequired();
            modelBuilder.Entity<SongList>()
                .HasOne(s => s.Song).WithMany(s => s.SongLists)
                .HasForeignKey(s => s.SongId).IsRequired();
        }
    }
}
