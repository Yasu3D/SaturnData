using SaturnData.Notation.Core;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A measure line to help keep time.
/// </summary>
public sealed class MeasureLineNote : Note
{
    public MeasureLineNote(MeasureLineNote cloneSource)
    {
        Timestamp = cloneSource.Timestamp;
        IsBeatLine = cloneSource.IsBeatLine;
    }
    
    public MeasureLineNote(Timestamp timestamp, bool isBeatLine)
    {
        Timestamp = timestamp;
        IsBeatLine = isBeatLine;
    }

    /// <summary>
    /// Determines if the measure line indicates a beat.
    /// </summary>
    public bool IsBeatLine { get; set; }
}