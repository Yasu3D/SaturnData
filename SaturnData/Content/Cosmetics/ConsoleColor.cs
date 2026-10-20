using SaturnData.Content.Items;
using SaturnData.Content.Items.Attributes;

namespace SaturnData.Content.Cosmetics;

/// <summary>
/// A color theme of the input console.
/// </summary>
public class ConsoleColor : CosmeticItem
{
    /// <summary>
    /// The color to display in-game for areas with hidden lanes.
    /// </summary>
    [SctLineBreak]
    [SctKey("COLOR_A", 7)]
    public uint ColorA { get; set; } = 0xFFFF0000;
    
    /// <summary>
    /// The color to display in-game for areas with visible lanes.
    /// </summary>
    [SctKey("COLOR_B", 7)]
    public uint ColorB { get; set; } = 0xFF00FF00;
    
    /// <summary>
    /// The color to display in-game for areas with active notes.
    /// </summary>
    [SctKey("COLOR_C", 7)]
    public uint ColorC { get; set; } = 0xFF0000FF;
    
    /// <summary>
    /// The color to send to external LEDs for areas with hidden lanes.
    /// </summary>
    [SctKey("LED_A", 7)]
    public uint LedA { get; set; } = 0xFF800000;
    
    /// <summary>
    /// The color to send to external LEDs for areas with visible lanes.
    /// </summary>
    [SctKey("LED_B", 7)]
    public uint LedB { get; set; } = 0xFF008000;
    
    /// <summary>
    /// The color to send to external LEDs for areas with active notes.
    /// </summary>
    [SctKey("LED_C", 7)]
    public uint LedC { get; set; } = 0xFF000080;
}