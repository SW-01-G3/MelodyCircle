using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MelodyCircle.Models
{
    public class UserRating
    {
        public Guid Id { get; set; }
        public required string UserName { get; set; } 
        public required string RatedUserName { get; set; } 
        public int Value { get; set; }

        // Foreign key to the rated user
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
