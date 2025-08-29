using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Events;

/// <summary>
/// Changes the current bpm (beats per minute).
/// </summary>
public class TempoChangeEvent : Event, ITimeable
{
    public TempoChangeEvent(TempoChangeEvent cloneSource)
    {
        Timestamp = cloneSource.Timestamp;
        Bpm = cloneSource.Bpm;
    }
    
    public TempoChangeEvent(Timestamp timestamp, float bpm)
    {
        Timestamp = timestamp;
        Bpm = bpm;
    }
    
    public Timestamp Timestamp { get; set; }
    
    /// <summary>
    /// The new bpm value this event changes to.
    /// </summary>
    public float Bpm { get; set; }
}