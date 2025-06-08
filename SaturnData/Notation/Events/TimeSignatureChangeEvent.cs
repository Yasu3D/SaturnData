using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Events;

/// <summary>
/// Changes the current time signature.
/// </summary>
public class TimeSignatureChangeEvent : Event, ITimeable
{
    public Timestamp Timestamp { get; set; }

    /// <summary>
    /// The upper (numerator) in the time signature.
    /// </summary>
    public int Upper { get; set; }

    /// <summary>
    /// The lower (denominator) in the time signature.
    /// </summary>
    public int Lower { get; set; }

    /// <summary>
    /// The time signature ratio.
    /// </summary>
    public float Ratio => (float)Upper / Lower;
}