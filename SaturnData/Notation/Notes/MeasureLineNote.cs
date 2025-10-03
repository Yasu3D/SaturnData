using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A measure line to help keep time.
/// </summary>
public class MeasureLineNote : Note
{
    public MeasureLineNote(MeasureLineNote cloneSource)
    {
        Timestamp = cloneSource.Timestamp;
    }
    
    public MeasureLineNote(Timestamp timestamp)
    {
        Timestamp = timestamp;
    }
}