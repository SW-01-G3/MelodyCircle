public static class InstrumentData
{
    public static List<Instrument> AvailableInstruments = new List<Instrument>
    {
        new Instrument { Name = "Guitar", SoundPath = "/sounds/guitar.mp3" },
        new Instrument { Name = "Bass", SoundPath = "/sounds/bass.mp3" },
        new Instrument { Name = "Drums", SoundPath = "/sounds/drums.mp3" }
    };
}

public class Instrument
{
    public string Name { get; set; }
    public string SoundPath { get; set; }
}