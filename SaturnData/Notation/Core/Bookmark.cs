using System;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Core;

public class Bookmark : ITimeable, ICloneable
{
    public Bookmark(Timestamp timestamp, uint color, string message)
    {
        Timestamp = timestamp;
        Color = color;
        Message = message;
    }
    
    public Timestamp Timestamp { get; set; }

    public uint Color { get; set; }
    
    public string Message { get; set; }

    public object Clone() => new Bookmark(new(Timestamp), Color, Message);
}