using System.Reflection;

namespace SaturnData.Notation.Serialization;

public struct NotationWriteOptions
{
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

    public NotationWriteOptions()
    {
        ExportWatermark = $"Generated with SaturnData v{Assembly.GetExecutingAssembly().GetName().Version}";
        BakeHoldNotes = true;
        
        
        WriteMerMusicFilePath = WriteMerMusicFilePathOption.NoExtension;
    }

    /// <summary>
    /// A watermark to leave on
    /// </summary>
    public string? ExportWatermark { get; set; }

    /// <summary>
    /// Determines if no-render segments are created when exporting.
    /// </summary>
    public bool BakeHoldNotes { get; set; }

    /// <summary>
    /// <b>Only affects .SATv1 and .SATv2 export!</b><br/>
    /// Determines if layer 0 should be implicit, or explicitly written as <c>.L0</c>.
    /// </summary>
    public bool ExplicitLayerAttributes { get; set; }

    /// <summary>
    /// <b>Only affects .SATv1 and .SATv2 export!</b><br/>
    /// Determines if <c>BonusType.None</c> should be implicit, or explicitly written as <c>.NORMAL</c>.
    /// </summary>
    public bool ExplicitBonusTypeAttributes { get; set; }

    /// <summary>
    /// <b>Only affects .MER export!</b><br/>
    /// Determines how the <c>#MUSIC_FILE_PATH</c> tag is written in a Mer format file.
    /// </summary>
    public WriteMerMusicFilePathOption WriteMerMusicFilePath { get; set; }
    
    
}