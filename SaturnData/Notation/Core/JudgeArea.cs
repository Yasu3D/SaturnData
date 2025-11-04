using System;

namespace SaturnData.Notation.Core;

/// <summary>
/// A judge area that defines when certain judgements are available.
/// </summary>
public class JudgeArea
{
    private const float FrameDuration = 1000.0f / 60.0f;
    
    /// <summary>
    /// Creates a judge area from another judge area.
    /// </summary>
    /// <param name="cloneSource"></param>
    public JudgeArea(JudgeArea cloneSource)
    {
        MarvelousEarly        = cloneSource.MarvelousEarly; 
        MarvelousLate         = cloneSource.MarvelousLate; 
        GreatEarly            = cloneSource.GreatEarly; 
        GreatLate             = cloneSource.GreatLate; 
        GoodEarly             = cloneSource.GoodEarly; 
        GoodLate              = cloneSource.GoodLate; 
    }
    
    /// <summary>
    /// Creates a judge area from milliseconds.
    /// </summary>
    /// <remarks>
    /// Parameters are interpreted as "signed" milliseconds:<br/>
    /// <i>- Early area values are negative<br/>
    /// - Late area values are positive.<br/></i>
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when the judge areas are defined out of order. (e.g. early area is later than the late area.)</exception>
    public JudgeArea(float time, float goodEarly, float greatEarly, float marvelousEarly, float marvelousLate,  float greatLate, float goodLate)
    {
        bool order = goodEarly             <= greatEarly
                  && greatEarly            <= marvelousEarly
                  && marvelousEarly        <= marvelousLate
                  && marvelousLate         <= greatLate
                  && greatLate             <= goodLate;

        if (!order)
        {
            throw new ArgumentException("Defined judge area is invalid.");
        }
        
        MarvelousEarly        = time + marvelousEarly;
        MarvelousLate         = time + marvelousLate;
        GreatEarly            = time + greatEarly;
        GreatLate             = time + greatLate;
        GoodEarly             = time + goodEarly;
        GoodLate              = time + goodLate;
    }

    /// <summary>
    /// Creates a judge area from 60fps frame timings.
    /// </summary>
    /// <remarks>
    /// Parameters are interpreted as "signed" frames:<br/>
    /// <i>- Early area values are negative<br/>
    /// - Late area values are positive.<br/></i>
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when the judge areas are defined out of order. (e.g. early area is later than the late area.)</exception>
    public static JudgeArea FromFrames(float time, int goodEarlyFrames, int greatEarlyFrames, int marvelousEarlyFrames, int marvelousLateFrames,  int greatLateFrames, int goodLateFrames)
    {
        bool order = goodEarlyFrames             <= greatEarlyFrames
                  && greatEarlyFrames            <= marvelousEarlyFrames
                  && marvelousEarlyFrames        <= marvelousLateFrames
                  && marvelousLateFrames         <= greatLateFrames
                  && greatLateFrames             <= goodLateFrames;

        if (!order)
        {
            throw new ArgumentException("Defined judge area is invalid.");
        }
        
        float marvelousEarly        = time + FrameDuration * marvelousEarlyFrames;
        float marvelousLate         = time + FrameDuration * marvelousLateFrames;
        float greatEarly            = time + FrameDuration * greatEarlyFrames;
        float greatLate             = time + FrameDuration * greatLateFrames;
        float goodEarly             = time + FrameDuration * goodEarlyFrames;
        float goodLate              = time + FrameDuration * goodLateFrames;

        return new(time, goodEarly, greatEarly, marvelousEarly, marvelousLate, greatLate, goodLate);
    }

    /// <summary>
    /// The earliest judge area.
    /// </summary>
    public float MaxEarly => Math.Min(MarvelousEarly, Math.Min(GreatEarly, GoodEarly));
    
    /// <summary>
    /// The latest judge area.
    /// </summary>
    public float MaxLate => Math.Max(MarvelousLate, Math.Max(GreatLate, GoodLate));
        
    /// <summary>
    /// The earliest judge area.
    /// </summary>
    public float ScaledMaxEarly => Math.Min(ScaledMarvelousEarly, Math.Min(ScaledGreatEarly, ScaledGoodEarly));
    
    /// <summary>
    /// The latest judge area.
    /// </summary>
    public float ScaledMaxLate => Math.Max(ScaledMarvelousLate, Math.Max(ScaledGreatLate, ScaledGoodLate));

    /// <summary>
    /// Start of "Marvelous" area in milliseconds.
    /// </summary>
    public float MarvelousEarly;

    /// <summary>
    /// End of "Marvelous" area in milliseconds.
    /// </summary>
    public float MarvelousLate;

    /// <summary>
    /// Start of "Great" area in milliseconds.
    /// </summary>
    public float GreatEarly;

    /// <summary>
    /// End of "Great" area in milliseconds.
    /// </summary>
    public float GreatLate;

    /// <summary>
    /// Start of "Good" area in milliseconds.
    /// </summary>
    public float GoodEarly;

    /// <summary>
    /// End of "Good" area in milliseconds.
    /// </summary>
    public float GoodLate;

    /// <summary>
    /// Start of "Marvelous" area in milliseconds, scaled by speed changes.
    /// </summary>
    public float ScaledMarvelousEarly;

    /// <summary>
    /// End of "Marvelous" area in milliseconds, scaled by speed changes.
    /// </summary>
    public float ScaledMarvelousLate;

    /// <summary>
    /// Start of "Great" area in milliseconds, scaled by speed changes.
    /// </summary>
    public float ScaledGreatEarly;

    /// <summary>
    /// End of "Great" area in milliseconds, scaled by speed changes.
    /// </summary>
    public float ScaledGreatLate;

    /// <summary>
    /// Start of "Good" area in milliseconds, scaled by speed changes.
    /// </summary>
    public float ScaledGoodEarly;

    /// <summary>
    /// End of "Good" area in milliseconds, scaled by speed changes.
    /// </summary>
    public float ScaledGoodLate;
}