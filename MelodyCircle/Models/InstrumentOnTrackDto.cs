﻿namespace MelodyCircle.Models
{
    public class InstrumentOnTrackDto
    {
        public Guid? InstrumentId { get; set; }
        public Guid? TrackId { get; set; }
        public string? InstrumentName { get; set; }
        public double? StartTime { get; set; }
        public bool IsUploaded { get; set; }
    }
}