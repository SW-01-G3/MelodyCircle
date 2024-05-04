﻿namespace MelodyCircle.Models
{
    public class UploadedInstrument
    {
        /* Guilherme Bernardino */
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte[] SoundContent { get; set; }
        public Guid CollaborationId { get; set; }
        public Collaboration Collaboration { get; set; }
    }
}
