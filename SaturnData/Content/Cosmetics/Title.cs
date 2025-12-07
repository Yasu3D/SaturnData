using System;
using SaturnData.Content.Cosmetics.Items;

namespace SaturnData.Content.Cosmetics;

/// <summary>
/// A piece of text to display on the user profile. Acts like a profile status or formal title.
/// </summary>
[Serializable]
public class Title : CosmeticItem
{
    /// <summary>
    /// The title message to display to users.
    /// </summary>
    public string Message { get; set; } = "";
}