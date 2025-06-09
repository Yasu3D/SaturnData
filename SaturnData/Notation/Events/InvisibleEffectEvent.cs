using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Events;

/// <summary>
/// Changes the current visibility of a layer.
/// </summary>
public class InvisibleEffectEvent : Event, ITimeable
{
    public InvisibleEffectEvent(InvisibleEffectEvent cloneSource)
    {
        Timestamp = cloneSource.Timestamp;
        Visible = cloneSource.Visible;
    }
    
    public InvisibleEffectEvent(Timestamp timestamp, bool visible)
    {
        Timestamp = timestamp;
        Visible = visible;
    }
    
    public Timestamp Timestamp { get; set; }

    /// <summary>
    /// The new visibility value this event changes to.
    /// </summary>
    public bool Visible { get; set; }
}