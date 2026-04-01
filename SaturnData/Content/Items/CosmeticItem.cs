using SaturnData.Content.Items;

namespace SaturnData.Content.Cosmetics.Items;

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
    /// The description of the <see cref="CosmeticItem"/>.
    /// </summary>
    /// <remarks>
    /// - Some CosmeticItems may not display their description.
    /// </remarks>
    public string Description { get; set; } = "";

    /// <summary>
    /// The rarity of the <see cref="CosmeticItem"/>.
    /// </summary>
    /// <remarks>
    /// - Rarity has no effect on what the CosmeticItem does.<br/>
    /// - Some CosmeticItems may not display their rarity.
    /// </remarks>
    public int Rarity { get; set; } = 0;

    /// <summary>
    /// A string listing all contributors of a CosmeticItem.
    /// </summary>
    public string Copyright
    {
        get
        {
            if (this is Emblem emblem)
            {
                return $"(c) {Author}, {emblem.Artist}";
            }
            
            if (this is Icon icon)
            {
                return $"(c) {Author}, {icon.Artist}";
            }
            
            if (this is Navigator navigator)
            {
                return $"(c) {Author}, {navigator.Artist}, {navigator.Voice}";
            }
            
            if (this is NoteSound noteSound)
            {
                return $"(c) {Author}, {noteSound.Artist}";
            }
            
            if (this is Plate plate)
            {
                return $"(c) {Author}, {plate.Artist}";
            }
            
            if (this is SystemMusic systemMusic)
            {
                return $"(c) {Author}, {systemMusic.Artist}";
            }
            
            if (this is SystemSound systemSound)
            {
                return $"(c) {Author}, {systemSound.Artist}";
            }
            
            return $"(c) {Author}";
        }
    }
}