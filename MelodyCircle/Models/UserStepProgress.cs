using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MelodyCircle.Models
{
    public class UserStepProgress
    {
        [Key]
        public Guid Id { get; set; }

        public Guid StepId { get; set; }

        public string UserName { get; set; }
        public bool IsCompleted { get; set; }

        // Foreign key to the rated user
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
