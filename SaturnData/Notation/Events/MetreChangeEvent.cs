using System;
using SaturnData.Notation.Core;

namespace SaturnData.Notation.Events;

/// <summary>
/// Changes the current metre / time signature.
/// </summary>
public sealed class MetreChangeEvent : Event, ICloneable
{
    public MetreChangeEvent(Timestamp timestamp, int upper, int lower)
    {
        Timestamp = timestamp;
        Upper = upper;
        Lower = lower;
    }

    /// <summary>
    /// The upper (numerator) in the metre to change to.
    /// </summary>
    public int Upper { get; set; }

    /// <summary>
    /// The lower (denominator) in the metre to change to.
    /// </summary>
    public int Lower { get; set; }

    /// <summary>
    /// The metre ratio to change to.
    /// </summary>
    public float Ratio => (float)Upper / Lower;

    public object Clone() => new MetreChangeEvent(new(Timestamp), Upper, Lower);
}