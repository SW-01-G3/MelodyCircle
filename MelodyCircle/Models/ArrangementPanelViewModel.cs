using MelodyCircle.Models;

public class ArrangementPanelViewModel
{
    public Collaboration Collaboration { get; set; }
    public IList<Track> Tracks { get; set; }
    public bool IsContributorOrCreator { get; set; }
}