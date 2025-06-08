using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Events;

/// <summary>
/// Changes the current bpm (beats per minute).
/// </summary>
public class BpmChangeEvent : Event, ITimeable
{
    public Timestamp Timestamp { get; set; }
}