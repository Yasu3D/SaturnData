using System;

namespace SaturnData.Notation.Core;

/// <summary>
/// A timing window that defines when certain judgements are available.
/// </summary>
public class TimingWindow
{
    private const float FrameDuration = 1000.0f / 60.0f;
    
    /// <summary>
    /// Creates a timing window from another timing window.
    /// </summary>
    /// <param name="cloneSource"></param>
    public TimingWindow(TimingWindow cloneSource)
    {
        MarvelousPerfectEarly = cloneSource.MarvelousPerfectEarly; 
        MarvelousPerfectLate  = cloneSource.MarvelousPerfectLate; 
        MarvelousEarly        = cloneSource.MarvelousEarly; 
        MarvelousLate         = cloneSource.MarvelousLate; 
        GreatEarly            = cloneSource.GreatEarly; 
        GreatLate             = cloneSource.GreatLate; 
        GoodEarly             = cloneSource.GoodEarly; 
        GoodLate              = cloneSource.GoodLate; 
    }
    
    /// <summary>
    /// Creates a timing window from milliseconds.
    /// </summary>
    /// <remarks>
    /// Parameters are interpreted as "signed" milliseconds:<br/>
    /// <i>- Early window values are negative<br/>
    /// - Late window values are positive.<br/></i>
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when the timing windows are defined out of order. (e.g. early window is later than the late window.)</exception>
    public TimingWindow(float time, float goodEarly, float greatEarly, float marvelousEarly, float marvelousPerfectEarly, float marvelousPerfectLate,  float marvelousLate,  float greatLate, float goodLate)
    {
        bool order = goodEarly             <= greatEarly
                  && greatEarly            <= marvelousEarly
                  && marvelousEarly        <= marvelousPerfectEarly
                  && marvelousPerfectEarly <= marvelousPerfectLate
                  && marvelousPerfectLate  <= marvelousLate
                  && marvelousLate         <= greatLate
                  && greatLate             <= goodLate;

        if (!order)
        {
            throw new ArgumentException("Defined timing window is invalid.");
        }
            
        MarvelousPerfectEarly = time + marvelousPerfectEarly;
        MarvelousPerfectLate  = time + marvelousPerfectLate;
        MarvelousEarly        = time + marvelousEarly;
        MarvelousLate         = time + marvelousLate;
        GreatEarly            = time + greatEarly;
        GreatLate             = time + greatLate;
        GoodEarly             = time + goodEarly;
        GoodLate              = time + goodLate;
    }

    /// <summary>
    /// Creates a timing window from 60fps frame timings.
    /// </summary>
    /// <remarks>
    /// Parameters are interpreted as "signed" frames:<br/>
    /// <i>- Early window values are negative<br/>
    /// - Late window values are positive.<br/></i>
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when the timing windows are defined out of order. (e.g. early window is later than the late window.)</exception>
    public static TimingWindow FromFrames(float time, int goodEarlyFrames, int greatEarlyFrames, int marvelousEarlyFrames, int marvelousPerfectEarlyFrames, int marvelousPerfectLateFrames,  int marvelousLateFrames,  int greatLateFrames, int goodLateFrames)
    {
        bool order = goodEarlyFrames             <= greatEarlyFrames
                  && greatEarlyFrames            <= marvelousEarlyFrames
                  && marvelousEarlyFrames        <= marvelousPerfectEarlyFrames
                  && marvelousPerfectEarlyFrames <= marvelousPerfectLateFrames
                  && marvelousPerfectLateFrames  <= marvelousLateFrames
                  && marvelousLateFrames         <= greatLateFrames
                  && greatLateFrames             <= goodLateFrames;

        if (!order)
        {
            throw new ArgumentException("Defined timing window is invalid.");
        }
        
        float marvelousPerfectEarly = time + FrameDuration * marvelousPerfectEarlyFrames;
        float marvelousPerfectLate  = time + FrameDuration * marvelousPerfectLateFrames;
        float marvelousEarly        = time + FrameDuration * marvelousEarlyFrames;
        float marvelousLate         = time + FrameDuration * marvelousLateFrames;
        float greatEarly            = time + FrameDuration * greatEarlyFrames;
        float greatLate             = time + FrameDuration * greatLateFrames;
        float goodEarly             = time + FrameDuration * goodEarlyFrames;
        float goodLate              = time + FrameDuration * goodLateFrames;

        return new(time, goodEarly, greatEarly, marvelousEarly, marvelousPerfectEarly, marvelousPerfectLate, marvelousLate, greatLate, goodLate);
    }

    /// <summary>
    /// The earliest timing window.
    /// </summary>
    public float MaxEarly => Math.Min(MarvelousPerfectEarly, Math.Min(MarvelousEarly, Math.Min(GreatEarly, GoodEarly)));
    
    /// <summary>
    /// The latest timing window.
    /// </summary>
    public float MaxLate => Math.Max(MarvelousPerfectLate, Math.Max(MarvelousLate, Math.Max(GreatLate, GoodLate)));
        
    /// <summary>
    /// The earliest timing window.
    /// </summary>
    public float ScaledMaxEarly => Math.Min(ScaledMarvelousPerfectEarly, Math.Min(ScaledMarvelousEarly, Math.Min(ScaledGreatEarly, ScaledGoodEarly)));
    
    /// <summary>
    /// The latest timing window.
    /// </summary>
    public float ScaledMaxLate => Math.Max(ScaledMarvelousPerfectLate, Math.Max(ScaledMarvelousLate, Math.Max(ScaledGreatLate, ScaledGoodLate)));
    
    /// <summary>
    /// Start of "Perfect Marvelous" window in milliseconds.
    /// </summary>
    public float MarvelousPerfectEarly;

    /// <summary>
    /// End of "Perfect Marvelous" window in milliseconds.
    /// </summary>
    public float MarvelousPerfectLate;

    /// <summary>
    /// Start of "Marvelous" window in milliseconds.
    /// </summary>
    public float MarvelousEarly;

    /// <summary>
    /// End of "Marvelous" window in milliseconds.
    /// </summary>
    public float MarvelousLate;

    /// <summary>
    /// Start of "Great" window in milliseconds.
    /// </summary>
    public float GreatEarly;

    /// <summary>
    /// End of "Great" window in milliseconds.
    /// </summary>
    public float GreatLate;

    /// <summary>
    /// Start of "Good" window in milliseconds.
    /// </summary>
    public float GoodEarly;

    /// <summary>
    /// End of "Good" window in milliseconds.
    /// </summary>
    public float GoodLate;
    
    /// <summary>
    /// Start of "Perfect Marvelous" window in milliseconds, scaled by speed changes.
    /// </summary>
    public float ScaledMarvelousPerfectEarly;

    /// <summary>
    /// End of "Perfect Marvelous" window in milliseconds, scaled by speed changes.
    /// </summary>
    public float ScaledMarvelousPerfectLate;

    /// <summary>
    /// Start of "Marvelous" window in milliseconds, scaled by speed changes.
    /// </summary>
    public float ScaledMarvelousEarly;

    /// <summary>
    /// End of "Marvelous" window in milliseconds, scaled by speed changes.
    /// </summary>
    public float ScaledMarvelousLate;

    /// <summary>
    /// Start of "Great" window in milliseconds, scaled by speed changes.
    /// </summary>
    public float ScaledGreatEarly;

    /// <summary>
    /// End of "Great" window in milliseconds, scaled by speed changes.
    /// </summary>
    public float ScaledGreatLate;

    /// <summary>
    /// Start of "Good" window in milliseconds, scaled by speed changes.
    /// </summary>
    public float ScaledGoodEarly;

    /// <summary>
    /// End of "Good" window in milliseconds, scaled by speed changes.
    /// </summary>
    public float ScaledGoodLate;
}