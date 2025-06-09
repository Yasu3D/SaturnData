using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Events;

/// <summary>
/// Changes the current bpm (beats per minute).
/// </summary>
public class BpmChangeEvent : Event, ITimeable
{
    public BpmChangeEvent(BpmChangeEvent cloneSource)
    {
        Timestamp = cloneSource.Timestamp;
        Bpm = cloneSource.Bpm;
    }
    
    public BpmChangeEvent(Timestamp timestamp, float bpm)
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