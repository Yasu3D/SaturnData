using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A note hit by holding down within its area at any time.
/// </summary>
public class ChainNote : Note, ITimeable, IPositionable, ILayerable, IPlayable
{
    public ChainNote(ChainNote source)
    {
        Timestamp = source.Timestamp;
        Position = source.Position;
        Size = source.Size;
        Layer = source.Layer;
        BonusType = source.BonusType;
        IsJudgeable = source.IsJudgeable;
        TimingWindow = source.TimingWindow;
    }
    
    public ChainNote(Timestamp timestamp, int position, int size, int layer, BonusType bonusType, bool isJudgeable)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
        Layer = layer;
        BonusType = bonusType;
        IsJudgeable = isJudgeable;
        TimingWindow = new(-4, -4, -4, -4, 4, 4, 4, 4);
    }
    
    public Timestamp Timestamp { get; set; }
    
    public int Position { get; set; }
    
    public int Size { get; set; }
    
    public int Layer { get; set; }

    public TimingWindow TimingWindow { get; set; }
    
    public BonusType BonusType { get; set; }
    
    public bool IsJudgeable { get; set; }
}