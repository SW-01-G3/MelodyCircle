
namespace MelodyCircle.Models
{
    public class Search
    {
        public string SearchTerm { get; set; }
        public SearchType SearchType { get; set; }
    }

    public enum SearchType
    {
        User,
        Tutorial,
        Collaboration
    }
}