using System;
using System.Collections.Generic;
using System.Linq;
using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A note hit by tapping and holding down within its area at the right time.
/// </summary>
public class HoldNote : Note, IPositionable, IPlayable, ICloneable
{ 
    public HoldNote(BonusType bonusType, JudgementType judgementType)
    {
        BonusType = bonusType;
        JudgementType = judgementType;
        JudgeArea = JudgeAreaTemplate;
    }
    
    /// <summary>
    /// The timestamp when the hold note begins.<br/>
    /// Modifying this timestamp will move all points as well.
    /// </summary>
    public sealed override Timestamp Timestamp => Points.Count == 0 ? Timestamp.Zero : Points[0].Timestamp;

    public int Position
    {
        get => Points.Count == 0 ? -1 : Points[0].Position;
        set
        {
            if (Points.Count == 0) return;

            Points[0].Position = value;
        }
    }

    public int Size
    {
        get => Points.Count == 0 ? -1 : Points[0].Size;
        set
        {
            if (Points.Count == 0) return;

            Points[0].Size = value;
        }
    }
    
    public JudgeArea JudgeArea { get; set; }

    public JudgeArea JudgeAreaTemplate => JudgeArea.FromFrames(0, -6, -5, -3, 3, 5, 6);

    public BonusType BonusType { get; set; }
    
    public JudgementType JudgementType { get; set; }

    /// <summary>
    /// The individual points defining the shape and path a hold note takes.
    /// </summary>
    public List<HoldPointNote> Points { get; set; } = [];

    /// <summary>
    /// The size of the largest point in the hold note.
    /// </summary>
    public int MaxSize => Points.Max(x => x.Size);
    
    public object Clone()
    {
        HoldNote clone = new(BonusType, JudgementType)
        {
            JudgeArea = new(JudgeArea),
        };
        
        foreach (HoldPointNote point in Points)
        {
            clone.Points.Add(new(new(point.Timestamp), point.Position, point.Size, clone, point.RenderType));
        }

        return clone;
    }
}