using SaturnData.Notation.Core;

namespace SaturnData.Notation.Interfaces;

/// <summary>
/// Implements timestamp attributes.
/// </summary>
public interface ITimeable
{
    /// <summary>
    /// The timestamp of an ITimeable.
    /// </summary>
    public Timestamp Timestamp { get; set; }
}