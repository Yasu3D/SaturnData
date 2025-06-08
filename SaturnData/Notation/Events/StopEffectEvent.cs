using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Events;

/// <summary>
/// Changes the scroll speed of a layer to 0 for its duration.<br/>
/// Effectively the same as two HiSpeed events.
/// </summary>
public class StopEffectEvent : Event, ITimeable, ILayerable
{
    /// <summary>
    /// The timestamp when the stop effect begins.<br/>
    /// Modifying this timestamp will move all sub-events as well.
    /// </summary>
    public Timestamp Timestamp
    {
        get => SubEvents[0].Timestamp;
        set
        { 
            // Move all sub-events equally when setting timestamp.
            Timestamp delta = value - SubEvents[0].Timestamp;
            
            SubEvents[0].Timestamp += delta;
            SubEvents[1].Timestamp += delta;
        }
    }

    public int Layer { get; set; }

    /// <summary>
    /// Sub-events that define specific sections of the stop effect.<br/>
    /// </summary>
    /// <code>
    /// [0] = Stop Effect Begin
    /// [1] = Stop Effect End
    /// </code>
    public readonly EffectSubEvent[] SubEvents = new EffectSubEvent[2];
}