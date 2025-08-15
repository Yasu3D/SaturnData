using System.Reflection;

namespace SaturnData.Notation.Serialization;

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

public enum WriteMerMusicFilePathOption
    {
        /// <summary>
        /// Don't write the file name at all.
        /// </summary>
        /// <code>
        /// AudioPath = "MUSIC_FILE.wav"
        /// Output = "#MUSIC_FILE_PATH MUSIC_FILE" 
        /// </code>
        None = 0,

        /// <summary>
        /// Write the file name without the extension.
        /// </summary>
        /// <code>
        /// AudioPath = "MUSIC_FILE.wav"
        /// Output = "#MUSIC_FILE_PATH MUSIC_FILE" 
        /// </code>
        NoExtension = 1,

        /// <summary>
        /// Write the file name with the extension.
        /// </summary>
        /// <code>
        /// AudioPath = "MUSIC_FILE.wav"
        /// Output = "#MUSIC_FILE_PATH MUSIC_FILE.wav" 
        /// </code>
        WithExtension = 2,
    }

public class NotationWriteArgs
{
    /// <summary>
    /// A watermark to write at the beginning of the chart file.
    /// </summary>
    public string? ExportWatermark { get; set; } = $"Generated with SaturnData v{Assembly.GetExecutingAssembly().GetName().Version}";

    /// <summary>
    /// The file format type and version to write the file in.
    /// </summary>
    public FormatVersion FormatVersion { get; set; } = FormatVersion.SatV3;

    
    
    /// <summary>
    /// <b>Only affects .MER export!</b><br/>
    /// Determines how the <c>#MUSIC_FILE_PATH</c> tag is written in a Mer format file.
    /// </summary>
    public WriteMerMusicFilePathOption WriteMerMusicFilePath { get; set; } = WriteMerMusicFilePathOption.NoExtension;
    
    
    
    /// <summary>
    /// Determines if hidden interpolated segments are created when exporting.
    /// </summary>
    public bool BakeHoldNotes { get; set; } = false;

    /// <summary>
    /// Determines if lane toggle notes with <c>SweepDirection.Instant</c>
    /// </summary>
    public bool BakeInstantLaneToggleNotes { get; set; } = false;

    /// <summary>
    /// <b>Only affects .SATv1 and .SATv2 export!</b><br/>
    /// Determines if layer 0 should be implicit, or explicitly written as <c>.L0</c>.
    /// </summary>
    public bool ExplicitLayerAttributes { get; set; } = false;

    /// <summary>
    /// <b>Only affects .SATv1 and .SATv2 export!</b><br/>
    /// Determines if <c>BonusType.Normal</c> should be implicit, or explicitly written as <c>.NORMAL</c>.
    /// </summary>
    public bool ExplicitBonusTypeAttributes { get; set; } = false;
}