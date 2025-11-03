using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Core;

public class Bookmark : ITimeable
{
    public Bookmark(Bookmark cloneSource)
    {
        Timestamp = new(cloneSource.Timestamp.FullTick);
        Color = cloneSource.Color;
        Message = cloneSource.Message;
    }
    
    public Bookmark(Timestamp timestamp, uint color, string message)
    {
        Timestamp = timestamp;
        Color = color;
        Message = message;
    }
    
    public Timestamp Timestamp { get; set; }

    public uint Color { get; set; }
    
    public string Message { get; set; }
}