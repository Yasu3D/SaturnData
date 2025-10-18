using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A note hit by holding down within its area at any time.
/// </summary>
public sealed class ChainNote : Note, IPositionable, IPlayable
{
    public ChainNote(ChainNote cloneSource)
    {
        Timestamp = new(cloneSource.Timestamp);
        Position = cloneSource.Position;
        Size = cloneSource.Size;
        BonusType = cloneSource.BonusType;
        JudgementType = cloneSource.JudgementType;
        JudgeArea = new(cloneSource.JudgeArea);
    }
    
    public ChainNote(Timestamp timestamp, int position, int size, BonusType bonusType, JudgementType judgementType)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
        BonusType = bonusType;
        JudgementType = judgementType;
        JudgeArea = JudgeAreaTemplate;
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

    public JudgeArea JudgeArea { get; set; }
    
    public JudgeArea JudgeAreaTemplate => JudgeArea.FromFrames(0, -4, -4, -4, 4, 4, 4);
    
    public BonusType BonusType { get; set; }
    
    public JudgementType JudgementType { get; set; }
}