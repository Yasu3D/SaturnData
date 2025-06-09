using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A note hit by sliding clockwise within its area at the right time.
/// </summary>
public class SlideClockwiseNote : Note, ITimeable, IPositionable, IPlayable
{
    public SlideClockwiseNote(SlideClockwiseNote cloneSource)
    {
        Timestamp = cloneSource.Timestamp;
        Position = cloneSource.Position;
        Size = cloneSource.Size;
        BonusType = cloneSource.BonusType;
        IsJudgeable = cloneSource.IsJudgeable;
        TimingWindow = cloneSource.TimingWindow;
    }
    
    public SlideClockwiseNote(Timestamp timestamp, int position, int size, BonusType bonusType, bool isJudgeable)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
        BonusType = bonusType;
        IsJudgeable = isJudgeable;
        TimingWindow = new(-10, -8, -5, -1, 1, 7, 10, 10);
    }
    
    public Timestamp Timestamp { get; set; }
    
    public int Position { get; set; }
    
    public int Size { get; set; }

    public TimingWindow TimingWindow { get; set; }
    
    public BonusType BonusType { get; set; }
    
    public bool IsJudgeable { get; set; }
}