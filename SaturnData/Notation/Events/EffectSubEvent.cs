using SaturnData.Notation.Core;

namespace SaturnData.Notation.Events;

/// <summary>
/// A sub-event contained within another event.
/// Used for any kind of event that requires
/// multiple timestamps, such as reverses and stops.
/// </summary>
public sealed class EffectSubEvent : Event
{
    public EffectSubEvent(Timestamp timestamp, Event parent)
    {
        Timestamp = timestamp;
        Parent = parent;
    }
    
    public Event Parent { get; }
}