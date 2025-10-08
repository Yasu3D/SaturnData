using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A note hit by tapping within its area at the right time.
/// </summary>
public sealed class TouchNote : Note, IPositionable, IPlayable
{
    public TouchNote(TouchNote cloneSource)
    {
        Timestamp = cloneSource.Timestamp;
        Position = cloneSource.Position;
        Size = cloneSource.Size;
        BonusType = cloneSource.BonusType;
        JudgementType = cloneSource.JudgementType;
        TimingWindow = cloneSource.TimingWindow;
    }
    
    public TouchNote(Timestamp timestamp, int position, int size, BonusType bonusType, JudgementType judgementType)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
        BonusType = bonusType;
        JudgementType = judgementType;
        TimingWindow = new(-6, -5, -3, -1, 1, 3, 5, 6);
    }
    
    public int Position { get; set; }
    
    public int Size { get; set; }

    public TimingWindow TimingWindow { get; set; }
    
    public BonusType BonusType { get; set; }
    
    public JudgementType JudgementType { get; set; }
}