using SaturnData.Notation.Core;

namespace SaturnData.Notation.Events;

/// <summary>
/// Changes the current visibility of a layer.
/// </summary>
public sealed class VisibilityChangeEvent : Event
{
    public VisibilityChangeEvent(VisibilityChangeEvent cloneSource)
    {
        Timestamp = new(cloneSource.Timestamp);
        Visibility = cloneSource.Visibility;
    }
    
    public VisibilityChangeEvent(Timestamp timestamp, bool visibility)
    {
        Timestamp = timestamp;
        Visibility = visibility;
    }

    /// <summary>
    /// The new visibility value this event changes to.
    /// </summary>
    public bool Visibility { get; set; }
}