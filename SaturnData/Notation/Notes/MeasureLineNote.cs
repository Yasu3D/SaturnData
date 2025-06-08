using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A measure line that normally appears every measure.
/// </summary>
public class MeasureLineNote : Note, ITimeable
{
    public MeasureLineNote(MeasureLineNote source)
    {
        Timestamp = source.Timestamp;
    }
    
    public MeasureLineNote(Timestamp timestamp)
    {
        Timestamp = timestamp;
    }
    
    public Timestamp Timestamp { get; set; }
}