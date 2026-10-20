using System.Collections.Generic;

namespace SaturnData.Content.Cosmetics.NavigatorData;

/// <summary>
/// A collection of <see cref="NavigatorDialogue"/>s in one language.
/// </summary>
public class NavigatorLanguage
{
    /// <summary>
    /// The collection of dialogues, accessible by their key.
    /// </summary>
    public Dictionary<string, NavigatorDialogue> Dialogues { get; set; } = [];
}