using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A note that shows the tunnel lanes it occupies once it reaches the judgement line.
/// </summary>
public sealed class LaneShowNote : Note, IPositionable, ILaneToggle
{
    public LaneShowNote(LaneShowNote cloneSource)
    {
        Timestamp = new(cloneSource.Timestamp);
        Position = cloneSource.Position;
        Size = cloneSource.Size;
        Direction = cloneSource.Direction;
    }
    
    public LaneShowNote(Timestamp timestamp, int position, int size, LaneSweepDirection direction)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
        Direction = direction;
    }
    
    public int Position
    {
        get => position;
        set => position = IPositionable.LimitPosition(value);
    }
    private int position = 0;
    
    public int Size
    {
        get => size;
        set => size = IPositionable.LimitSize(value);
    }
    private int size = 15;
    
    public LaneSweepDirection Direction { get; set; }
}