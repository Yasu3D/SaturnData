namespace SaturnData.Notation.Interfaces;

public enum MaskDirection
{
    CounterClockwise = 0,
    Clockwise = 1,
    Center = 2,
    Instant = 3,
}

public interface IMask
{
    /// <summary>
    /// The direction a mask sweep animation takes.
    /// </summary>
    public MaskDirection Direction { get; set; }
}