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
    /// The local filepath of the <see cref="Plate"/> image, relative to the <see cref="ContentItem.AbsoluteSourcePath">AbsoluteSourcePath</see>.
    /// </summary>
    [SctKey("IMAGE_PATH", 10)]
    public string ImagePath { get; set; } = "";

    /// <summary>
    /// The color of the <c>TITLE</c> label on the plate.
    /// </summary>
    [SctLineBreak]
    [SctKey("TITLE_LABEL_COLOR", 18)]
    public uint TitleLabelColor { get; set; } = 0xFFB2B2B2;
    
    /// <summary>
    /// The color of the <c>Lv.</c> label on the plate.
    /// </summary>
    [SctKey("LEVEL_LABEL_COLOR", 18)]
    public uint LevelLabelColor { get; set; } = 0xFFB2B2B2;
    
    /// <summary>
    /// The color of the <c>SP</c> label on the plate.
    /// </summary>
    [SctKey("POINT_LABEL_COLOR", 18)]
    public uint PointLabelColor { get; set; } = 0xFFB2B2B2;
    
    /// <summary>
    /// The color of the <c>Rate:</c> label on the plate.
    /// </summary>
    [SctKey("RATING_LABEL_COLOR", 18)]
    public uint RatingLabelColor { get; set; } = 0xFFFFFFFF;
    
    /// <summary>
    /// The color of the <see cref="Title"/> on the plate.
    /// </summary>
    [SctLineBreak]
    [SctKey("TITLE_COLOR", 11)]
    public uint TitleColor { get; set; } = 0xFF1A1A1A;
    
    /// <summary>
    /// The color of the profile level value on the plate.
    /// </summary>
    [SctKey("LEVEL_COLOR", 11)]
    public uint LevelColor { get; set; } = 0xFF1A1A1A;
    
    /// <summary>
    /// The color of the profile point value on the plate.
    /// </summary>
    [SctKey("POINT_COLOR", 11)]
    public uint PointColor { get; set; } = 0xFF1A1A1A;
    
    /// <summary>
    /// The color of the profile name on the plate.
    /// </summary>
    [SctKey("NAME_COLOR", 11)]
    public uint NameColor { get; set; } = 0xFF1A1A1A;

    /// <summary>
    /// The absolute filepath of the <see cref="Plate"/> image.
    /// </summary>
    [SctIgnore]
    public string AbsoluteImagePath => AbsolutePath(ImagePath);
}