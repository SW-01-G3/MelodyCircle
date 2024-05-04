using MelodyCircle.Data;

namespace MelodyCircle.Services
{
    public class UniqueEmailService
    {
        /* Guilherme Bernardino */
        private readonly ApplicationDbContext _context;

        public UniqueEmailService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool DoesEmailExist(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }
    }
}
