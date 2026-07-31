using SaturnData.Content.Cosmetics.Items;

namespace SaturnData.Content.Cosmetics;

/// <summary>
/// 
/// </summary>
public class JudgementLineColor : CosmeticItem
{
    /// <summary>
    /// A string listing all contributors of a <see cref="JudgementLineColor"/>.
    /// </summary>
    public override string Copyright => $"(c) {Author}";
    
    /// <summary>
    /// The local filepath of the <see cref="JudgementLineColor"/> gradient image, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string ImagePath { get; set; } = "";

    /// <summary>
    /// The absolute filepath of the <see cref="JudgementLineColor"/> gradient image.
    /// </summary>
    public string AbsoluteImagePath => AbsolutePath(ImagePath);
}