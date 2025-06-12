namespace SaturnData.Notation.Serialization;

public struct NotationReadOptions
{
    public NotationReadOptions()
    {
        TrimHoldNotes = true;
    }
    
    /// <summary>
    /// Determines if no-render segments are removed when importing.
    /// </summary>
    public bool TrimHoldNotes { get; set; }
}