using System;
using SaturnData.Content.Cosmetics.Items;
using SaturnData.Content.Items;

namespace SaturnData.Content.Cosmetics;

/// <summary>
/// A background to display behind the user profile.
/// </summary>
[Serializable]
public class Plate : CosmeticItem
{
    /// <summary>
    /// The creator of the <see cref="Plate"/> image.
    /// </summary>
    public string Artist { get; set; } = "";

    /// <summary>
    /// The local filepath of the <see cref="Plate"/> image, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string ImagePath { get; set; } = "";

    /// <summary>
    /// The absolute filepath of the <see cref="Plate"/> image.
    /// </summary>
    public string AbsoluteImagePath => AbsolutePath(ImagePath);
}