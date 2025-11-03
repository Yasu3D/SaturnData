using System;
using SaturnData.Notation.Core;

namespace SaturnData.Notation.Events;

/// <summary>
/// Changes the scroll speed of a layer to 0 for its duration.<br/>
/// Effectively the same as two HiSpeed events.
/// </summary>
public class StopEffectEvent : Event, ICloneable
{
    /// <summary>
    /// The timestamp when the stop effect begins.<br/>
    /// Modifying this timestamp will move all sub-events as well.
    /// </summary>
    public sealed override Timestamp Timestamp => SubEvents[0].Timestamp;

    /// <summary>
    /// Sub-events that define specific sections of the stop effect.<br/>
    /// </summary>
    /// <code>
    /// [0] = Stop Effect Begin
    /// [1] = Stop Effect End
    /// </code>
    public EffectSubEvent[] SubEvents = new EffectSubEvent[2];

    public object Clone()
    {
        StopEffectEvent clone = new();
        clone.SubEvents[0] = new(SubEvents[0].Timestamp, clone);
        clone.SubEvents[1] = new(SubEvents[1].Timestamp, clone);

        return clone;
    }
}