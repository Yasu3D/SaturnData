using System.IO;

namespace SaturnData.Notation.Core;

/// <summary>
/// The file format type and version. Dictates how a chart file is treated.
/// </summary>
public enum FormatVersion
{
    /// <summary>
    /// An unknown, unrecognized, or broken format that can't be parsed.
    /// </summary>
    Unknown = -1,
    
    /// <summary>
    /// Mer format. Legacy support for the original game format.<br/>
    /// </summary>
    Mer = 0,
    
    /// <summary>
    /// First Saturn format. See <see href="https://saturn.yasu3d.art/docs/#/sat_format_1">Saturn Docs</see>.
    /// </summary>
    SatV1 = 1,
    
    /// <summary>
    /// Second Saturn format. See <see href="https://saturn.yasu3d.art/docs/#/sat_format_2">Saturn Docs</see>.
    /// </summary>
    SatV2 = 2,
    
    /// <summary>
    /// Third Saturn format. See <see href="https://saturn.yasu3d.art/docs/#/sat_format_3">Saturn Docs</see>.
    /// </summary>
    SatV3 = 3,
}

/// <summary>
/// The "difficulty category" of a chart.
/// </summary>
/// <remarks>
/// <c>None</c> difficulty is not a valid difficulty for a chart. This enum value is only used as a failsafe in Saturn.
/// </remarks>
public enum Difficulty
{
    None = -1,
    Normal = 0,
    Hard = 1,
    Expert = 2,
    Inferno = 3,
    WorldsEnd = 4,
}

/// <summary>
/// The default background to display in Saturn when playing the chart.
/// </summary>
public enum BackgroundOption
{
    Auto = 0,
    Saturn = 1,
    Version3 = 2,
    Version2 = 3,
    Version1 = 4,
    Boss = 5,
    StageUp = 6,
    WorldsEnd = 7,
    Jacket = 8,
}

/// <summary>
/// Holds all metadata for a chart.
/// </summary>
public class Entry
{
    /// <summary>
    /// Was this entry defined by a <c>.sat</c> file and contains a chart?
    /// </summary>
    public bool Exists;

    /// <summary>
    /// The file format version of the chart.
    /// </summary>
    public FormatVersion FormatVersion;

    /// <summary>
    /// The unique identifier of the chart.
    /// </summary>
    public string Guid = "";

    /// <summary>
    /// The current revision of the chart. Optional and only used for users to keep track of different chart revisions.
    /// </summary>
    public string Revision = "";
    
    /// <summary>
    /// The title of the chart's song.
    /// </summary>
    public string Title = "";
    
    /// <summary>
    /// The kana reading of the title.
    /// </summary>
    public string Reading = "";
    
    /// <summary>
    /// The artist of the chart's song.
    /// </summary>
    public string Artist = "";
    
    /// <summary>
    /// The notes designer of the chart.
    /// </summary>
    public string NotesDesigner = "";

    /// <summary>
    /// The bpm text to display on the song select screen.
    /// </summary>
    public string BpmMessage = "";

    /// <summary>
    /// The default background for the chart.
    /// </summary>
    public BackgroundOption Background = BackgroundOption.Auto;

    /// <summary>
    /// The difficulty category of the chart.
    /// </summary>
    public Difficulty Diff = Difficulty.Normal;

    /// <summary>
    /// The level (or constant) of the chart.
    /// </summary>
    public float Level = 0;

    /// <summary>
    /// The timestamp the chart preview starts at <b>in milliseconds</b>
    /// </summary>
    public float PreviewBegin = 0;

    /// <summary>
    /// The duration of the chart preview <b>in milliseconds</b>
    /// </summary>
    public float PreviewDuration = 10;
    
    /// <summary>
    /// Audio offset <b>in milliseconds</b>
    /// </summary>
    public float AudioOffset = 0;

    /// <summary>
    /// Video offset <b>in milliseconds</b>
    /// </summary>
    public float VideoOffset = 0;

    /// <summary>
    /// Absolute filepath to the chart file that defined this entry.
    /// </summary>
    public string ChartPath = "";

    /// <summary>
    /// Absolute filepath to the background audio file.
    /// </summary>
    public string AudioPath = "";

    /// <summary>
    /// Absolute filepath to the background video file.
    /// </summary>
    public string VideoPath = "";

    /// <summary>
    /// Absolute filepath to the jacket image file.
    /// </summary>
    public string JacketPath = "";

    /// <summary>
    /// Does a file exist at <c>VideoPath</c>?
    /// </summary>
    public bool VideoExists => File.Exists(VideoPath);

    /// <summary>
    /// Returns the integer part of the level, and adds a + if the decimal part is >= 0.7.
    /// </summary>
    /// <code>
    /// 12.6 => "12"
    /// 12.7 => "12+"
    /// 13.2 => "13"
    /// </code>
    public string LevelString => Level < 0
        ? "X"
        : $"{(int)Level}{(Level - (int)Level >= 0.7f ? "+" : "")}";
}