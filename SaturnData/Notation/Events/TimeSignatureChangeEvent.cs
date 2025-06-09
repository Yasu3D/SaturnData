using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Events;

/// <summary>
/// Changes the current time signature.
/// </summary>
public class TimeSignatureChangeEvent : Event, ITimeable
{
    public TimeSignatureChangeEvent(TimeSignatureChangeEvent cloneSource)
    {
        Timestamp = cloneSource.Timestamp;
        Upper = cloneSource.Upper;
        Lower = cloneSource.Lower;
    }

    public TimeSignatureChangeEvent(Timestamp timestamp, int upper, int lower)
    {
        Timestamp = timestamp;
        Upper = upper;
        Lower = lower;
    }
    
    public Timestamp Timestamp { get; set; }

    /// <summary>
    /// The upper (numerator) in the time signature to change to.
    /// </summary>
    public int Upper { get; set; }

    /// <summary>
    /// The lower (denominator) in the time signature to change to.
    /// </summary>
    public int Lower { get; set; }

    /// <summary>
    /// The time signature ratio to change to.
    /// </summary>
    public float Ratio => (float)Upper / Lower;
}