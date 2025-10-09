using System;
using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Events;

/// <summary>
/// Scrolls a set of notes on a layer backwards.
/// </summary>
public class ReverseEffectEvent : Event
{
    public ReverseEffectEvent(ReverseEffectEvent cloneSource)
    {
        SubEvents = new EffectSubEvent[3];
        SubEvents[0] = new(cloneSource.SubEvents[0].Timestamp, this);
        SubEvents[1] = new(cloneSource.SubEvents[1].Timestamp, this);
        SubEvents[2] = new(cloneSource.SubEvents[2].Timestamp, this);
    }

    public ReverseEffectEvent()
    {
        SubEvents = new EffectSubEvent[3];
    }
    
    /// <summary>
    /// The timestamp when the reverse effect begins.<br/>
    /// Modifying this timestamp will move all sub-events as well.
    /// </summary>
    public sealed override Timestamp Timestamp => SubEvents[0].Timestamp;

    /// <summary>
    /// Sub-events that define specific sections of the reverse effect.<br/>
    /// </summary>
    /// <code>
    /// [0] = Reverse Effect Begin
    /// [1] = Reverse Effect End / Reverse Note Capture Begin
    /// [2] = Reverse Note Capture End
    /// </code>
    public readonly EffectSubEvent[] SubEvents;
}