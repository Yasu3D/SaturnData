using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Core;

public abstract class Event : ITimeable
{
    public virtual Timestamp Timestamp { get; set; }
}