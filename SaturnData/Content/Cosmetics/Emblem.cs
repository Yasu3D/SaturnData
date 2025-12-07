using System;
using SaturnData.Content.Cosmetics.Items;
using SaturnData.Content.Items;

namespace SaturnData.Content.Cosmetics;

/// <summary>
/// A secondary icon to display on the user profile, often denoting a stage up clear.
/// </summary>
[Serializable]
public class Emblem : CosmeticItem
{
    /// <summary>
    /// The creator of the <see cref="Emblem"/> image.
    /// </summary>
    public string Artist { get; set; } = "";

    /// <summary>
    /// The local filepath of the <see cref="Emblem"/> image, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string ImagePath { get; set; } = "";

    /// <summary>
    /// The absolute filepath of the <see cref="Emblem"/> image.
    /// </summary>
    public string AbsoluteImagePath => AbsolutePath(ImagePath);
}