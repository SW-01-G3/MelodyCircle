﻿using MelodyCircle.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MelodyCircle.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<UserRating> UserRating { get; set; }
        public DbSet<TutorialRating> TutorialRating { get; set; }
        public DbSet<CollaborationRating> CollaborationRating { get; set; }
        public DbSet<Tutorial> Tutorials { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<SubscribeTutorial> SubscribeTutorials { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Collaboration> Collaborations { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<InstrumentOnTrack> InstrumentOnTrack { get; set; }
        public DbSet<ForumPost> ForumPost { get; set; }
        public DbSet<UploadedInstrument> UploadedInstruments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.ContributingCollaborations)
                .WithMany(c => c.ContributingUsers)
                .UsingEntity(j => j.ToTable("UserCollaborations"));

            modelBuilder.Entity<User>()
                .HasMany(u => u.WaitingCollaborations)
                .WithMany(c => c.WaitingUsers)
                .UsingEntity(j => j.ToTable("UserWaitingCollaborations"));

            modelBuilder.Entity<User>()
                .HasMany(u => u.ContributingCollaborations)
                .WithMany(c => c.ContributingUsers)
                .UsingEntity(j => j.ToTable("UserContributingCollaborations"));
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}