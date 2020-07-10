using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Like>()
                .HasKey(key => new { key.LikerId, key.LikeeId});

            builder.Entity<Like>()
                .HasOne(like => like.Likee)
                .WithMany(user => user.ReceivedLikes)
                .HasForeignKey(like => like.LikeeId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Like>()
                .HasOne(like => like.Liker)
                .WithMany(user => user.SentLikes)
                .HasForeignKey(like => like.LikerId)
                .OnDelete(DeleteBehavior.Restrict);            
        }
    }
}