using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Events;

/// <summary>
/// Changes the current visibility of a layer.
/// </summary>
public class InvisibleEffectEvent : Event, ITimeable, ILayerable
{
    public Timestamp Timestamp { get; set; }
    
    public int Layer { get; set; }

    /// <summary>
    /// The new visibility value this event changes to.
    /// </summary>
    public bool Visible { get; set; }
}