using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A note hit by flicking backwards within its area at the right time.
/// </summary>
public sealed class SnapBackwardNote : Note, IPositionable, IPlayable
{
    public SnapBackwardNote(SnapBackwardNote cloneSource)
    {
        Timestamp = new(cloneSource.Timestamp);
        Position = cloneSource.Position;
        Size = cloneSource.Size;
        BonusType = cloneSource.BonusType;
        JudgementType = cloneSource.JudgementType;
        TimingWindow = new(cloneSource.TimingWindow);
    }
    
    public SnapBackwardNote(Timestamp timestamp, int position, int size, BonusType bonusType, JudgementType judgementType)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
        BonusType = bonusType;
        JudgementType = judgementType;
        TimingWindow = TimingWindowTemplate;
    }
    
    public int Position
    {
        get => position;
        set => position = IPositionable.LimitPosition(value);
    }
    private int position = 0;
    
    public int Size
    {
        get => size;
        set => size = IPositionable.LimitSize(value);
    }
    private int size = 15;

    public TimingWindow TimingWindow { get; set; }
    
    public TimingWindow TimingWindowTemplate => TimingWindow.FromFrames(0, -10, -10, -7, -1, 1, 5, 8, 10);
    
    public BonusType BonusType { get; set; }
    
    public JudgementType JudgementType { get; set; }
}