using System;
using SaturnData.Content.Cosmetics.Items;
using SaturnData.Content.Items;

namespace SaturnData.Content.Cosmetics;

/// <summary>
/// A set of note sound effects, also known as "Hitsounds".
/// </summary>
[Serializable]
public class NoteSound : CosmeticItem
{
    /// <summary>
    /// The creator of the <see cref="NoteSound"/>.
    /// </summary>
    public string Artist { get; set; } = "";

    /// <summary>
    /// The time in milliseconds to loop back to, once <see cref="HoldLoopEndTime"/> is reached.
    /// </summary>
    public float HoldLoopStartTime { get; set; } = 0;
    
    /// <summary>
    /// The time in milliseconds at which to loop back to <see cref="HoldLoopEndTime"/>.
    /// </summary>
    public float HoldLoopEndTime { get; set; } = 0;
    
    /// <summary>
    /// The time in milliseconds to loop back to, once <see cref="ReHoldLoopEndTime"/> is reached.
    /// </summary>
    public float ReHoldLoopStartTime { get; set; } = 0;
    
    /// <summary>
    /// The time in milliseconds at which to loop back to <see cref="ReHoldLoopEndTime"/>.
    /// </summary>
    public float ReHoldLoopEndTime { get; set; } = 0;

    /// <summary>
    /// The local filepath of the "touch marvelous" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioTouchMarvelousPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "touch good" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioTouchGoodPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "slide marvelous" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioSlideMarvelousPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "slide good" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioSlideGoodPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "snap marvelous" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioSnapMarvelousPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "snap good" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioSnapGoodPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "hold" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioHoldPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "re-hold" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioReHoldPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "chain" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioChainPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "bonus" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioBonusPath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the "r" audio file, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string AudioRPath { get; set; } = "";
    
    /// <summary>
    /// The absolute filepath of the "touch marvelous" audio file.
    /// </summary>
    public string AbsoluteAudioTouchMarvelousPath => AbsolutePath(AudioTouchMarvelousPath);
    
    /// <summary>
    /// The absolute filepath of the "touch good" audio file.
    /// </summary>
    public string AbsoluteAudioTouchGoodPath => AbsolutePath(AudioTouchGoodPath);
    
    /// <summary>
    /// The absolute filepath of the "slide marvelous" audio file.
    /// </summary>
    public string AbsoluteAudioSlideMarvelousPath => AbsolutePath(AudioSlideMarvelousPath);
    
    /// <summary>
    /// The absolute filepath of the "slide good" audio file.
    /// </summary>
    public string AbsoluteAudioSlideGoodPath => AbsolutePath(AudioSlideGoodPath);
    
    /// <summary>
    /// The absolute filepath of the "snap marvelous" audio file.
    /// </summary>
    public string AbsoluteAudioSnapMarvelousPath => AbsolutePath(AudioSnapMarvelousPath);
    
    /// <summary>
    /// The absolute filepath of the "snap good" audio file.
    /// </summary>
    public string AbsoluteAudioSnapGoodPath => AbsolutePath(AudioSnapGoodPath);
    
    /// <summary>
    /// The absolute filepath of the "hold" audio file.
    /// </summary>
    public string AbsoluteAudioHoldPath => AbsolutePath(AudioHoldPath);
    
    /// <summary>
    /// The absolute filepath of the "re-hold" audio file.
    /// </summary>
    public string AbsoluteAudioReHoldPath => AbsolutePath(AudioReHoldPath);
    
    /// <summary>
    /// The absolute filepath of the "chain" audio file.
    /// </summary>
    public string AbsoluteAudioChainPath => AbsolutePath(AudioChainPath);
    
    /// <summary>
    /// The absolute filepath of the "bonus" audio file.
    /// </summary>
    public string AbsoluteAudioBonusPath => AbsolutePath(AudioBonusPath);
    
    /// <summary>
    /// The absolute filepath of the "r" audio file.
    /// </summary>
    public string AbsoluteAudioRPath => AbsolutePath(AudioRPath);
}