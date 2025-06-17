using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A note hit by sliding counterclockwise within its area at the right time.
/// </summary>
public class SlideCounterclockwiseNote : Note, ITimeable, IPositionable, IPlayable
{
    public SlideCounterclockwiseNote(SlideCounterclockwiseNote cloneSource)
    {
        Timestamp = cloneSource.Timestamp;
        Position = cloneSource.Position;
        Size = cloneSource.Size;
        BonusType = cloneSource.BonusType;
        JudgementType = cloneSource.JudgementType;
        TimingWindow = cloneSource.TimingWindow;
    }
    
    public SlideCounterclockwiseNote(Timestamp timestamp, int position, int size, BonusType bonusType, JudgementType judgementType)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
        BonusType = bonusType;
        JudgementType = judgementType;
        TimingWindow = new(-10, -8, -5, -1, 1, 7, 10, 10);
    }
    
    public Timestamp Timestamp { get; set; }
    
    public int Position { get; set; }
    
    public int Size { get; set; }

    public TimingWindow TimingWindow { get; set; }
    
    public BonusType BonusType { get; set; }
    
    public JudgementType JudgementType { get; set; } 
}