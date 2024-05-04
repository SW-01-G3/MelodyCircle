using System.ComponentModel.DataAnnotations;

namespace MelodyCircle.Services
{
    public class UniqueEmailAttribute : ValidationAttribute
    {
        /* Guilherme Bernardino */
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var emailService = (UniqueEmailService)validationContext.GetService(typeof(UniqueEmailService));
            var email = value as string;

            if (emailService.DoesEmailExist(email))
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"Email is already in use.";
        }
    }
}
