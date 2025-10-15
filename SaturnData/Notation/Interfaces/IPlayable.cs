using SaturnData.Notation.Core;

namespace SaturnData.Notation.Interfaces;

public enum BonusType
{
    Normal = 0,
    Bonus = 1,
    R = 2,
}

public enum JudgementType
{
    Normal = 0,
    Fake = 1,
    Autoplay = 2,
}

/// <summary>
/// Implements gameplay-specific attributes.
/// </summary>
public interface IPlayable
{
    /// <summary>
    /// The timing window of the IPlayable.
    /// </summary>
    public TimingWindow TimingWindow { get; set; }

    /// <summary>
    /// The template timing window of the IPlayable.
    /// </summary>
    public TimingWindow TimingWindowTemplate { get; }

    /// <summary>
    /// The bonus type of the IPlayable.
    /// </summary>
    public BonusType BonusType { get; set; }
    
    /// <summary>
    /// Determines if an IPlayable should be judged or not.
    /// </summary>
    public JudgementType JudgementType { get; set; }
}