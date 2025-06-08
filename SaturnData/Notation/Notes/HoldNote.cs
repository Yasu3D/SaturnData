using System.Collections.Generic;
using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A note hit by tapping and holding down within its area at the right time.
/// </summary>
public class HoldNote : Note, ITimeable, IPlayable
{ 
    public HoldNote(HoldNote source)
    {
        Timestamp = source.Timestamp;
        BonusType = source.BonusType;
        IsJudgeable = source.IsJudgeable;
        TimingWindow = source.TimingWindow;

        foreach (HoldPointNote point in source.Points)
        {
            Points.Add(new(point));
        }
    }
    
    public HoldNote(List<HoldPointNote> points, BonusType bonusType, bool isJudgeable)
    {
        if (points.Count == 0) return;
        
        Timestamp = points[0].Timestamp;
        BonusType = bonusType;
        IsJudgeable = isJudgeable;
        TimingWindow = new(-6, -5, -3, -1, 1, 3, 5, 6);
        
        Points = points;
    }
    
    /// <summary>
    /// The timestamp when the reverse effect begins.<br/>
    /// Modifying this timestamp will move all sub-events as well.
    /// </summary>
    public Timestamp Timestamp
    {
        get => Points[0].Timestamp;
        set
        { 
            // Move all sub-events equally when setting timestamp.
            Timestamp delta = value - Points[0].Timestamp;

            foreach (HoldPointNote point in Points)
            {
                point.Timestamp += delta;
            }
        }
    }
    
    public TimingWindow TimingWindow { get; set; }
    
    public BonusType BonusType { get; set; }
    
    public bool IsJudgeable { get; set; }
    
    /// <summary>
    /// The individual points defining the shape and path a hold note takes.
    /// </summary>
    public readonly List<HoldPointNote> Points = [];
}