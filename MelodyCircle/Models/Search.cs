namespace MelodyCircle.Models
{
    public class Search
    {
        /* Rodrigo Nogueira */
        public string SearchTerm { get; set; }
        public SearchType? SearchType { get; set; }
    }

    public enum SearchType
    {
        /* Rodrigo Nogueira */
        None,
        User,
        Tutorial,
        Collaboration
    }
}