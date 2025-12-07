using System;
using SaturnData.Content.Cosmetics.Items;
using SaturnData.Content.Items;

namespace SaturnData.Content.Cosmetics;

/// <summary>
/// A set of background songs to play in different scenes.
/// </summary>
[Serializable]
public class SystemMusic : CosmeticItem
{
    /// <summary>
    /// The creator of the <see cref="SystemMusic"/>.
    /// </summary>
    public string Artist { get; set; } = "";

    /// <summary>
    /// The local filepath of the "attract bgm" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioAttractPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "select bgm" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioSelectPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "result bgm" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioResultPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "stage-up select bgm" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioStageUpSelectPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "stage-up secret bgm" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioStageUpSecretPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "see-you bgm" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioSeeYouPath { get; set; } = "";
    
    /// <summary>
    /// The absolute filepath of the "attract bgm" audio file.
    /// </summary>
    public string AbsoluteAudioAttractPath => AbsolutePath(AudioAttractPath);
    
    /// <summary>
    /// The absolute filepath of the "select bgm" audio file.
    /// </summary>
    public string AbsoluteAudioSelectPath => AbsolutePath(AudioSelectPath);
    
    /// <summary>
    /// The absolute filepath of the "result bgm" audio file.
    /// </summary>
    public string AbsoluteAudioResultPath => AbsolutePath(AudioResultPath);
    
    /// <summary>
    /// The absolute filepath of the "stage-up select" bgm audio file.
    /// </summary>
    public string AbsoluteAudioStageUpSelectPath => AbsolutePath(AudioStageUpSelectPath);
    
    /// <summary>
    /// The absolute filepath of the "stage-up secret" bgm audio file.
    /// </summary>
    public string AbsoluteAudioStageUpSecretPath => AbsolutePath(AudioStageUpSecretPath);
    
    /// <summary>
    /// The absolute filepath of the "see-you" bgm audio file.
    /// </summary>
    public string AbsoluteAudioSeeYouPath => AbsolutePath(AudioSeeYouPath);
}