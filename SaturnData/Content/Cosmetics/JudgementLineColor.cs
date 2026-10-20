using SaturnData.Content.Items;
using SaturnData.Content.Items.Attributes;

namespace SaturnData.Content.Cosmetics;
/// <summary>
/// 
/// </summary>
public class JudgementLineColor : CosmeticItem
{
    /// <summary>
    /// The local filepath of the <see cref="JudgementLineColor"/> gradient image, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctLineBreak]
    [SctKey("IMAGE_PATH", 10)]
    public string ImagePath { get; set; } = "";

    /// <summary>
    /// The absolute filepath of the <see cref="JudgementLineColor"/> gradient image.
    /// </summary>
    [SctIgnore]
    public string AbsoluteImagePath => AbsolutePath(ImagePath);
}