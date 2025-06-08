using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A note that makes the tunnel lanes it occupies visible once it reaches the judgement line.
/// </summary>
public class MaskAddNote : Note, ITimeable, IPositionable
{
    public MaskAddNote(MaskAddNote source)
    {
        Timestamp = source.Timestamp;
        Position = source.Position;
        Size = source.Size;
    }
    
    public MaskAddNote(Timestamp timestamp, int position, int size)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
    }
    
    public Timestamp Timestamp { get; set; }
    
    public int Position { get; set; }
    
    public int Size { get; set; }
}