using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A note hit by sliding counterclockwise within its area at the right time.
/// </summary>
public class SlideCounterclockwiseNote : Note, ITimeable, IPositionable, ILayerable, IPlayable
{
    public SlideCounterclockwiseNote(SlideCounterclockwiseNote source)
    {
        Timestamp = source.Timestamp;
        Position = source.Position;
        Size = source.Size;
        Layer = source.Layer;
        BonusType = source.BonusType;
        IsJudgeable = source.IsJudgeable;
        TimingWindow = source.TimingWindow;
    }
    
    public SlideCounterclockwiseNote(Timestamp timestamp, int position, int size, int layer, BonusType bonusType, bool isJudgeable)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
        Layer = layer;
        BonusType = bonusType;
        IsJudgeable = isJudgeable;
        TimingWindow = new(-10, -8, -5, -1, 1, 7, 10, 10);
    }
    
    public Timestamp Timestamp { get; set; }
    
    public int Position { get; set; }
    
    public int Size { get; set; }
    
    public int Layer { get; set; }

    public TimingWindow TimingWindow { get; set; }
    
    public BonusType BonusType { get; set; }
    
    public bool IsJudgeable { get; set; }
}