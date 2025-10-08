using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Events;

/// <summary>
/// Changes the current scroll speed of a layer.
/// </summary>
public sealed class SpeedChangeEvent : Event
{
    public SpeedChangeEvent(SpeedChangeEvent cloneSource)
    {
        Timestamp = cloneSource.Timestamp;
        HiSpeed = cloneSource.HiSpeed;
    }

    public SpeedChangeEvent(Timestamp timestamp, float hiSpeed)
    {
        Timestamp = timestamp;
        HiSpeed = hiSpeed;
    }
    
    /// <summary>
    /// The speed multiplier to set the scroll speed to.<br/>
    /// Default: <c>1.0</c><br/>
    /// Range: <c>Single.MinValue / Single.MaxValue</c>
    /// </summary>
    public float HiSpeed { get; set; }
}