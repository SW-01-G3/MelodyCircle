using System.ComponentModel.DataAnnotations;

namespace MelodyCircle.Models
{
    public class Comment
    {
        public Guid Id { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public ForumPost ForumPost { get; set; }

        public Guid ForumPostId { get; set; }

        public User User { get; set; }

        public string UserId { get; set; }
    }
}