using SaturnData.Content.Items;
using SaturnData.Content.Items.Attributes;

namespace SaturnData.Content.Cosmetics;

/// <summary>
/// A set of background songs to play in different scenes.
/// </summary>
public class SystemMusic : CosmeticItem
{
    /// <summary>
    /// A string listing all contributors of a <see cref="SystemMusic"/> pack.
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
    /// The creator of the <see cref="SystemMusic"/>.
    /// </summary>
    [SctLineBreak]
    [SctKey("ARTIST", 15)]
    public string Artist { get; set; } = "";

    /// <summary>
    /// The local filepath of the "attract bgm" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("ATTRACT", 15)]
    public string AudioAttractPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "select bgm" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("SELECT", 15)]
    public string AudioSelectPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "result bgm" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("RESULT", 15)]
    public string AudioResultPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "stage-up select bgm" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("STAGE_UP_SELECT", 15)]
    public string AudioStageUpSelectPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "stage-up secret bgm" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("STAGE_UP_SECRET", 15)]
    public string AudioStageUpSecretPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "see-you bgm" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("SEE_YOU", 15)]
    public string AudioSeeYouPath { get; set; } = "";
    
    /// <summary>
    /// The absolute filepath of the "attract bgm" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioAttractPath => AbsolutePath(AudioAttractPath);
    
    /// <summary>
    /// The absolute filepath of the "select bgm" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioSelectPath => AbsolutePath(AudioSelectPath);
    
    /// <summary>
    /// The absolute filepath of the "result bgm" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioResultPath => AbsolutePath(AudioResultPath);
    
    /// <summary>
    /// The absolute filepath of the "stage-up select" bgm audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioStageUpSelectPath => AbsolutePath(AudioStageUpSelectPath);
    
    /// <summary>
    /// The absolute filepath of the "stage-up secret" bgm audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioStageUpSecretPath => AbsolutePath(AudioStageUpSecretPath);
    
    /// <summary>
    /// The absolute filepath of the "see-you" bgm audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioSeeYouPath => AbsolutePath(AudioSeeYouPath);
}