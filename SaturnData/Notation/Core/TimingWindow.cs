using System;

namespace SaturnData.Notation.Core;

/// <summary>
/// A timing window that defines when certain judgements are available.
/// </summary>
public struct TimingWindow
{
    /// <summary>
    /// Creates a timing window.
    /// </summary>
    /// <remarks>
    /// Parameters are interpreted as "signed" frames:<br/>
    /// <i>- Early window values are negative<br/>
    /// - Late window values are positive.<br/></i>
    /// And converted to milliseconds within the constructor.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when the timing windows are defined out of order. (e.g. early window is later than the late window.)</exception>
    public TimingWindow(int goodEarlyFrames, int greatEarlyFrames, int marvelousEarlyFrames, int marvelousPerfectEarlyFrames, int marvelousPerfectLateFrames,  int marvelousLateFrames,  int greatLateFrames, int goodLateFrames)
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
            
        MarvelousPerfectEarly = FrameDuration * marvelousPerfectEarlyFrames;
        MarvelousPerfectLate = FrameDuration * marvelousPerfectLateFrames;
        MarvelousEarly = FrameDuration * marvelousEarlyFrames;
        MarvelousLate = FrameDuration * marvelousLateFrames;
        GreatEarly = FrameDuration * greatEarlyFrames;
        GreatLate = FrameDuration * greatLateFrames;
        GoodEarly = FrameDuration * goodEarlyFrames;
        GoodLate = FrameDuration * goodLateFrames;
    }

    public const float FrameDuration = 1.0f / 60.0f;

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
}