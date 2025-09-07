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
        Tempo = cloneSource.Tempo;
    }
    
    public TempoChangeEvent(Timestamp timestamp, float tempo)
    {
        Timestamp = timestamp;
        Tempo = tempo;
    }
    
    public Timestamp Timestamp { get; set; }
    
    /// <summary>
    /// The new bpm value this event changes to.
    /// </summary>
    public float Tempo { get; set; }
}