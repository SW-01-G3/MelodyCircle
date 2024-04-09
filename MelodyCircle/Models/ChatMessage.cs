namespace MelodyCircle.Models
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } 
        public Guid CollaborationId { get; set; } 
        public string MessageText { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual User User { get; set; }
        public virtual Collaboration Collaboration { get; set; }
    }

}
