using MelodyCircle.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MelodyCircle.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<UserRating> UserRating { get; set; }
        public DbSet<Tutorial> Tutorials { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<SubscribeTutorial> SubscribeTutorials { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Collaboration> Collaborations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}