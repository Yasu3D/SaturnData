using SaturnData.Content.Items;
using SaturnData.Content.Items.Attributes;

namespace SaturnData.Content.Cosmetics;

/// <summary>
/// A primary icon to display on the user profile. Acts like a profile picture.
/// </summary>
public class Icon : CosmeticItem
{
    /// <summary>
    /// A string listing all contributors of an <see cref="Icon"/>.
    /// </summary>
    public override string Copyright
    {
        get
        {
            if (Author == "" && Artist == "") return "";
            if (Author == "" && Artist != "") return $"(c) {Artist}";
            if (Author != "" && Artist == "") return $"(c) {Author}";
            
            return $"(c) {Author}, {Artist}";
        }
    }
    
    /// <summary>
    /// The creator of the <see cref="Icon"/> image.
    /// </summary>
    [SctLineBreak]
    [SctKey("ARTIST", 10)]
    public string Artist { get; set; } = "";

    /// <summary>
    /// The local filepath of the <see cref="Icon"/> image, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("IMAGE_PATH", 10)]
    public string ImagePath { get; set; } = "";

    /// <summary>
    /// The absolute filepath of the <see cref="Icon"/> image.
    /// </summary>
    [SctIgnore]
    public string AbsoluteImagePath => AbsolutePath(ImagePath);
}