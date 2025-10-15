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
        Timestamp = new(cloneSource.Timestamp);
        Position = cloneSource.Position;
        Size = cloneSource.Size;
        BonusType = cloneSource.BonusType;
        JudgementType = cloneSource.JudgementType;
        TimingWindow = new(cloneSource.TimingWindow);
    }
    
    public TouchNote(Timestamp timestamp, int position, int size, BonusType bonusType, JudgementType judgementType)
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
    
    public TimingWindow TimingWindowTemplate => TimingWindow.FromFrames(0, -6, -5, -3, -1, 1, 3, 5, 6);
    
    public BonusType BonusType { get; set; }
    
    public JudgementType JudgementType { get; set; }
}