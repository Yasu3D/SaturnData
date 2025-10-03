using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Events;

/// <summary>
/// Changes the current visibility of a layer.
/// </summary>
public class VisibilityChangeEvent : Event
{
    public VisibilityChangeEvent(VisibilityChangeEvent cloneSource)
    {
        Timestamp = cloneSource.Timestamp;
        Visible = cloneSource.Visible;
    }
    
    public VisibilityChangeEvent(Timestamp timestamp, bool visible)
    {
        Timestamp = timestamp;
        Visible = visible;
    }

    /// <summary>
    /// The new visibility value this event changes to.
    /// </summary>
    public bool Visible { get; set; }
}