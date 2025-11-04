namespace SaturnData.Content.Cosmetics;

/// <summary>
/// Defines the base data for a user-defined cosmetic.
/// </summary>
public abstract class CosmeticItem : ContentItem
{
    /// <summary>
    /// The author of the <see cref="CosmeticItem"/>.
    /// </summary>
    /// <remarks>
    /// This property should be used for the person that compiled the <see cref="CosmeticItem"/>.
    /// Authors for media used (images, audio, video) are specified in classes that inherit from <see cref="CosmeticItem"/>.
    /// </remarks>
    public string Author { get; set; } = "";

    /// <summary>
    /// The rarity of the <see cref="CosmeticItem"/>.
    /// </summary>
    /// <remarks>
    /// - Rarity has no effect on what the CosmeticItem does.<br/>
    /// - Some CosmeticItems may not display their rarity.
    /// </remarks>
    public int Rarity { get; set; } = 0;
}