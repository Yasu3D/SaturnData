using SaturnData.Content.Items;
using SaturnData.Content.Items.Attributes;

namespace SaturnData.Content.Cosmetics;

/// <summary>
/// A background to display behind the user profile.
/// </summary>
public class Plate : CosmeticItem
{
    /// <summary>
    /// A string listing all contributors of a <see cref="Plate"/>.
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
    /// The creator of the <see cref="Plate"/> image.
    /// </summary>
    [SctLineBreak]
    [SctKey("ARTIST", 10)]
    public string Artist { get; set; } = "";

    /// <summary>
    /// The local filepath of the <see cref="Plate"/> image, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("IMAGE_PATH", 10)]
    public string ImagePath { get; set; } = "";

    /// <summary>
    /// The absolute filepath of the <see cref="Plate"/> image.
    /// </summary>
    [SctIgnore]
    public string AbsoluteImagePath => AbsolutePath(ImagePath);
}