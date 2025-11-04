using System;

namespace SaturnData.Content.Cosmetics;

/// <summary>
/// A set of menu sound effects to play on UI interactions.
/// </summary>
[Serializable]
public class SystemSound : ContentItem
{
    /// <summary>
    /// The creator of the <see cref="SystemSound"/>.
    /// </summary>
    public string Artist { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "login" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioLoginPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "cycle mode" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioCycleModePath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "cycle folder" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioCycleFolderPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "cycle song" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioCycleSongPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "cycle option" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioCycleOptionPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "select ok" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioSelectOkPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "select back" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioSelectBackPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "select denied" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioSelectDeniedPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "select decide" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioSelectDecidePath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "select preview song" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioSelectPreviewSongPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "select start song" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioSelectStartSongPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "select start song alt" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioSelectStartSongAltPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "favorite add" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioFavoriteAddPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "favorite remove" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioFavoriteRemovePath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "score count" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioResultScoreCountPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "score finished" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioResultScoreFinishedPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "rate bad" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioResultRateBadPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "rate good" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioResultRateGoodPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "ready" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioRhythmGameReadyPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "fail" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioRhythmGameFailPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "clear" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioRhythmGameClearPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "special clear" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioRhythmGameSpecialClearPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "timer warning" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioTimerWarningPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "textbox appear" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioTextboxAppearPath { get; set; } = "";
    
    /// <summary>
    /// The absolute filepath of the "login" audio file.
    /// </summary>
    public string AbsoluteAudioLoginPath => AbsolutePath(AudioLoginPath);
    
    /// <summary>
    /// The absolute filepath of the "cycle mode" audio file.
    /// </summary>
    public string AbsoluteAudioCycleModePath => AbsolutePath(AudioCycleModePath);
    
    /// <summary>
    /// The absolute filepath of the "cycle folder" audio file.
    /// </summary>
    public string AbsoluteAudioCycleFolderPath => AbsolutePath(AudioCycleFolderPath);
    
    /// <summary>
    /// The absolute filepath of the "cycle song" audio file.
    /// </summary>
    public string AbsoluteAudioCycleSongPath => AbsolutePath(AudioCycleSongPath);
    
    /// <summary>
    /// The absolute filepath of the "cycle option" audio file.
    /// </summary>
    public string AbsoluteAudioCycleOptionPath => AbsolutePath(AudioCycleOptionPath);
    
    /// <summary>
    /// The absolute filepath of the "select ok" audio file.
    /// </summary>
    public string AbsoluteAudioSelectOkPath => AbsolutePath(AudioSelectOkPath);
    
    /// <summary>
    /// The absolute filepath of the "select back" audio file.
    /// </summary>
    public string AbsoluteAudioSelectBackPath => AbsolutePath(AudioSelectBackPath);
    
    /// <summary>
    /// The absolute filepath of the "select denied" audio file.
    /// </summary>
    public string AbsoluteAudioSelectDeniedPath => AbsolutePath(AudioSelectDeniedPath);
    
    /// <summary>
    /// The absolute filepath of the "select decide" audio file.
    /// </summary>
    public string AbsoluteAudioSelectDecidePath => AbsolutePath(AudioSelectDecidePath);
    
    /// <summary>
    /// The absolute filepath of the "select preview song" audio file.
    /// </summary>
    public string AbsoluteAudioSelectPreviewSongPath => AbsolutePath(AudioSelectPreviewSongPath);
    
    /// <summary>
    /// The absolute filepath of the "select start song" audio file.
    /// </summary>
    public string AbsoluteAudioSelectStartSongPath => AbsolutePath(AudioSelectStartSongPath);
    
    /// <summary>
    /// The absolute filepath of the "select start song alt" audio file.
    /// </summary>
    public string AbsoluteAudioSelectStartSongAltPath => AbsolutePath(AudioSelectStartSongAltPath);
    
    /// <summary>
    /// The absolute filepath of the "favorite add" audio file.
    /// </summary>
    public string AbsoluteAudioFavoriteAddPath => AbsolutePath(AudioFavoriteAddPath);
    
    /// <summary>
    /// The absolute filepath of the "favorite remove" audio file.
    /// </summary>
    public string AbsoluteAudioFavoriteRemovePath => AbsolutePath(AudioFavoriteRemovePath);
    
    /// <summary>
    /// The absolute filepath of the "score count" audio file.
    /// </summary>
    public string AbsoluteAudioResultScoreCountPath => AbsolutePath(AudioResultScoreCountPath);
    
    /// <summary>
    /// The absolute filepath of the "score finished" audio file.
    /// </summary>
    public string AbsoluteAudioResultScoreFinishedPath => AbsolutePath(AudioResultScoreFinishedPath);
    
    /// <summary>
    /// The absolute filepath of the "rate bad" audio file.
    /// </summary>
    public string AbsoluteAudioResultRateBadPath => AbsolutePath(AudioResultRateBadPath);
    
    /// <summary>
    /// The absolute filepath of the "rate good" audio file.
    /// </summary>
    public string AbsoluteAudioResultRateGoodPath => AbsolutePath(AudioResultRateGoodPath);
    
    /// <summary>
    /// The absolute filepath of the "ready" audio file.
    /// </summary>
    public string AbsoluteAudioRhythmGameReadyPath => AbsolutePath(AudioRhythmGameReadyPath);
    
    /// <summary>
    /// The absolute filepath of the "fail" audio file.
    /// </summary>
    public string AbsoluteAudioRhythmGameFailPath => AbsolutePath(AudioRhythmGameFailPath);
    
    /// <summary>
    /// The absolute filepath of the "clear" audio file.
    /// </summary>
    public string AbsoluteAudioRhythmGameClearPath => AbsolutePath(AudioRhythmGameClearPath);
    
    /// <summary>
    /// The absolute filepath of the "special clear" audio file.
    /// </summary>
    public string AbsoluteAudioRhythmGameSpecialClearPath => AbsolutePath(AudioRhythmGameSpecialClearPath);
    
    /// <summary>
    /// The absolute filepath of the "timer warning" audio file.
    /// </summary>
    public string AbsoluteAudioTimerWarningPath => AbsolutePath(AudioTimerWarningPath);
    
    /// <summary>
    /// The absolute filepath of the "textbox appear" audio file.
    /// </summary>
    public string AbsoluteAudioTextboxAppearPath => AbsolutePath(AudioTextboxAppearPath);
}