using System.ComponentModel.DataAnnotations;

namespace MelodyCircle.Models
{
    public class Tutorial
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }
        public string Creator { get; set; }
        public ICollection<Step> Steps { get; set; }
    }
}