namespace SaturnData.Notation.Interfaces;

/// <summary>
/// Implements scroll layer attributes.
/// </summary>
public interface ILayerable
{
    /// <summary>
    /// The layer of an ILayerable.<br/>
    /// ILayerable must be visible during gameplay to be affected by different layers.
    /// </summary>
    public int Layer { get; set; }
}