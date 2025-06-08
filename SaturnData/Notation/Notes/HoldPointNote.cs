using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A point defining the shape and path of a hold note.
/// </summary>
public class HoldPointNote : Note, ITimeable, IPositionable, ILayerable
{
     public HoldPointNote(HoldPointNote source)
    {
        Timestamp = source.Timestamp;
        Position = source.Position;
        Size = source.Size;
        Layer = source.Layer;
    }
    
    public HoldPointNote(Timestamp timestamp, int position, int size, int layer)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
        Layer = layer;
    }
    
    public Timestamp Timestamp { get; set; }
    
    public int Position { get; set; }
    
    public int Size { get; set; }
    
    public int Layer { get; set; }
}