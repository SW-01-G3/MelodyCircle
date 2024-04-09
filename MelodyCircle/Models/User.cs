using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MelodyCircle.Models
{
    public class User : IdentityUser
    {
        //[PersonalData]
        //[Required]
        //[EmailAddress]
        //[UniqueEmail]
        //[Display(Name = "E-Mail")]
        //public string Email { get; set; }

        [PersonalData]
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Nome")]
        public string Name { get; set; }

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

        [PersonalData]
        [DataType(DataType.Upload)]
        [Display(Name = "Foto de Perfil")]
        public byte[]? ProfilePicture { get; set; } =[];

        [PersonalData]
        [DataType(DataType.Text)]
        [Display(Name = "Localidade")]
        public string? Locality { get; set; }

        [PersonalData]
        //[DataType(DataType.)]
        [Display(Name = "Género")]
        public Gender? Gender { get; set; }

        public virtual List<User>? Connections { get; set; }

        public virtual List<UserRating> Ratings { get; set; } = new List<UserRating>();

        public virtual List<string> MusicURI { get; set; } = new List<string>();

        public virtual List<Collaboration>? ContributingCollaborations { get; set; }
        public virtual List<Collaboration>? WaitingCollaborations { get; set; }

		public int? LoginCount { get; set; } 
		public DateTime? LastLoginTime { get; set; }
        public DateTime? SignupTime { get; set; }

        public DateTime CreationDate { get; set; }
    }

    public enum Gender
    {
        Male,
        Female,
        Other
    }
}