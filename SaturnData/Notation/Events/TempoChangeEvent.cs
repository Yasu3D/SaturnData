using System;
using SaturnData.Notation.Core;

namespace SaturnData.Notation.Events;

/// <summary>
/// Changes the current bpm (beats per minute).
/// </summary>
public sealed class TempoChangeEvent : Event, ICloneable
{
    public TempoChangeEvent(Timestamp timestamp, float tempo)
    {
        Timestamp = timestamp;
        Tempo = tempo;
    }
    
    /// <summary>
    /// The new bpm value this event changes to.
    /// </summary>
    public float Tempo { get; set; }

    public object Clone() => new TempoChangeEvent(new(Timestamp), Tempo);
}