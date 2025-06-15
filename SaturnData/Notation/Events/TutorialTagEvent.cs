using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Events;

public class TutorialTagEvent : Event, ITimeable
{
    public TutorialTagEvent(TutorialTagEvent cloneSource)
    {
        Timestamp = cloneSource.Timestamp;
        Key = cloneSource.Key;
    }
    
    public TutorialTagEvent(Timestamp timestamp, string key)
    {
        Timestamp = timestamp;
        Key = key;
    }
    
    public Timestamp Timestamp { get; set; }
    
    /// <summary>
    /// The key of the tutorial message to display.
    /// </summary>
    public string Key { get; set; }
}