﻿using System.ComponentModel.DataAnnotations;

namespace MelodyCircle.Models
{
    public class Collaboration
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public int MaxUsers { get; set; }

        public string? AccessPassword { get; set; }

        public string CreatorId { get; set; }

        public byte[]? Photo { get; set; }
        public string? PhotoContentType { get; set; }

        [Required]
        public AccessMode AccessMode { get; set; }

        public virtual List<User>? ContributingUsers { get; set; }
        public virtual List<User>? WaitingUsers { get; set; }
        public virtual List<Track>? Tracks { get; set; }

        public bool IsFinished { get; set; }

        public virtual List<CollaborationRating> Ratings { get; set; } = new List<CollaborationRating>();

        public Collaboration()
        {
            ContributingUsers = new List<User>();
            WaitingUsers = new List<User>();
        }
    }
    
    public enum AccessMode
    {
        Public,
        Private
    }
}