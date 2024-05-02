using MelodyCircle.Models;

namespace MelodyCircle.ViewModels
{
    public class SearchResultViewModel
    {
        public List<User> Users { get; set; }
        public List<Tutorial> Tutorials { get; set; }
        public List<Collaboration> Collaborations { get; set; }
    }
}