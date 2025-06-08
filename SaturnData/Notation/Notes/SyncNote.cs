using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A connector that appears between simultaneous notes.
/// </summary>
public class SyncNote : Note, ITimeable, IPositionable, ILayerable
{
    public SyncNote(SyncNote source)
    {
        Timestamp = source.Timestamp;
        Position = source.Position;
        Size = source.Size;
    }
    
    public SyncNote(Timestamp timestamp, int position, int size, int layer)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
    }
    
    public Timestamp Timestamp { get; set; }
    
    public int Position { get; set; }
    
    public int Size { get; set; }
    
    public int Layer { get; set; }
}