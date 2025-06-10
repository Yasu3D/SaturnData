namespace SaturnData.Notation.Serialization;

public struct NotationSerializerOptions
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

    public NotationSerializerOptions()
    {
        WriteMerMusicFilePath = WriteMerMusicFilePathOption.NoExtension;
        TrimHoldNotes = true;
        BakeHoldNotes = true;
    }

    /// <summary>
    /// Determines how the <c>#MUSIC_FILE_PATH</c> tag is written in a Mer format file.
    /// </summary>
    public WriteMerMusicFilePathOption WriteMerMusicFilePath { get; set; }
    
    /// <summary>
    /// Determines if no-render segments are removed when importing.
    /// </summary>
    public bool TrimHoldNotes { get; set; }
    
    /// <summary>
    /// Determines if no-render segments are created when exporting.
    /// </summary>
    public bool BakeHoldNotes { get; set; }
}