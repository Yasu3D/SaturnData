using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Events;

/// <summary>
/// Scrolls a set of notes on a layer backwards.
/// </summary>
public class ReverseEffectEvent : Event, ITimeable, ILayerable
{
    /// <summary>
    /// The timestamp when the reverse effect begins.<br/>
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
            SubEvents[2].Timestamp += delta;
        }
    }

    public int Layer { get; set; }

    /// <summary>
    /// Sub-events that define specific sections of the reverse effect.<br/>
    /// </summary>
    /// <code>
    /// [0] = Reverse Effect Begin
    /// [1] = Reverse Effect End / Reverse Note Capture Begin
    /// [2] = Reverse Note Capture End
    /// </code>
    public readonly EffectSubEvent[] SubEvents = new EffectSubEvent[3];
}