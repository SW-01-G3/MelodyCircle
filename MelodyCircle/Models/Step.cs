using System.ComponentModel.DataAnnotations;

namespace MelodyCircle.Models
{
    public class Step
    {
        /* Rodrigo Nogueira */
        public Guid Id { get; set; }
        public Guid TutorialId { get; set; }
        public Tutorial Tutorial { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
        public int Order { get; set; }

        public DateTime CreationDate { get; set; }
    }
}