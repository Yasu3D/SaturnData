using SaturnData.Content.Items.Attributes;

namespace SaturnData.Content.Items;

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
    [SctLineBreak]
    [SctKey("AUTHOR", 11)]
    public string Author { get; set; } = "";

    /// <summary>
    /// The description of the <see cref="CosmeticItem"/>.
    /// </summary>
    /// <remarks>
    /// - Some CosmeticItems may not display their description.
    /// </remarks>
    [SctKey("DESCRIPTION", 11)]
    public string Description { get; set; } = "";

    /// <summary>
    /// The rarity of the <see cref="CosmeticItem"/>.
    /// </summary>
    /// <remarks>
    /// - Rarity has no effect on what the CosmeticItem does.<br/>
    /// - Some CosmeticItems may not display their rarity.
    /// </remarks>
    [SctKey("RARITY", 11)]
    public int Rarity { get; set; } = 0;

    /// <summary>
    /// A string listing all contributors of a <see cref="CosmeticItem"/>.
    /// </summary>
    [SctIgnore]
    public virtual string Copyright => $"(c) {Author}";
}