using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A note hit by holding down within its area at any time.
/// </summary>
public class ChainNote : Note, ITimeable, IPositionable, IPlayable
{
    public ChainNote(ChainNote cloneSource)
    {
        Timestamp = cloneSource.Timestamp;
        Position = cloneSource.Position;
        Size = cloneSource.Size;
        BonusType = cloneSource.BonusType;
        IsJudgeable = cloneSource.IsJudgeable;
        TimingWindow = cloneSource.TimingWindow;
    }
    
    public ChainNote(Timestamp timestamp, int position, int size, BonusType bonusType, bool isJudgeable)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
        BonusType = bonusType;
        IsJudgeable = isJudgeable;
        TimingWindow = new(-4, -4, -4, -4, 4, 4, 4, 4);
    }
    
    public Timestamp Timestamp { get; set; }
    
    public int Position { get; set; }
    
    public int Size { get; set; }

    public TimingWindow TimingWindow { get; set; }
    
    public BonusType BonusType { get; set; }
    
    public bool IsJudgeable { get; set; }
}