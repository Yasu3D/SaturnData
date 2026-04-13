using System;
using SaturnData.Content.Items;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Content.StageUp;

public enum ErrorThreshold
{
    Miss = 1,
    GoodOrBelow = 2,
    GreatOrBelow = 3,
}

/// <summary>
/// A set of three songs to play in stage-up mode.
/// </summary>
[Serializable]
public class StageUpStage : ContentItem
{
    /// <summary>
    /// The local filepath of the icon image file to display next to the stage song list, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string ImagePath { get; set; } = "";

    /// <summary>
    /// The threshold where anything below or equal to the specified <see cref="JudgementType"/> will count as an error and subtract health.
    /// </summary>
    public ErrorThreshold ErrorThreshold { get; set; } = ErrorThreshold.Miss;

    /// <summary>
    /// The number of mistakes a player is allowed to make before failing the stage.
    /// </summary>
    public int Health { get; set; } = 100;

    /// <summary>
    /// The amount of health to recover between songs.
    /// </summary>
    public int HealthRecovery { get; set; } = 10;
    
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
    /// The absolute filepath of the icon image file to display next to the stage songlist.
    /// </summary>
    public string AbsoluteIconPath => AbsolutePath(ImagePath);
    
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

    /// <summary>
    /// Returns the <see cref="StageUpSong"/> by its zero-indexed Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public StageUpSong SongById(int id)
    {
        if (id == 0) return Song1;
        if (id == 1) return Song2;
        if (id == 2) return Song3;

        return Song1;
    }
}