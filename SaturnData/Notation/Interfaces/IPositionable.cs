using System;

namespace SaturnData.Notation.Interfaces;

/// <summary>
/// Implements position and size attributes.
/// </summary>
public interface IPositionable
{
    /// <summary>
    /// The position of the IPositionable.<br/>
    /// Position starts at 3 o'clock/East, and steps Counterclockwise in 6° increments.
    /// </summary>
    /// <remarks>
    /// Range [0 - 59]
    /// </remarks>
    public int Position { get; set; }

    /// <summary>
    /// The size of the IPositionable.<br/>
    /// Increasing values makes a Note grow Counterclockwise.
    /// </summary>
    /// <remarks>
    /// Range [1 - 60]
    /// </remarks>
    public int Size { get; set; }

    internal static int LimitPosition(int value)
    {
        int r = value % 60;
        return r < 0 ? r + 60 : r;
    }

    internal static int LimitSize(int value)
    {
        return value == 0 ? 60 : Math.Clamp(value, 1, 60);
    }
}