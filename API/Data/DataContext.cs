using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserLike>()
                .HasKey(key => new { key.SourceUserId, key.LikedUserId });

            builder.Entity<UserLike>()
                .HasOne(likingUser => likingUser.SourceUser)
                .WithMany(likesGiven => likesGiven.LikedUsers)
                .HasForeignKey(sourceUser => sourceUser.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade); // TODO: Use NoAction for SQL Server. 
                                                   // Otherwise migration will result in error!  
            builder.Entity<UserLike>()
                .HasOne(likedUser => likedUser.LikedUser)
                .WithMany(likesReceived => likesReceived.LikedByUsers)
                .HasForeignKey(sourceUser => sourceUser.LikedUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}