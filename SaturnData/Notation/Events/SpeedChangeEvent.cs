using SaturnData.Notation.Core;

namespace SaturnData.Notation.Events;

/// <summary>
/// Changes the current scroll speed of a layer.
/// </summary>
public sealed class SpeedChangeEvent : Event
{
    public SpeedChangeEvent(SpeedChangeEvent cloneSource)
    {
        Timestamp = cloneSource.Timestamp;
        Speed = cloneSource.Speed;
    }

    public SpeedChangeEvent(Timestamp timestamp, float speed)
    {
        Timestamp = timestamp;
        Speed = speed;
    }
    
    /// <summary>
    /// The speed multiplier to set the scroll speed to.<br/>
    /// Default: <c>1.0</c><br/>
    /// Range: <c>Single.MinValue / Single.MaxValue</c>
    /// </summary>
    public float Speed { get; set; }
}