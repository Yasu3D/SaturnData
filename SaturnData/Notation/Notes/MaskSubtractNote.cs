using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A note that hides the tunnel lanes it occupies once it reaches the judgement line.
/// </summary>
public class MaskSubtractNote : Note, ITimeable, IPositionable, IMask
{
    public MaskSubtractNote(MaskSubtractNote cloneSource)
    {
        Timestamp = cloneSource.Timestamp;
        Position = cloneSource.Position;
        Size = cloneSource.Size;
        Direction = cloneSource.Direction;
    }
    
    public MaskSubtractNote(Timestamp timestamp, int position, int size, MaskDirection direction)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
        Direction = direction;
    }
    
    public Timestamp Timestamp { get; set; }
    
    public int Position { get; set; }
    
    public int Size { get; set; }
    
    public MaskDirection Direction { get; set; }
}