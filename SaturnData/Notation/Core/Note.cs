using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Core;

public abstract class Note : ITimeable
{
    public virtual Timestamp Timestamp { get; set; }
}