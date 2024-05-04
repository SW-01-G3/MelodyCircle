namespace MelodyCircle.Models
{
    public class TutorialRating
    {
        /* Guilherme Bernardino */
        public Guid Id { get; set; }
        public required string UserName { get; set; }
        public int Value { get; set; }

        // Foreign key to the rated tutorial
        public Guid TutorialId { get; set; }
        public Tutorial Tutorial { get; set; }
    }
}
