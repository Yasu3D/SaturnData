using System;
using SaturnData.Notation.Core;

namespace SaturnData.Content.StageUp;

/// <summary>
/// 
/// </summary>
[Serializable]
public class StageUpStage : ContentItem
{
    /// <summary>
    /// The local filepath of the icon image file to display next to the stage song list, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string IconPath { get; set; } = "";

    /// <summary>
    /// The first song to play.
    /// </summary>
    public StageUpSong Song1 { get; set; } = new();
    
    /// <summary>
    /// The second song to play.
    /// </summary>
    public StageUpSong Song2 { get; set; } = new();
    
    /// <summary>
    /// The third song to play.
    /// </summary>
    public StageUpSong Song3 { get; set; } = new();

    /// <summary>
    /// The threshold where anything below or equal to the specified <see cref="JudgementType"/> will count as an error and subtract health.
    /// </summary>
    public JudgementGrade ErrorThreshold { get; set; } = JudgementGrade.Miss;

    /// <summary>
    /// The number of mistakes a player is allowed to make before failing the stage.
    /// </summary>
    public int Health { get; set; } = 100;

    /// <summary>
    /// The amount of health to recover between songs.
    /// </summary>
    public int HealthRecovery { get; set; } = 10;

    /// <summary>
    /// The absolute filepath of the icon image file to display next to the stage songlist.
    /// </summary>
    public string AbsoluteIconPath => AbsolutePath(IconPath);
    
    /// <summary>
    /// The threshold where a clear counts as a rainbow clear. (100%)
    /// </summary>
    public int RainbowClearThreshold => Health;

    /// <summary>
    /// The threshold where a clear counts as a gold clear. (80%)
    /// </summary>
    public int GoldClearThreshold => (int)Math.Ceiling(Health * 0.8);

    /// <summary>
    /// The threshold where a clear counts as a silver clear. (50%)
    /// </summary>
    public int SilverClearThreshold => (int)Math.Ceiling(Health * 0.5);
}

/// <summary>
/// 
/// </summary>
[Serializable]
public class StageUpSong
{
    /// <summary>
    /// 
    /// </summary>
    public string SongId { get; set; } = "";

    /// <summary>
    /// 
    /// </summary>
    public Difficulty Difficulty { get; set; } = Difficulty.Normal;

    /// <summary>
    /// 
    /// </summary>
    public bool Secret { get; set; } = false;
}