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
    /// All notes (except mask notes), grouped by layer.
    /// </summary>
    public readonly Dictionary<int, Layer<Note>> NoteLayers = new();

    /// <summary>
    /// All mask notes.
    /// </summary>
    public readonly List<Note> Masks = [];

    /// <summary>
    /// The Chart End timestamp, where playback stops and the gameplay result is shown.
    /// </summary>
    public Timestamp? ChartEnd = null;
}