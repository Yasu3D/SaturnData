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
    public readonly List<Event> GlobalEvents = [];
    
    /// <summary>
    /// All events that are bound to layers, grouped by layer.
    /// </summary>
    public readonly Dictionary<int, Layer<Event>> EventLayers = new();
    
    /// <summary>
    /// All notes (except lane toggle notes), grouped by layer.
    /// </summary>
    public readonly Dictionary<int, Layer<Note>> NoteLayers = new();

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