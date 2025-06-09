using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A note hit by tapping within its area at the right time.
/// </summary>
public class TouchNote : Note, ITimeable, IPositionable, IPlayable
{
    public TouchNote(TouchNote cloneSource)
    {
        Timestamp = cloneSource.Timestamp;
        Position = cloneSource.Position;
        Size = cloneSource.Size;
        BonusType = cloneSource.BonusType;
        IsJudgeable = cloneSource.IsJudgeable;
        TimingWindow = cloneSource.TimingWindow;
    }
    
    public TouchNote(Timestamp timestamp, int position, int size, BonusType bonusType, bool isJudgeable)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
        BonusType = bonusType;
        IsJudgeable = isJudgeable;
        TimingWindow = new(-6, -5, -3, -1, 1, 3, 5, 6);
    }
    
    public Timestamp Timestamp { get; set; }
    
    public int Position { get; set; }
    
    public int Size { get; set; }

    public TimingWindow TimingWindow { get; set; }
    
    public BonusType BonusType { get; set; }
    
    public bool IsJudgeable { get; set; }
}