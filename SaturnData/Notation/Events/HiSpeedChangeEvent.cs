using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Events;

/// <summary>
/// Changes the current scroll speed of a layer.
/// </summary>
public class HiSpeedChangeEvent : Event, ITimeable, ILayerable
{
    public Timestamp Timestamp { get; set; }
    
    /// <summary>
    /// TODO: Docs
    /// </summary>
    public float HiSpeed { get; set; }

    public int Layer { get; set; }
}