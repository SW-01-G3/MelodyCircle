namespace MelodyCircle.Models
{
    public class Track
    {
        public Guid Id { get; set; }

        public Guid AssignedUserId { get; set; }

        public Guid CollaborationId { get; set; }

        public double BPM { get; set; }

        public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(4);

        public virtual User User { get; set; }

        public virtual Collaboration Collaboration { get; set; }

        public virtual ICollection<InstrumentOnTrack> InstrumentsOnTrack { get; set; }
    }
}