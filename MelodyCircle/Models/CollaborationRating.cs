namespace MelodyCircle.Models
{
    public class CollaborationRating
    {
        public Guid Id { get; set; }
        public required string UserName { get; set; }
        public int Value { get; set; }

        // Foreign key to the rated tutorial
        public Guid CollaborationId { get; set; }
        public Collaboration Collaboration { get; set; }
    }
}
