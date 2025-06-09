using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Events;

/// <summary>
/// A sub-event contained within another event.
/// Used for any kind of event that requires
/// multiple timestamps, such as reverses and stops.
/// </summary>
public class EffectSubEvent : Event, ITimeable
{
    public EffectSubEvent(Timestamp timestamp, Event parent)
    {
        Timestamp = timestamp;
        Parent = parent;
    }
    
    public Timestamp Timestamp { get; set; }
    
    public Event Parent { get; }
}