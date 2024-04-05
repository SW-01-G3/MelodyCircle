﻿namespace MelodyCircle.Models
{
    public class InstrumentOnTrack
    {
        public Guid Id { get; set; }

        public string InstrumentType { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan Duration { get; set; }

        public virtual Track Track { get; set; }
    }
}