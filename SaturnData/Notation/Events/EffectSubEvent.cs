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
    public Timestamp Timestamp { get; set; }
}