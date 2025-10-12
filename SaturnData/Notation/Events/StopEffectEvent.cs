using System;
using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Events;

/// <summary>
/// Changes the scroll speed of a layer to 0 for its duration.<br/>
/// Effectively the same as two HiSpeed events.
/// </summary>
public class StopEffectEvent : Event
{
    public StopEffectEvent(StopEffectEvent cloneSource)
    {
        SubEvents = new EffectSubEvent[3];
        SubEvents[0] = new(cloneSource.SubEvents[0].Timestamp, this);
        SubEvents[1] = new(cloneSource.SubEvents[1].Timestamp, this);
    }
    
    public StopEffectEvent()
    {
        SubEvents = new EffectSubEvent[2];
    }
    
    /// <exception cref="ArgumentException">Thrown when <c>subEvents</c> does not have 2 elements.</exception>
    public StopEffectEvent(EffectSubEvent[] subEvents)
    {
        if (subEvents.Length != 2) throw new ArgumentException("SubEvents array must have 2 elements");

        SubEvents = subEvents;
    }
    
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
    public EffectSubEvent[] SubEvents;
}