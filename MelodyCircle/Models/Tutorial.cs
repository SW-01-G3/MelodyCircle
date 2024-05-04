using System.ComponentModel.DataAnnotations;

namespace MelodyCircle.Models
{
    public class Tutorial
    {
        /* Rodrigo Nogueira */
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string Creator { get; set; }
        public virtual List<Step>? Steps { get; set; }
        public int? StepCount { get; set; }
        public byte[]? Photo { get; set; }
        public string? PhotoContentType { get; set; }
        public int? SubscribersCount { get; set; } = 0;

        public virtual List<TutorialRating> Ratings { get; set; } = new List<TutorialRating>();

        public DateTime CreationDate { get; set; }
    }
}