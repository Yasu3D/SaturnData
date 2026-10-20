using SaturnData.Content.Items;
using SaturnData.Content.Items.Attributes;

namespace SaturnData.Content.Cosmetics;

/// <summary>
/// A set of note sound effects, also known as "Hitsounds".
/// </summary>
public class NoteSound : CosmeticItem
{
    /// <summary>
    /// A string listing all contributors of a <see cref="NoteSound"/> pack.
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
    /// The creator of the <see cref="NoteSound"/>.
    /// </summary>
    [SctLineBreak]
    [SctKey("ARTIST", 18)]
    public string Artist { get; set; } = "";
    
    /// <summary>
    /// The time in milliseconds to loop back to, once <see cref="HoldLoopEndTime"/> is reached.
    /// </summary>
    [SctKey("HOLD_LOOP_START", 18)]
    public float HoldLoopStartTime { get; set; } = 0;
    
    /// <summary>
    /// The time in milliseconds at which to loop back to <see cref="HoldLoopEndTime"/>.
    /// </summary>
    [SctKey("HOLD_LOOP_END", 18)]
    public float HoldLoopEndTime { get; set; } = 0;
    
    /// <summary>
    /// The time in milliseconds to loop back to, once <see cref="ReHoldLoopEndTime"/> is reached.
    /// </summary>
    [SctKey("RE_HOLD_LOOP_START", 18)]
    public float ReHoldLoopStartTime { get; set; } = 0;
    
    /// <summary>
    /// The time in milliseconds at which to loop back to <see cref="ReHoldLoopEndTime"/>.
    /// </summary>
    [SctKey("RE_HOLD_LOOP_END", 18)]
    public float ReHoldLoopEndTime { get; set; } = 0;

    /// <summary>
    /// The local filepath of the "click" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctLineBreak]
    [SctKey("CLICK", 15)]
    public string AudioClickPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "guide" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("GUIDE", 15)]
    public string AudioGuidePath { get; set; } = "";

    /// <summary>
    /// The local filepath of the "touch marvelous" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("TOUCH_MARVELOUS", 15)]
    public string AudioTouchMarvelousPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "touch good" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("TOUCH_GOOD", 15)]
    public string AudioTouchGoodPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "slide marvelous" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("SLIDE_MARVELOUS", 15)]
    public string AudioSlideMarvelousPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "slide good" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("SLIDE_GOOD", 15)]
    public string AudioSlideGoodPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "snap marvelous" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("SNAP_MARVELOUS", 15)]
    public string AudioSnapMarvelousPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "snap good" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("SNAP_GOOD", 15)]
    public string AudioSnapGoodPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "hold" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("HOLD", 15)]
    public string AudioHoldPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "re-hold" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("RE_HOLD", 15)]
    public string AudioReHoldPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "chain" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("CHAIN", 15)]
    public string AudioChainPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "bonus marvelous" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("BONUS_MARVELOUS", 15)]
    public string AudioBonusMarvelousPath { get; set; } = "";

    /// <summary>
    /// The local filepath of the "bonus great" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("BONUS_GREAT", 15)]
    public string AudioBonusGreatPath { get; set; } = "";

    /// <summary>
    /// The local filepath of the "r" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("R", 15)]
    public string AudioRPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "damage" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("DAMAGE", 15)]
    public string AudioDamagePath { get; set; } = "";
    
    /// <summary>
    /// The absolute filepath of the "click" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioClickPath => AbsolutePath(AudioClickPath);
    
    /// <summary>
    /// The absolute filepath of the "guide" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioGuidePath => AbsolutePath(AudioGuidePath);
    
    /// <summary>
    /// The absolute filepath of the "touch marvelous" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioTouchMarvelousPath => AbsolutePath(AudioTouchMarvelousPath);
    
    /// <summary>
    /// The absolute filepath of the "touch good" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioTouchGoodPath => AbsolutePath(AudioTouchGoodPath);
    
    /// <summary>
    /// The absolute filepath of the "slide marvelous" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioSlideMarvelousPath => AbsolutePath(AudioSlideMarvelousPath);
    
    /// <summary>
    /// The absolute filepath of the "slide good" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioSlideGoodPath => AbsolutePath(AudioSlideGoodPath);
    
    /// <summary>
    /// The absolute filepath of the "snap marvelous" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioSnapMarvelousPath => AbsolutePath(AudioSnapMarvelousPath);
    
    /// <summary>
    /// The absolute filepath of the "snap good" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioSnapGoodPath => AbsolutePath(AudioSnapGoodPath);
    
    /// <summary>
    /// The absolute filepath of the "hold" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioHoldPath => AbsolutePath(AudioHoldPath);
    
    /// <summary>
    /// The absolute filepath of the "re-hold" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioReHoldPath => AbsolutePath(AudioReHoldPath);
    
    /// <summary>
    /// The absolute filepath of the "chain" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioChainPath => AbsolutePath(AudioChainPath);
    
    /// <summary>
    /// The absolute filepath of the "bonus marvelous" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioBonusMarvelousPath => AbsolutePath(AudioBonusMarvelousPath);

    /// <summary>
    /// The absolute filepath of the "bonus great" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioBonusGreatPath => AbsolutePath(AudioBonusGreatPath);
    
    /// <summary>
    /// The absolute filepath of the "r" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioRPath => AbsolutePath(AudioRPath);
    
    /// <summary>
    /// The absolute filepath of the "damage" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioDamagePath => AbsolutePath(AudioDamagePath);
}