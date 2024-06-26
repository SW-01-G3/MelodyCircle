﻿using MelodyCircle.Models;

namespace MelodyCircle.ViewModels
{
    public class ArrangementPanelViewModel
    {
        public Collaboration Collaboration { get; set; }

        public IList<Track> Tracks { get; set; }

        public IList<Instrument> AvailableInstruments { get; set; }
        public IList<UploadedInstrument> UploadedInstruments { get; set; }

        public bool IsContributorOrCreator { get; set; }

        public Track UserTrack { get; set; }

        public int AssignedTrackNumber { get; set; }
    }
}