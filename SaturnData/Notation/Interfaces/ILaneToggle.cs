namespace SaturnData.Notation.Interfaces;

public enum LaneSweepDirection
{
    Counterclockwise = 0,
    Clockwise = 1,
    Center = 2,
    Instant = 3,
}

public interface ILaneToggle
{
    /// <summary>
    /// The direction a lane toggle sweep animation takes.
    /// </summary>
    public LaneSweepDirection Direction { get; set; }
}