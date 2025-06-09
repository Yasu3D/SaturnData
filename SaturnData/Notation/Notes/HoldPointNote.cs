using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A point defining the shape and path of a hold note.
/// </summary>
public class HoldPointNote : Note, ITimeable, IPositionable
{
    public HoldPointNote(Timestamp timestamp, int position, int size, HoldNote parent)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
        Parent = parent;
    }
    
    public Timestamp Timestamp { get; set; }
    
    public int Position { get; set; }
    
    public int Size { get; set; }
    
    public HoldNote Parent { get; }
}