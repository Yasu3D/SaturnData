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
    /// And then converted to milliseconds.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when the early window is greater than the late window.</exception>
    public TimingWindow(int goodEarly, int greatEarly, int marvelousEarly, int marvelousPerfectEarly, int marvelousPerfectLate,  int marvelousLate,  int greatLate, int goodLate)
    {
        bool order = goodEarly <= greatEarly;
        order = order && greatEarly <= marvelousEarly;
        order = order && marvelousEarly <= marvelousPerfectEarly;
        order = order && marvelousPerfectEarly <= marvelousPerfectLate;
        order = order && marvelousPerfectLate <= marvelousLate;
        order = order && marvelousLate <= greatLate;
        order = order && greatLate <= goodLate;

        if (!order)
        {
            throw new ArgumentException("Defined timing window is invalid.");
        }
            
        MarvelousPerfectEarly = FrameDuration * marvelousPerfectEarly;
        MarvelousPerfectLate = FrameDuration * marvelousPerfectLate;
        MarvelousEarly = FrameDuration * marvelousEarly;
        MarvelousLate = FrameDuration * marvelousLate;
        GreatEarly = FrameDuration * greatEarly;
        GreatLate = FrameDuration * greatLate;
        GoodEarly = FrameDuration * goodEarly;
        GoodLate = FrameDuration * goodLate;
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