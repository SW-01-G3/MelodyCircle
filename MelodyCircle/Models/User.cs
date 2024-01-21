using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace MelodyCircle.Models
{
    public class User: IdentityUser
    {
        public Guid Id { get; set; }

        [PersonalData]
        [Required]
        [EmailAddress]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        [PersonalData]
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [PersonalData]
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data Nascimento")]
        public DateOnly BirthDate { get; set; }

        [PersonalData]
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
