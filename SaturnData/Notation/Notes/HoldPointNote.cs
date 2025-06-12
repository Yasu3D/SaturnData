using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

public enum HoldPointRenderBehaviour
{
    Hidden = 0,
    Visible = 1,
}

/// <summary>
/// A point defining the shape and path of a hold note.
/// </summary>
public class HoldPointNote : Note, ITimeable, IPositionable
{
    internal HoldPointNote(Timestamp timestamp, int position, int size, HoldPointRenderBehaviour renderBehaviour)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
        RenderBehaviour = renderBehaviour;
        Parent = null!;
    }
    
    public HoldPointNote(Timestamp timestamp, int position, int size, HoldNote parent, HoldPointRenderBehaviour renderBehaviour)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
        Parent = parent;
        RenderBehaviour = renderBehaviour;
    }
    
    public Timestamp Timestamp { get; set; }
    
    public int Position { get; set; }
    
    public int Size { get; set; }
    
    /// <summary>
    /// The parent hold note this hold point note belongs to.
    /// </summary>
    public HoldNote Parent { get; internal set; }
    
    /// <summary>
    /// The render behaviour of the hold point.
    /// </summary>
    public HoldPointRenderBehaviour RenderBehaviour { get; set; }
}