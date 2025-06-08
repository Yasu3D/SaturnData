using System.Collections.Generic;

namespace SaturnData.Notation.Core;

/// <summary>
/// Holds all data associated with gameplay.
/// </summary>
public class Chart
{
    /// <summary>
    /// Song entry associated with the chart.
    /// </summary>
    public Entry Entry = new();
    
    /// <summary>
    /// All notes (except mask notes), grouped by layer.
    /// </summary>
    public Dictionary<int, List<Note>> NoteLayers = new();

    /// <summary>
    /// All mask notes.
    /// </summary>
    public List<Note> Masks = [];
    
    /// <summary>
    /// All events that are bound to layers, grouped by layer.
    /// </summary>
    public Dictionary<int, List<Event>> EventLayers = new();

    /// <summary>
    /// All global events that aren't bound to layers.
    /// </summary>
    public List<Event> GlobalEvents = [];
}