using System;
using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A note hit by flicking backwards within its area at the right time.
/// </summary>
public sealed class SnapBackwardNote : Note, IPositionable, IPlayable, ICloneable
{
    public SnapBackwardNote(Timestamp timestamp, int position, int size, BonusType bonusType, JudgementType judgementType)
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
    
    public JudgeArea JudgeAreaTemplate => JudgeArea.FromFrames(0, -10, -10, -7, 5, 8, 10);
    
    public BonusType BonusType { get; set; }
    
    public JudgementType JudgementType { get; set; }
    
    public object Clone()
    {
        SnapBackwardNote clone = new(new(Timestamp), Position, Size, BonusType, JudgementType)
        {
            JudgeArea = new(JudgeArea),
        };

        return clone;
    }
}