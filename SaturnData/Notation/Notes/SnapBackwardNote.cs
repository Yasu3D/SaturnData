using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A note hit by sliding backwards within its area at the right time.
/// </summary>
public class SnapBackwardNote : Note, ITimeable, IPositionable, IPlayable
{
    public SnapBackwardNote(SnapBackwardNote cloneSource)
    {
        Timestamp = cloneSource.Timestamp;
        Position = cloneSource.Position;
        Size = cloneSource.Size;
        BonusType = cloneSource.BonusType;
        IsJudgeable = cloneSource.IsJudgeable;
        TimingWindow = cloneSource.TimingWindow;
    }
    
    public SnapBackwardNote(Timestamp timestamp, int position, int size, BonusType bonusType, bool isJudgeable)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
        BonusType = bonusType;
        IsJudgeable = isJudgeable;
        TimingWindow = new(-10, -10, -7, -1, 1, 5, 8, 10);
    }
    
    public Timestamp Timestamp { get; set; }
    
    public int Position { get; set; }
    
    public int Size { get; set; }

    public TimingWindow TimingWindow { get; set; }
    
    public BonusType BonusType { get; set; }
    
    public bool IsJudgeable { get; set; }
}