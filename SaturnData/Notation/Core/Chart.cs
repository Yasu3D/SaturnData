using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SaturnData.Notation.Events;
using SaturnData.Notation.Interfaces;
using SaturnData.Notation.Notes;

namespace SaturnData.Notation.Core;

/// <summary>
/// Holds all data associated with gameplay.
/// </summary>
public class Chart
{
    /// <summary>
    /// All global events that aren't bound to layers.
    /// </summary>
    public List<Event> Events { get; set; } = [];
    
    /// <summary>
    /// All local events and notes, grouped by layer.
    /// </summary>
    public List<Layer> Layers { get; set; } = [];
    
    /// <summary>
    /// All lane toggle notes.
    /// </summary>
    public List<Note> LaneToggles { get; set; } = [];
    
    /// <summary>
    /// Editor-only bookmarks.
    /// </summary>
    public List<Bookmark> Bookmarks { get; set; } = [];
    
    /// <summary>
    /// Finds the last tempo change before a given timestamp.
    /// </summary>
    public TempoChangeEvent? LastTempoChange(Timestamp timestamp)
    {
        if (timestamp.FullTick == 0)
        {
            return Events.LastOrDefault(x => x is TempoChangeEvent && x.Timestamp.FullTick == 0) as TempoChangeEvent;
        }
        
        return Events.OrderBy(x => x.Timestamp).LastOrDefault(x => x is TempoChangeEvent && x.Timestamp < timestamp) as TempoChangeEvent;
    }
    
    /// <summary>
    /// Finds the last tempo change before a given time.
    /// </summary>
    public TempoChangeEvent? LastTempoChange(float time)
    {
        if (time == 0)
        {
            return Events.LastOrDefault(x => x is TempoChangeEvent && x.Timestamp.Time == 0) as TempoChangeEvent;
        }
        
        return Events.OrderBy(x => x.Timestamp).LastOrDefault(x => x is TempoChangeEvent && x.Timestamp.Time < time) as TempoChangeEvent;
    }

    /// <summary>
    /// Finds the last metre change before a given timestamp.
    /// </summary>
    public MetreChangeEvent? LastMetreChange(Timestamp timestamp)
    {
        if (timestamp.FullTick == 0)
        {
            return Events.LastOrDefault(x => x is MetreChangeEvent && x.Timestamp.FullTick == 0) as MetreChangeEvent;
        }
        
        return Events.OrderBy(x => x.Timestamp).LastOrDefault(x => x is MetreChangeEvent && x.Timestamp < timestamp) as MetreChangeEvent;
    }

    /// <summary>
    /// Finds the last metre change before a given time.
    /// </summary>
    public MetreChangeEvent? LastMetreChange(float time)
    {
        if (time == 0)
        {
            return Events.LastOrDefault(x => x is MetreChangeEvent && x.Timestamp.Time == 0) as MetreChangeEvent;
        }
        
        return Events.OrderBy(x => x.Timestamp).LastOrDefault(x => x is MetreChangeEvent && x.Timestamp.Time < time) as MetreChangeEvent;
    }
    
    /// <summary>
    /// Calculates the ideal chart end timestamp, based on all objects in a chart and the length of audio.
    /// </summary>
    /// <param name="audioLength">The length of the audio file in milliseconds.(optional)</param>
    /// <returns></returns>
    public Timestamp FindIdealChartEnd(float audioLength = 0)
    {
        Timestamp audioEnd = Timestamp.TimestampFromTime(this, audioLength, 1920);
        Timestamp chartEnd = Timestamp.Zero;

        foreach (Layer layer in Layers)
        foreach (Note note in layer.Notes)
        {
            if (note is not IPlayable playable) continue;
            if (playable.JudgementType is JudgementType.Fake) continue; // Allow for fake notes behind end
            
            chartEnd = Timestamp.Max(chartEnd, note.Timestamp);
        }
        
        chartEnd = Timestamp.TimestampFromTime(this, chartEnd.Time + 2000, 1920);
        
        // Round up to next measure.
        if (chartEnd.Tick != 0)
        {
            chartEnd.Measure += 1;
            chartEnd.Tick = 0;
            chartEnd.Time = Timestamp.TimeFromTimestamp(this, chartEnd);
        }
        
        if (audioEnd.Tick != 0)
        {
            audioEnd.Measure += 1;
            audioEnd.Tick = 0;
            audioEnd.Time = Timestamp.TimeFromTimestamp(this, audioEnd);
        }

        return Timestamp.Max(chartEnd, audioEnd);
    }
    
    /// <summary>
    /// Pre-calculates all values for rendering or gameplay to function properly.
    /// </summary>
    /// <param name="entry"></param>
    public void Build(Entry entry)
    {
        // Update Millisecond Time & ScaledTime.
        // Clear Generated Notes on all layers.
        foreach (Event @event in Events)
        {
            float time = Timestamp.TimeFromTimestamp(this, @event.Timestamp);
            @event.Timestamp = @event.Timestamp with { Time = time, ScaledTime = time };
        }

        foreach (Bookmark bookmark in Bookmarks)
        {
            float time = Timestamp.TimeFromTimestamp(this, bookmark.Timestamp);
            bookmark.Timestamp = bookmark.Timestamp with { Time = time, ScaledTime = time };
        }

        foreach (Note laneToggle in LaneToggles)
        {
            float time = Timestamp.TimeFromTimestamp(this, laneToggle.Timestamp);
            laneToggle.Timestamp = laneToggle.Timestamp with { Time = time, ScaledTime = time };
        }

        foreach (Layer layer in Layers)
        {
            layer.GeneratedNotes.Clear(); // Clear Generated Notes here to save an enumeration.
            
            foreach (Event @event in layer.Events)
            {
                if (@event is StopEffectEvent stopEffectEvent)
                {
                    stopEffectEvent.SubEvents = stopEffectEvent.SubEvents.OrderBy(x => x.Timestamp.FullTick).ToArray();
                    
                    foreach (EffectSubEvent subEvent in stopEffectEvent.SubEvents)
                    {
                        float time = Timestamp.TimeFromTimestamp(this, subEvent.Timestamp);
                        subEvent.Timestamp = subEvent.Timestamp with { Time = time, ScaledTime = Timestamp.ScaledTimeFromTime(layer, time) };
                    }
                }
                else if (@event is ReverseEffectEvent reverseEffectEvent)
                {
                    reverseEffectEvent.SubEvents = reverseEffectEvent.SubEvents.OrderBy(x => x.Timestamp.FullTick).ToArray();
                    
                    foreach (EffectSubEvent subEvent in reverseEffectEvent.SubEvents)
                    {
                        float time = Timestamp.TimeFromTimestamp(this, subEvent.Timestamp);
                        subEvent.Timestamp = subEvent.Timestamp with { Time = time, ScaledTime = Timestamp.ScaledTimeFromTime(layer, time) };
                    }
                }
                else
                {
                    float time = Timestamp.TimeFromTimestamp(this, @event.Timestamp);
                    @event.Timestamp = @event.Timestamp with { Time = time, ScaledTime = Timestamp.ScaledTimeFromTime(layer, time) };
                }
            }

            foreach (Note note in layer.Notes)
            {
                if (note is HoldNote holdNote)
                {
                    foreach (HoldPointNote point in holdNote.Points)
                    {
                        float time = Timestamp.TimeFromTimestamp(this, point.Timestamp);
                        point.Timestamp = point.Timestamp with { Time = time, ScaledTime = Timestamp.ScaledTimeFromTime(layer, time) };
                    }
                }
                else
                {
                    float time = Timestamp.TimeFromTimestamp(this, note.Timestamp);
                    note.Timestamp = note.Timestamp with { Time = time, ScaledTime = Timestamp.ScaledTimeFromTime(layer, time) };
                }
            }
        }

        // Update Chart End.
        if (entry.AutoChartEnd)
        {
            entry.ChartEnd = FindIdealChartEnd();
        }
        else
        {
            entry.ChartEnd = entry.ChartEnd with { Time = Timestamp.TimeFromTimestamp(this, entry.ChartEnd) };
        }
        
        // Create new generated notes.
        for (int l = 0; l < Layers.Count; l++)
        {
            Layer layer = Layers[l];
            
            // Create Measure and Beat Lines
            if (l == 0)
            {
                for (int i = 0; i < entry.ChartEnd.Measure; i++)
                {
                    Timestamp timestamp = new(i, 0);
                    timestamp.Time = Timestamp.TimeFromTimestamp(this, timestamp);
                    timestamp.ScaledTime = Timestamp.ScaledTimeFromTime(layer, timestamp.Time);
                    
                    layer.GeneratedNotes.Add(new MeasureLineNote(timestamp, false));
                }
                
                List<MetreChangeEvent> metreChangeEvents = Events.OfType<MetreChangeEvent>().ToList();

                for (int i = 0; i < metreChangeEvents.Count; i++)
                {
                    int startTick = metreChangeEvents[i].Timestamp.FullTick;
                    int endTick = i == metreChangeEvents.Count - 1
                        ? entry.ChartEnd.FullTick
                        : metreChangeEvents[i + 1].Timestamp.FullTick;

                    int step = 1920 / metreChangeEvents[i].Upper;

                    for (int j = startTick; j < endTick; j += step)
                    {
                        if (j % 1920 == 0) continue;
                        
                        Timestamp timestamp = new(j);
                        timestamp.Time = Timestamp.TimeFromTimestamp(this, timestamp);
                        timestamp.ScaledTime = Timestamp.ScaledTimeFromTime(layer, timestamp.Time);
                        
                        layer.GeneratedNotes.Add(new MeasureLineNote(timestamp, true));
                    }
                }
            }
            
            // Create Sync Notes
            for (int i = 1; i < layer.Notes.Count; i++)
            {
                Note current = layer.Notes[i];
                Note previous = layer.Notes[i - 1];

                if (!current.IsSync(previous)) continue;
                if (current is not IPositionable currentPositionable) continue;
                if (previous is not IPositionable previousPositionable) continue;

                // -1 + 60
                // => + 59
                int position0 = (currentPositionable.Position + currentPositionable.Size + 59) % 60;
                int position1 = (previousPositionable.Position + previousPositionable.Size + 59) % 60;

                int size0 = (previousPositionable.Position - position0 + 60) % 60 + 1;
                int size1 = (currentPositionable.Position  - position1 + 60) % 60 + 1;

                int finalPosition = size0 < size1 ? position0 : position1;
                int finalSize = Math.Min(size0, size1);

                if (finalSize > 30) continue;

                SyncNote syncNote = new(current.Timestamp, finalPosition, finalSize);
                layer.GeneratedNotes.Add(syncNote);
            }

            layer.GeneratedNotes = layer.GeneratedNotes.OrderBy(x => x.Timestamp.FullTick).ToList();
        }
        
        // Re-build reverse effect note lists.
        foreach (Layer layer in Layers)
        {
            foreach (Event @event in layer.Events)
            {
                if (@event is not ReverseEffectEvent reverseEffectEvent) continue;
                if (reverseEffectEvent.SubEvents.Length != 3) continue;

                reverseEffectEvent.ContainedNotes.Clear();
                
                foreach (Note note in layer.Notes)
                {
                    if (note.Timestamp.FullTick <= reverseEffectEvent.SubEvents[1].Timestamp.FullTick) continue;
                    if (note.Timestamp.FullTick >= reverseEffectEvent.SubEvents[2].Timestamp.FullTick) continue;

                    if (note is HoldNote holdNote && holdNote.Points.Count != 0)
                    {
                        int end = holdNote.Points[^1].Timestamp.FullTick;
                        if (end >= reverseEffectEvent.SubEvents[2].Timestamp.FullTick) continue;
                    }
                    
                    reverseEffectEvent.ContainedNotes.Add(note);
                }

                // Not source-game accurate, but looks nicer :)
                foreach (Note note in layer.GeneratedNotes)
                {
                    // It looks nicer if measure lines on top of the reverse sub-events are included as well.
                    // Do the == checks only for sync notes.
                    if (note is SyncNote)
                    {
                        if (note.Timestamp.FullTick <= reverseEffectEvent.SubEvents[1].Timestamp.FullTick) continue;
                        if (note.Timestamp.FullTick >= reverseEffectEvent.SubEvents[2].Timestamp.FullTick) continue;
                    }
                    else
                    {
                        if (note.Timestamp.FullTick < reverseEffectEvent.SubEvents[1].Timestamp.FullTick) continue;
                        if (note.Timestamp.FullTick > reverseEffectEvent.SubEvents[2].Timestamp.FullTick) continue;
                    }
                    
                    reverseEffectEvent.ContainedNotes.Add(note);
                }
            }
        }
    }
}