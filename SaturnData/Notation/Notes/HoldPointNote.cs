using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

public enum HoldPointRenderType
{
    Hidden = 0,
    Visible = 1,
}

/// <summary>
/// A point defining the shape and path of a hold note.
/// </summary>
public sealed class HoldPointNote : Note, IPositionable
{
    public HoldPointNote(Timestamp timestamp, int position, int size, HoldNote parent, HoldPointRenderType renderType)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
        Parent = parent;
        RenderType = renderType;
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
    
    /// <summary>
    /// The parent hold note this hold point note belongs to.
    /// </summary>
    public HoldNote Parent { get; internal set; }
    
    /// <summary>
    /// The render behaviour of the hold point.
    /// </summary>
    public HoldPointRenderType RenderType { get; set; }
}