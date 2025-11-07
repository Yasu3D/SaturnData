using SaturnData.Notation.Interfaces;
using SaturnData.Notation.Notes;

namespace SaturnData.Notation.Core;

public abstract class Note : ITimeable
{
    public virtual Timestamp Timestamp { get; set; } = Timestamp.Zero;
    
    /// <summary>
    /// Determines if two notes should have a "Sync" outline or not.
    /// </summary>
    public bool IsSync(Note? other)
    {
        if (other == null) return false;
        if (Timestamp != other.Timestamp) return false;

        if (this is not IPositionable pos0) return false;
        if (other is not IPositionable pos1) return false;
        if (this is SyncNote) return false; // ironic.
        if (other is SyncNote) return false;
        
        if (pos0.Position == pos1.Position && pos0.Size == pos1.Size) return false;
        
        if (this is ChainNote chain0 && chain0.BonusType != BonusType.R) return false;
        if (other is ChainNote chain1 && chain1.BonusType != BonusType.R) return false;
        
        return true;
    }
}