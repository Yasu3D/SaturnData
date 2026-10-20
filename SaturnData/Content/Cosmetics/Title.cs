using SaturnData.Content.Items;
using SaturnData.Content.Items.Attributes;

namespace SaturnData.Content.Cosmetics;

/// <summary>
/// A piece of text to display on the user profile. Acts like a profile status or formal title.
/// </summary>
public class Title : CosmeticItem
{
    /// <summary>
    /// The title message to display to users.
    /// </summary>
    [SctLineBreak]
    [SctKey("MESSAGE", 7)]
    public string Message { get; set; } = "";
}