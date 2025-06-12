using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Core;

public class Bookmark : ITimeable
{
    public Bookmark(Timestamp timestamp, string color, string message)
    {
        Timestamp = timestamp;
        Color = color;
        Message = message;
    }
    
    public Timestamp Timestamp { get; set; }

    public string Color { get; set; }
    
    public string Message { get; set; }
}