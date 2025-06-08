using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Events;

/// <summary>
/// Changes the current visibility of a layer.
/// </summary>
public class InvisibleEffectEvent : Event, ITimeable, ILayerable
{
    public Timestamp Timestamp { get; set; }
    
    /// <summary>
    /// TODO: Docs
    /// </summary>
    public bool IsActive { get; set; }

    public int Layer { get; set; }
}