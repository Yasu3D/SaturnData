using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A note hit by flicking backwards within its area at the right time.
/// </summary>
public class SnapBackwardNote : Note, IPositionable, IPlayable
{
    public SnapBackwardNote(SnapBackwardNote cloneSource)
    {
        Timestamp = cloneSource.Timestamp;
        Position = cloneSource.Position;
        Size = cloneSource.Size;
        BonusType = cloneSource.BonusType;
        JudgementType = cloneSource.JudgementType;
        TimingWindow = cloneSource.TimingWindow;
    }
    
    public SnapBackwardNote(Timestamp timestamp, int position, int size, BonusType bonusType, JudgementType judgementType)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
        BonusType = bonusType;
        JudgementType = judgementType;
        TimingWindow = new(-10, -10, -7, -1, 1, 5, 8, 10);
    }
    
    public int Position { get; set; }
    
    public int Size { get; set; }

    public TimingWindow TimingWindow { get; set; }
    
    public BonusType BonusType { get; set; }
    
    public JudgementType JudgementType { get; set; }
}