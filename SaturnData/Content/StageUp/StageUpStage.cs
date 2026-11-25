using System;
using SaturnData.Content.Items;
using SaturnData.Content.Items.Attributes;
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
public class StageUpStage : ContentItem
{
    /// <summary>
    /// The local filepath of the icon image file to display next to the stage song list, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctLineBreak]
    [SctKey("IMAGE_PATH", 15)]
    public string IconImagePath { get; set; } = "";

    /// <summary>
    /// The threshold where anything below or equal to the specified <see cref="JudgementType"/> will count as an error and subtract health.
    /// </summary>
    [SctKey("ERROR_THRESHOLD", 15)]
    public ErrorThreshold ErrorThreshold { get; set; } = ErrorThreshold.Miss;

    /// <summary>
    /// The number of mistakes a player is allowed to make before failing the stage.
    /// </summary>
    [SctKey("HEALTH", 15)]
    public int Health { get; set; } = 100;

    /// <summary>
    /// The amount of health to recover between songs.
    /// </summary>
    [SctKey("HEALTH_RECOVERY", 15)]
    public int HealthRecovery { get; set; } = 10;
    
    /// <summary>
    /// The first song to play.
    /// </summary>
    [SctLineBreak]
    [SctKey("SONG_1", 6)]
    public StageUpSong Song1 { get; set; } = new();
    
    /// <summary>
    /// The second song to play.
    /// </summary>
    [SctKey("SONG_2", 6)]
    public StageUpSong Song2 { get; set; } = new();
    
    /// <summary>
    /// The third song to play.
    /// </summary>
    [SctKey("SONG_3", 6)]
    public StageUpSong Song3 { get; set; } = new();

    /// <summary>
    /// The local filepath of the emblem image file to display on a blue clear, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctLineBreak]
    [SctKey("EMBLEM_BLUE_PATH", 19)]
    public string EmblemBlueImagePath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the emblem image file to display on a silver clear, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("EMBLEM_SILVER_PATH", 19)]
    public string EmblemSilverImagePath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the emblem image file to display on a gold clear, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("EMBLEM_GOLD_PATH", 19)]
    public string EmblemGoldImagePath { get; set; } = "";
    
    /// <summary>
    /// The local filepath of the emblem image file to display on a rainbow clear, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctKey("EMBLEM_RAINBOW_PATH", 19)]
    public string EmblemRainbowImagePath { get; set; } = "";

    /// <summary>
    /// The absolute filepath of the icon image file to display next to the stage songlist.
    /// </summary>
    [SctIgnore]
    public string AbsoluteIconImagePath => AbsolutePath(IconImagePath);
    
    /// <summary>
    /// The absolute filepath of the icon image file to display next to the stage songlist.
    /// </summary>
    [SctIgnore]
    public string AbsoluteEmblemBlueImagePath => AbsolutePath(EmblemBlueImagePath);
        
    /// <summary>
    /// The absolute filepath of the icon image file to display next to the stage songlist.
    /// </summary>
    [SctIgnore]
    public string AbsoluteEmblemSilverImagePath => AbsolutePath(EmblemSilverImagePath);
            
    /// <summary>
    /// The absolute filepath of the icon image file to display next to the stage songlist.
    /// </summary>
    [SctIgnore]
    public string AbsoluteEmblemGoldImagePath => AbsolutePath(EmblemGoldImagePath);
                
    /// <summary>
    /// The absolute filepath of the icon image file to display next to the stage songlist.
    /// </summary>
    [SctIgnore]
    public string AbsoluteEmblemRainbowImagePath => AbsolutePath(EmblemRainbowImagePath);

    /// <summary>
    /// The threshold where a clear counts as a gold clear. (80%)
    /// </summary>
    [SctIgnore]
    public int GoldClearThreshold => (int)Math.Ceiling(Health * 0.8);

    /// <summary>
    /// The threshold where a clear counts as a silver clear. (50%)
    /// </summary>
    [SctIgnore]
    public int SilverClearThreshold => (int)Math.Ceiling(Health * 0.5);

    /// <summary>
    /// Returns the <see cref="StageUpSong"/> by its zero-indexed Id.
    /// </summary>
    /// <param name="index">The index of the Song [0, 1, 2]</param>
    public StageUpSong SongByIndex(int index)
    {
        if (index == 0) return Song1;
        if (index == 1) return Song2;
        if (index == 2) return Song3;

        return Song1;
    }

    public override string[] GetAllExternalFilePaths()
    {
        return
        [
            AbsoluteIconImagePath,
            AbsoluteEmblemBlueImagePath,
            AbsoluteEmblemSilverImagePath,
            AbsoluteEmblemGoldImagePath,
            AbsoluteEmblemRainbowImagePath,
        ];
    }
}