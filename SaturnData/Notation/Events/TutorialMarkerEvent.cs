using SaturnData.Notation.Core;

namespace SaturnData.Notation.Events;

public sealed class TutorialMarkerEvent : Event
{
    public TutorialMarkerEvent(TutorialMarkerEvent cloneSource)
    {
        Timestamp = cloneSource.Timestamp;
        Key = cloneSource.Key;
    }
    
    public TutorialMarkerEvent(Timestamp timestamp, string key)
    {
        Timestamp = timestamp;
        Key = key;
    }
    
    /// <summary>
    /// The key of the tutorial message to display.
    /// </summary>
    public string Key { get; set; }
}