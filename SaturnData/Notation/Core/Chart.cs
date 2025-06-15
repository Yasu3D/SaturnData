using System.Collections.Generic;

namespace SaturnData.Notation.Core;

/// <summary>
/// Holds all data associated with gameplay.
/// </summary>
public class Chart
{
    /// <summary>
    /// All global events that aren't bound to layers.
    /// </summary>
    public readonly List<Event> Events = [];
    
    /// <summary>
    /// All local events and notes, grouped by layer.
    /// </summary>
    public readonly List<Layer<Event>> Layers = [];
    
    /// <summary>
    /// All lane toggle notes.
    /// </summary>
    public readonly List<Note> LaneToggles = [];
    
    /// <summary>
    /// Editor-only bookmarks.
    /// </summary>
    public readonly List<Bookmark> Bookmarks = [];

    /// <summary>
    /// The Chart End timestamp, where playback stops and the gameplay result is shown.
    /// </summary>
    public Timestamp? ChartEnd = null;
}