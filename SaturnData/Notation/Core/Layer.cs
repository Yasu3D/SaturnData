using System.Collections.Generic;
using System.Linq;
using SaturnData.Notation.Events;

namespace SaturnData.Notation.Core;

public class Layer(string name)
{
    /// <summary>
    /// The name of the layer.
    /// </summary>
    /// <remarks>
    /// <b>This only exists for human-readability!</b><br/>
    /// <see cref="Name"/> should not be used for identification by any renderer or engine.
    /// </remarks>
    public string Name { get; set; } = name;
    
    /// <summary>
    /// The visibility of the layer.
    /// </summary>
    /// <remarks>
    ///<b>This only exists for editors!</b><br/>
    /// <see cref="Visible"/> should only be used in editors, and should never be serialized/saved.<br/><br/>
    /// To test for user-defined visibility changes, see <see cref="LastVisibilityChange"/>
    /// </remarks>
    public bool Visible { get; set; } = true;
    
    /// <summary>
    /// The list of layer-specific events on this layer.
    /// </summary>
    public List<Event> Events { get; set; } = [];
    
    /// <summary>
    /// The list of user-placed notes on this layer.
    /// </summary>
    public List<Note> Notes { get; set; } = [];
    
    /// <summary>
    /// The list of automatically generated notes on this layer.
    /// </summary>
    /// <remarks>
    /// <b>No user-created content should be stored here!</b><br/>
    /// This list gets cleared every time <see cref="Chart.Build"/> is called on a chart. All previously added notes will be lost.<br/><br/>
    /// Since this list only contains automatically generated notes, it also shouldn't be serialized.
    /// </remarks>
    public List<Note> GeneratedNotes { get; set; } = [];
    
    /// <summary>
    /// Finds the last visibility change on this layer before a given time.
    /// </summary>
    public VisibilityChangeEvent? LastVisibilityChange(float time)
    {
        if (time == 0)
        {
            return Events.LastOrDefault(x => x is VisibilityChangeEvent && x.Timestamp.Time == 0) as VisibilityChangeEvent;
        }
        
        return Events.OrderBy(x => x.Timestamp).LastOrDefault(x => x is VisibilityChangeEvent && x.Timestamp.Time <= time) as VisibilityChangeEvent;
    }
    
    /// <summary>
    /// Finds the last speed change on this layer before a given time.
    /// </summary>
    public SpeedChangeEvent? LastSpeedChange(float time)
    {
        if (time == 0)
        {
            return Events.LastOrDefault(x => x is SpeedChangeEvent && x.Timestamp.Time == 0) as SpeedChangeEvent;
        }
        
        return Events.OrderBy(x => x.Timestamp).LastOrDefault(x => x is SpeedChangeEvent && x.Timestamp.Time < time) as SpeedChangeEvent;
    }

    /// <summary>
    /// Finds the last stop effect on this layer before a given time.
    /// </summary>
    public StopEffectEvent? LastStopEffect(float time)
    {
        if (time == 0)
        {
            return Events.LastOrDefault(x => x is StopEffectEvent && x.Timestamp.Time == 0) as StopEffectEvent;
        }
        
        return Events.OrderBy(x => x.Timestamp).LastOrDefault(x => x is StopEffectEvent && x.Timestamp.Time < time) as StopEffectEvent;
    }
    
    /// <summary>
    /// Finds the last stop effect on this layer before a given time.
    /// </summary>
    public ReverseEffectEvent? LastReverseEffect(float time)
    {
        if (time == 0)
        {
            return Events.LastOrDefault(x => x is ReverseEffectEvent && x.Timestamp.Time == 0) as ReverseEffectEvent;
        }
        
        return Events.OrderBy(x => x.Timestamp).LastOrDefault(x => x is ReverseEffectEvent && x.Timestamp.Time < time) as ReverseEffectEvent;
    }
}