using System;
using SaturnData.Content.Cosmetics.Items;
using SaturnData.Content.Items;

namespace SaturnData.Content.Cosmetics;

/// <summary>
/// A primary icon to display on the user profile. Acts like a profile picture.
/// </summary>
[Serializable]
public class Icon : CosmeticItem
{
    /// <summary>
    /// The creator of the <see cref="Icon"/> image.
    /// </summary>
    public string Artist { get; set; } = "";

    /// <summary>
    /// The local filepath of the <see cref="Icon"/> image, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string ImagePath { get; set; } = "";

    /// <summary>
    /// The absolute filepath of the <see cref="Icon"/> image.
    /// </summary>
    public string AbsoluteImagePath => AbsolutePath(ImagePath);
}