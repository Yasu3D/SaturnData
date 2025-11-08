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
    
    /// <summary>
    /// Returns the duration of a lane toggle sweep animation in milliseconds.
    /// </summary>
    public static float SweepDuration(LaneSweepDirection direction, int size)
    {
        return direction switch
        {
            LaneSweepDirection.Counterclockwise => size * 8.3333333f,
            LaneSweepDirection.Clockwise => size * 8.3333333f,
            LaneSweepDirection.Center => size * 4.1666666f,
            LaneSweepDirection.Instant => 0,
            _ => 0,
        };
    }
}