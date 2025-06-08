using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;

namespace SaturnData.Notation.Notes;

/// <summary>
/// A note hit by tapping within its area at the right time.
/// </summary>
public class TouchNote : Note, ITimeable, IPositionable, ILayerable, IPlayable
{
    public TouchNote(TouchNote source)
    {
        Timestamp = source.Timestamp;
        Position = source.Position;
        Size = source.Size;
        Layer = source.Layer;
        BonusType = source.BonusType;
        IsJudgeable = source.IsJudgeable;
        TimingWindow = source.TimingWindow;
    }
    
    public TouchNote(Timestamp timestamp, int position, int size, int layer, BonusType bonusType, bool isJudgeable)
    {
        Timestamp = timestamp;
        Position = position;
        Size = size;
        Layer = layer;
        BonusType = bonusType;
        IsJudgeable = isJudgeable;
        TimingWindow = new(-6, -5, -3, -1, 1, 3, 5, 6);
    }
    
    public Timestamp Timestamp { get; set; }
    
    public int Position { get; set; }
    
    public int Size { get; set; }
    
    public int Layer { get; set; }

    public TimingWindow TimingWindow { get; set; }
    
    public BonusType BonusType { get; set; }
    
    public bool IsJudgeable { get; set; }
}