using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A connector that appears between simultaneous notes.
/// </summary>
public class SyncNote : Note, IPositionable
{
    public SyncNote(SyncNote cloneSource)
    {
        Timestamp = cloneSource.Timestamp;
        Position = cloneSource.Position;
        Size = cloneSource.Size;
    }
    
    public SyncNote(Timestamp timestamp, int position, int size)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
    }
    
    public int Position { get; set; }
    
    public int Size { get; set; }
}