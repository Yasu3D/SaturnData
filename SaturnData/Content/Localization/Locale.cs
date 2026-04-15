using System.Collections.Generic;
using SaturnData.Content.Items;

namespace SaturnData.Content.Localization;

public class Locale : ContentItem
{
    /// <summary>
    /// A collection of localized strings.
    /// </summary>
    public Dictionary<string, string> Strings { get; set; } = new();
}