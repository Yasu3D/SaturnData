using System;
using SaturnData.Notation.Core;

namespace SaturnData.Notation.Events;

/// <summary>
/// Changes the current visibility of a layer.
/// </summary>
public sealed class VisibilityChangeEvent : Event, ICloneable
{
    public VisibilityChangeEvent(Timestamp timestamp, bool visibility)
    {
        Timestamp = timestamp;
        Visibility = visibility;
    }

    /// <summary>
    /// The new visibility value this event changes to.
    /// </summary>
    public bool Visibility { get; set; }

    public object Clone() => new VisibilityChangeEvent(new(Timestamp), Visibility);
}