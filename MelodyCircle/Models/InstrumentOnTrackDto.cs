namespace MelodyCircle.Models
{
    public class InstrumentOnTrackDto
    {
        public Guid TrackId { get; set; }

        public string InstrumentName { get; set; }

        public double StartTime { get; set; }
    }
}