using System.ComponentModel.DataAnnotations;

namespace MelodyCircle.Models
{
    public class ForumPost
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Content { get; set; }
    }
}
