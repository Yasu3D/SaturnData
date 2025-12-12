using System.Reflection;

namespace SaturnData.Notation.Serialization;

/// <summary>
/// The file format type and version. Dictates how a chart file is read.
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

public enum ConvertFakeNotesOption
{
    ExcludeFromExport = 0,
    ConvertToNormalNotes = 1,
}

public enum ConvertAutoplayNotesOption
{
    ExcludeFromExport = 0,
    ConvertToNormalNotes = 1,
}

public enum MergeExtraLayersOption
{
    ExcludeFromExport = 0,
    MergeIntoMainLayer = 1,
    MergeIntoMainLayerWithoutEvents = 2,
}

public enum ConvertExtendedBonusTypesOption
{
    ConvertToNormal = 0,
    ConvertToR = 1,
}

public enum WriteMusicFilePathOption
{
    /// <summary>
    /// Don't write the file name at all.
    /// </summary>
    /// <code>
    /// AudioPath = "MUSIC_FILE.wav"
    /// Output = "#MUSIC_FILE_PATH" 
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
    /// The file format type and version to write the file in.
    /// </summary>
    public FormatVersion FormatVersion { get; set; } = FormatVersion.SatV3;
    
    /// <summary>
    /// A watermark to write at the beginning of the chart file.
    /// </summary>
    public string? ExportWatermark { get; set; } = $"Generated with SaturnData v{Assembly.GetExecutingAssembly().GetName().Version}";
    
    public ConvertFakeNotesOption ConvertFakeNotes { get; set; } = ConvertFakeNotesOption.ExcludeFromExport;

    public ConvertAutoplayNotesOption ConvertAutoplayNotes { get; set; } = ConvertAutoplayNotesOption.ExcludeFromExport;

    public MergeExtraLayersOption MergeExtraLayers { get; set; } = MergeExtraLayersOption.MergeIntoMainLayerWithoutEvents;

    public ConvertExtendedBonusTypesOption ConvertExtendedBonusTypes { get; set; } = ConvertExtendedBonusTypesOption.ConvertToNormal;
    
    /// <summary>
    /// <b>Only affects .MER export!</b><br/>
    /// Determines how the <c>#MUSIC_FILE_PATH</c> tag is written in a Mer format file.
    /// </summary>
    public WriteMusicFilePathOption WriteMusicFilePath { get; set; } = WriteMusicFilePathOption.NoExtension;
}