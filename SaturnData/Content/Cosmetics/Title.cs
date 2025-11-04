using System;

namespace SaturnData.Content.Cosmetics;

/// <summary>
/// A piece of text to display on the user profile. Acts like a profile status or formal title.
/// </summary>
[Serializable]
public class Title : ContentItem
{
    /// <summary>
    /// The title message to display to users.
    /// </summary>
    public string Message { get; set; } = "";
}