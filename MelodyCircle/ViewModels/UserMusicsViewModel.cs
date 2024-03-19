using MelodyCircle.Models;

namespace MelodyCircle.ViewModels
{
    public class UserMusicsViewModel
    {
        public User User { get; set; }

        public IList<string> MusicUri { get; set; }
    }
}
