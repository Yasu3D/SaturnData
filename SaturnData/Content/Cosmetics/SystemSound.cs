using SaturnData.Content.Items;
using SaturnData.Content.Items.Attributes;

namespace SaturnData.Content.Cosmetics;

/// <summary>
/// A set of menu sound effects to play on UI interactions.
/// </summary>
public class SystemSound : CosmeticItem
{
    /// <summary>
    /// A string listing all contributors of a <see cref="SystemSound"/> pack.
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
    /// The creator of the <see cref="SystemSound"/> pack.
    /// </summary>
    [SctLineBreak]
    [SctKey("ARTIST", 25)]
    public string Artist { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "login" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("LOGIN", 25)]
    public string AudioLoginPath { get; set; } = "";

    /// <summary>
    /// The local filepath of the "login card success" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("LOGIN_CARD_SUCCESS", 25)]
    public string AudioLoginCardSuccessPath { get; set; } = "";

    /// <summary>
    /// The local filepath of the "login card error" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("LOGIN_CARD_ERROR", 25)]
    public string AudioLoginCardErrorPath { get; set; } = "";

    /// <summary>
    /// The local filepath of the "cycle mode" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("CYCLE_MODE", 25)]
    public string AudioCycleModePath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "cycle folder" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("CYCLE_FOLDER", 25)]
    public string AudioCycleFolderPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "cycle song" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("CYCLE_SONG", 25)]
    public string AudioCycleSongPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "cycle option" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("CYCLE_OPTION", 25)]
    public string AudioCycleOptionPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "select ok" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("SELECT_OK", 25)]
    public string AudioSelectOkPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "select back" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("SELECT_BACK", 25)]
    public string AudioSelectBackPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "select denied" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("SELECT_DENIED", 25)]
    public string AudioSelectDeniedPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "select decide" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("SELECT_DECIDE", 25)]
    public string AudioSelectDecidePath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "select preview song" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("SELECT_PREVIEW_SONG", 25)]
    public string AudioSelectPreviewSongPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "select start song" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("SELECT_START_SONG", 25)]
    public string AudioSelectStartSongPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "select start song alt" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("SELECT_START_SONG_ALT", 25)]
    public string AudioSelectStartSongAltPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "favorite add" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("FAVORITE_ADD", 25)]
    public string AudioFavoriteAddPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "favorite remove" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("FAVORITE_REMOVE", 25)]
    public string AudioFavoriteRemovePath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "level up count" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("LEVELUP_COUNT", 25)]
    public string AudioLevelUpCountPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "level up finished" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("LEVELUP_FINISHED", 25)]
    public string AudioLevelUpFinishedPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "score count" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("RESULT_SCORE_COUNT", 25)]
    public string AudioResultScoreCountPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "score finished" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("RESULT_SCORE_FINISHED", 25)]
    public string AudioResultScoreFinishedPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "rate bad" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("RESULT_RATE_BAD", 25)]
    public string AudioResultRateBadPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "rate good" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("RESULT_RATE_GOOD", 25)]
    public string AudioResultRateGoodPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "ready" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("RHYTHM_GAME_READY", 25)]
    public string AudioRhythmGameReadyPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "fail" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("RHYTHM_GAME_FAIL", 25)]
    public string AudioRhythmGameFailPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "clear" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("RHYTHM_GAME_CLEAR", 25)]
    public string AudioRhythmGameClearPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "special clear" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("RHYTHM_GAME_SPECIAL_CLEAR", 25)]
    public string AudioRhythmGameSpecialClearPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "timer warning" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("TIMER_WARNING", 25)]
    public string AudioTimerWarningPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "textbox appear" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("TEXTBOX_APPEAR", 25)]
    public string AudioTextboxAppearPath { get; set; } = "";

    /// <summary>
    /// The local filepath of the "message alert" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("MESSAGE_ALERT", 25)]
    public string AudioMessageAlertPath { get; set; } = "";
    
    /// <summary>
    /// The absolute filepath of the "login" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioLoginPath => AbsolutePath(AudioLoginPath);
    
    /// <summary>
    /// The absolute filepath of the "login card success" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioLoginCardSuccessPath => AbsolutePath(AudioLoginCardSuccessPath);
    
    /// <summary>
    /// The absolute filepath of the "login card error" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioLoginCardErrorPath => AbsolutePath(AudioLoginCardErrorPath);
    
    /// <summary>
    /// The absolute filepath of the "cycle mode" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioCycleModePath => AbsolutePath(AudioCycleModePath);
    
    /// <summary>
    /// The absolute filepath of the "cycle folder" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioCycleFolderPath => AbsolutePath(AudioCycleFolderPath);
    
    /// <summary>
    /// The absolute filepath of the "cycle song" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioCycleSongPath => AbsolutePath(AudioCycleSongPath);
    
    /// <summary>
    /// The absolute filepath of the "cycle option" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioCycleOptionPath => AbsolutePath(AudioCycleOptionPath);
    
    /// <summary>
    /// The absolute filepath of the "select ok" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioSelectOkPath => AbsolutePath(AudioSelectOkPath);
    
    /// <summary>
    /// The absolute filepath of the "select back" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioSelectBackPath => AbsolutePath(AudioSelectBackPath);
    
    /// <summary>
    /// The absolute filepath of the "select denied" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioSelectDeniedPath => AbsolutePath(AudioSelectDeniedPath);
    
    /// <summary>
    /// The absolute filepath of the "select decide" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioSelectDecidePath => AbsolutePath(AudioSelectDecidePath);
    
    /// <summary>
    /// The absolute filepath of the "select preview song" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioSelectPreviewSongPath => AbsolutePath(AudioSelectPreviewSongPath);
    
    /// <summary>
    /// The absolute filepath of the "select start song" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioSelectStartSongPath => AbsolutePath(AudioSelectStartSongPath);
    
    /// <summary>
    /// The absolute filepath of the "select start song alt" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioSelectStartSongAltPath => AbsolutePath(AudioSelectStartSongAltPath);
    
    /// <summary>
    /// The absolute filepath of the "favorite add" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioFavoriteAddPath => AbsolutePath(AudioFavoriteAddPath);
    
    /// <summary>
    /// The absolute filepath of the "favorite remove" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioFavoriteRemovePath => AbsolutePath(AudioFavoriteRemovePath);
    
    /// <summary>
    /// The absolute filepath of the "level up count" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioLevelUpCountPath => AbsolutePath(AudioLevelUpCountPath);
    
    /// <summary>
    /// The absolute filepath of the "level up finished" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioLevelUpFinishedPath => AbsolutePath(AudioLevelUpFinishedPath);
    
    /// <summary>
    /// The absolute filepath of the "score count" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioResultScoreCountPath => AbsolutePath(AudioResultScoreCountPath);
    
    /// <summary>
    /// The absolute filepath of the "score finished" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioResultScoreFinishedPath => AbsolutePath(AudioResultScoreFinishedPath);
    
    /// <summary>
    /// The absolute filepath of the "rate bad" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioResultRateBadPath => AbsolutePath(AudioResultRateBadPath);
    
    /// <summary>
    /// The absolute filepath of the "rate good" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioResultRateGoodPath => AbsolutePath(AudioResultRateGoodPath);
    
    /// <summary>
    /// The absolute filepath of the "ready" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioRhythmGameReadyPath => AbsolutePath(AudioRhythmGameReadyPath);
    
    /// <summary>
    /// The absolute filepath of the "fail" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioRhythmGameFailPath => AbsolutePath(AudioRhythmGameFailPath);
    
    /// <summary>
    /// The absolute filepath of the "clear" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioRhythmGameClearPath => AbsolutePath(AudioRhythmGameClearPath);
    
    /// <summary>
    /// The absolute filepath of the "special clear" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioRhythmGameSpecialClearPath => AbsolutePath(AudioRhythmGameSpecialClearPath);
    
    /// <summary>
    /// The absolute filepath of the "timer warning" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioTimerWarningPath => AbsolutePath(AudioTimerWarningPath);
    
    /// <summary>
    /// The absolute filepath of the "textbox appear" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioTextboxAppearPath => AbsolutePath(AudioTextboxAppearPath);
    
    /// <summary>
    /// The absolute filepath of the "message alert" audio file.
    /// </summary>
    [SctIgnore]
    public string AbsoluteAudioMessageAlertPath => AbsolutePath(AudioMessageAlertPath);
}