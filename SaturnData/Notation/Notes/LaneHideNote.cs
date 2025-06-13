using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A note that hides the tunnel lanes it occupies once it reaches the judgement line.
/// </summary>
public class LaneHideNote : Note, ITimeable, IPositionable, ILaneToggle
{
    public LaneHideNote(LaneHideNote cloneSource)
    {
        Timestamp = cloneSource.Timestamp;
        Position = cloneSource.Position;
        Size = cloneSource.Size;
        Direction = cloneSource.Direction;
    }
    
    public LaneHideNote(Timestamp timestamp, int position, int size, LaneSweepDirection direction)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
        Direction = direction;
    }
    
    public Timestamp Timestamp { get; set; }
    
    public int Position { get; set; }
    
    public int Size { get; set; }
    
    public LaneSweepDirection Direction { get; set; }
}