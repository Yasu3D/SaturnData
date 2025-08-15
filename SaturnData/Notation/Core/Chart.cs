using System;
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
    public List<Event> Events { get; set; } = [];
    
    /// <summary>
    /// All local events and notes, grouped by layer.
    /// </summary>
    public List<Layer> Layers { get; set; } = [];
    
    /// <summary>
    /// All lane toggle notes.
    /// </summary>
    public List<Note> LaneToggles { get; set; } = [];
    
    /// <summary>
    /// Editor-only bookmarks.
    /// </summary>
    public List<Bookmark> Bookmarks { get; set; } = [];
}