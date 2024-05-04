using MelodyCircle.Models;

namespace MelodyCircle.ViewModels
{
    /* Rodrigo Nogueira */
    public class SearchResultViewModel
    {
        public List<User> Users { get; set; }
        public List<Tutorial> Tutorials { get; set; }
        public List<Collaboration> Collaborations { get; set; }
    }
}