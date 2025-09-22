using System.Collections.Generic;
using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A note hit by tapping and holding down within its area at the right time.
/// </summary>
public class HoldNote : Note, ITimeable, IPositionable, IPlayable
{ 
    public HoldNote(HoldNote cloneSource)
    {
        Timestamp = cloneSource.Timestamp;
        BonusType = cloneSource.BonusType;
        JudgementType = cloneSource.JudgementType;
        TimingWindow = cloneSource.TimingWindow;

        foreach (HoldPointNote point in cloneSource.Points)
        {
            Points.Add(new(point.Timestamp, point.Position, point.Size, this, point.RenderType));
        }
    }
    
    public HoldNote(BonusType bonusType, JudgementType judgementType)
    {
        BonusType = bonusType;
        JudgementType = judgementType;
        TimingWindow = new(-6, -5, -3, -1, 1, 3, 5, 6);
    }
    
    /// <summary>
    /// The timestamp when the hold note begins.<br/>
    /// Modifying this timestamp will move all points as well.
    /// </summary>
    public Timestamp Timestamp
    {
        get => Points.Count == 0 ? new() : Points[0].Timestamp;
        set
        {
            if (Points.Count == 0) return;
            
            // Move all points equally when setting timestamp.
            Timestamp delta = value - Points[0].Timestamp;

            foreach (HoldPointNote point in Points)
            {
                point.Timestamp += delta;
            }
        }
    }

    public int Position
    {
        get => Points.Count == 0 ? -1 : Points[0].Position;
        set
        {
            if (Points.Count == 0) return;
            
            // Move all points equally when setting timestamp.
            int delta = value - Points[0].Position;

            foreach (HoldPointNote point in Points)
            {
                point.Position += delta;
            }
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
    
    public TimingWindow TimingWindow { get; set; }
    
    public BonusType BonusType { get; set; }
    
    public JudgementType JudgementType { get; set; }

    /// <summary>
    /// The individual points defining the shape and path a hold note takes.
    /// </summary>
    public List<HoldPointNote> Points { get; set; } = [];
}