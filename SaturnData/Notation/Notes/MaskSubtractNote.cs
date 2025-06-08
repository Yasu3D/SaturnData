using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A note that hides the tunnel lanes it occupies once it reaches the judgement line.
/// </summary>
public class MaskSubtractNote : Note, ITimeable, IPositionable
{
    public MaskSubtractNote(MaskSubtractNote source)
    {
        Timestamp = source.Timestamp;
        Position = source.Position;
        Size = source.Size;
    }
    
    public MaskSubtractNote(Timestamp timestamp, int position, int size)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
    }
    
    public Timestamp Timestamp { get; set; }
    
    public int Position { get; set; }
    
    public int Size { get; set; }
}