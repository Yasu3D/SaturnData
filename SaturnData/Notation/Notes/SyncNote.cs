using System;
using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A connector that appears between simultaneous notes.
/// </summary>
public sealed class SyncNote : Note, IPositionable, ICloneable
{
    public SyncNote(Timestamp timestamp, int position, int size)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
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
    
    public object Clone() => new SyncNote(new(Timestamp), Position, Size);
}