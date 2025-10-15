using System.Collections.Generic;
using SaturnData.Notation.Core;

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
    public EffectSubEvent[] SubEvents;

    /// <summary>
    /// All notes that appear during the reverse effect.
    /// </summary>
    public readonly HashSet<Note> ContainedNotes = [];

    /// <summary>
    /// Returns a non-linear interpolation from SubEvents[1].ScaledTime to SubEvents[2].ScaledTime.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public float SampleReversedTime(float time)
    {
        float start = SubEvents[0].Timestamp.Time;
        float end = SubEvents[1].Timestamp.Time;
        float t = start == end ? 0 : (time - start) / (end - start);

        t = t switch
        {
            >= 1 => t,
            >= 0.965f => (t - 0.965f) * 0.23162f + 0.991893f,
            >= 0.930f => (t - 0.930f) * 0.23300f + 0.983738f,
            >= 0.910f => (t - 0.910f) * 0.31100f + 0.977518f,
            >= 0.893f => (t - 0.893f) * 0.46700f + 0.969579f,
            >= 0.875f => (t - 0.875f) * 0.62300f + 0.958365f,
            >= 0.855f => (t - 0.855f) * 0.79100f + 0.942545f,
            >= 0.715f => (t - 0.715f) * 0.93400f + 0.811785f,
            >= 0.570f => (t - 0.570f) * 1.01100f + 0.665190f,
            >= 0.000f => t * 1.167f,
            _ => t,
        };
        
        return SubEvents[2].Timestamp.ScaledTime + t * (SubEvents[1].Timestamp.ScaledTime - SubEvents[2].Timestamp.ScaledTime);
    }

    /// <summary>
    /// Returns <c>true</c> if time lies between<br/>
    /// <see cref="SubEvents"/>[0] and <see cref="SubEvents"/>[1].
    /// </summary>
    public bool IsActive(float time) => time >= SubEvents[0].Timestamp.Time && time <= SubEvents[1].Timestamp.Time;
}