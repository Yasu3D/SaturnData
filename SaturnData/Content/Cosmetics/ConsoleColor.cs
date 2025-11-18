using System;

namespace SaturnData.Content.Cosmetics;

/// <summary>
/// A color theme of the input console.
/// </summary>
[Serializable]
public class ConsoleColor : CosmeticItem
{
    /// <summary>
    /// The color to display in-game for areas with hidden lanes.
    /// </summary>
    public uint ColorA { get; set; } = 0xFFFF0000;
    
    /// <summary>
    /// The color to display in-game for areas with visible lanes.
    /// </summary>
    public uint ColorB { get; set; } = 0xFF00FF00;
    
    /// <summary>
    /// The color to display in-game for areas with active notes.
    /// </summary>
    public uint ColorC { get; set; } = 0xFF0000FF;
    
    /// <summary>
    /// The color to send to external LEDs for areas with hidden lanes.
    /// </summary>
    public uint LedA { get; set; } = 0xFF800000;
    
    /// <summary>
    /// The color to send to external LEDs for areas with visible lanes.
    /// </summary>
    public uint LedB { get; set; } = 0xFF008000;
    
    /// <summary>
    /// The color to send to external LEDs for areas with active notes.
    /// </summary>
    public uint LedC { get; set; } = 0xFF000080;
}