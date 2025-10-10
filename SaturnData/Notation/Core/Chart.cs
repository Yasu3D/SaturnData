using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SaturnData.Notation.Events;
using SaturnData.Notation.Interfaces;

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
    
    public void Build()
    {
        /*
        /// <summary>
        /// Calculates millisecond timestamps for all objects in an entry and a chart.
        /// </summary>
        /// <param name="entry">The entry to calculate timestamps for.</param>
        /// <param name="chart">The chart to calculate timestamps for.</param>
        public static void GenerateTimeForAllObjects(Entry entry, Chart chart)
        {
            foreach (Event @event in chart.Events)
            {
                @event.Timestamp = @event.Timestamp with { Time = Timestamp.TimeFromTimestamp(chart, @event.Timestamp) };
            }
            
            foreach (Bookmark bookmark in chart.Bookmarks)
            {
                bookmark.Timestamp = bookmark.Timestamp with { Time = Timestamp.TimeFromTimestamp(chart, bookmark.Timestamp) };
            }

            foreach (Note laneToggle in chart.LaneToggles)
            {
                laneToggle.Timestamp = laneToggle.Timestamp with { Time = Timestamp.TimeFromTimestamp(chart, laneToggle.Timestamp) };
            }

            foreach (Layer layer in chart.Layers)
            {
                foreach (Event @event in layer.Events)
                {
                    if (@event is StopEffectEvent stopEffectEvent)
                    {
                        foreach (EffectSubEvent subEvent in stopEffectEvent.SubEvents)
                        {
                            subEvent.Timestamp = subEvent.Timestamp with { Time = Timestamp.TimeFromTimestamp(chart, subEvent.Timestamp) };
                        }
                    }
                    else if (@event is ReverseEffectEvent reverseEffectEvent)
                    {
                        foreach (EffectSubEvent subEvent in reverseEffectEvent.SubEvents)
                        {
                            subEvent.Timestamp = subEvent.Timestamp with { Time = Timestamp.TimeFromTimestamp(chart, subEvent.Timestamp) };
                        }
                    }
                    else
                    {
                        @event.Timestamp = @event.Timestamp with { Time = Timestamp.TimeFromTimestamp(chart, @event.Timestamp) };
                    }
                }
                
                foreach (Note note in layer.Notes)
                {
                    if (note is HoldNote holdNote)
                    {
                        foreach (HoldPointNote point in holdNote.Points)
                        {
                            point.Timestamp = point.Timestamp with { Time = Timestamp.TimeFromTimestamp(chart, point.Timestamp) };
                        }
                    }
                    else
                    {
                        note.Timestamp = note.Timestamp with { Time = Timestamp.TimeFromTimestamp(chart, note.Timestamp) };
                    }
                }

                foreach (Note note in layer.GeneratedNotes)
                {
                    note.Timestamp = note.Timestamp with { Time = Timestamp.TimeFromTimestamp(chart, note.Timestamp) };
                }
            }

            entry.ChartEnd = entry.ChartEnd with { Time = Timestamp.TimeFromTimestamp(chart, entry.ChartEnd) };
        }

        /// <summary>
        /// Calculates millisecond timestamps scaled by speed changes for all objects in a chart.
        /// </summary>
        /// <param name="chart">The chart to calculate timestamps for.</param>
        public static void GenerateScaledTimeForAllObjects(Chart chart)
        {
            foreach (Layer layer in chart.Layers)
            {
                foreach (Event @event in layer.Events)
                {
                    if (@event is StopEffectEvent stopEffectEvent)
                    {
                        foreach (EffectSubEvent subEvent in stopEffectEvent.SubEvents)
                        {
                            subEvent.Timestamp = subEvent.Timestamp with { ScaledTime = Timestamp.ScaledTimeFromTime(layer, subEvent.Timestamp.Time) };
                        }
                    }
                    else if (@event is ReverseEffectEvent reverseEffectEvent)
                    {
                        foreach (EffectSubEvent subEvent in reverseEffectEvent.SubEvents)
                        {
                            subEvent.Timestamp = subEvent.Timestamp with { ScaledTime = Timestamp.ScaledTimeFromTime(layer, subEvent.Timestamp.Time) };
                        }
                    }
                    else
                    {
                        @event.Timestamp = @event.Timestamp with { ScaledTime = Timestamp.ScaledTimeFromTime(layer, @event.Timestamp.Time) };
                    }
                }
                
                foreach (Note note in layer.Notes)
                {
                    if (note is HoldNote holdNote)
                    {
                        foreach (HoldPointNote point in holdNote.Points)
                        {
                            point.Timestamp = point.Timestamp with { ScaledTime = Timestamp.ScaledTimeFromTime(layer, point.Timestamp.Time) };
                        }
                    }
                    else
                    {
                        note.Timestamp = note.Timestamp with { ScaledTime = Timestamp.ScaledTimeFromTime(layer, note.Timestamp.Time) };
                    }
                }
                
                foreach (Note note in layer.GeneratedNotes)
                {
                    if (note is not ITimeable timeable) continue;
                    
                    timeable.Timestamp = timeable.Timestamp with { ScaledTime = Timestamp.ScaledTimeFromTime(layer, timeable.Timestamp.Time) };
                }
            }
            
            if (chart.Layers.Count == 0)
            {
                foreach (Event @event in chart.Events)
                {
                    @event.Timestamp = @event.Timestamp with { ScaledTime = @event.Timestamp.Time };
                }
            }
            else
            {
                foreach (Event @event in chart.Events)
                {
                    @event.Timestamp = @event.Timestamp with { ScaledTime = Timestamp.ScaledTimeFromTime(chart.Layers[0], @event.Timestamp.Time) };
                }
            }
        }
        
        public static void GenerateAllMeasureLineAndSyncNotes(Chart chart, Timestamp end)
        {
            if (chart.Layers.Count == 0) return;
            
            foreach (Layer layer in chart.Layers)
            {
                layer.GeneratedNotes.Clear();
            }
            
            GenerateMeasureLineNotes(chart.Layers[0], end);
            GenerateBeatLineNotes(chart, chart.Layers[0], end);

            foreach (Layer layer in chart.Layers)
            {
                GenerateSyncNotes(layer);
                
                layer.GeneratedNotes = layer.GeneratedNotes.OrderBy(x => ((ITimeable)x).Timestamp).ToList();
            }
        }
        
        public static void GenerateMeasureLineNotes(Layer layer, Timestamp end)
        {
            for (int i = 0; i < end.Measure; i++)
            {
                layer.GeneratedNotes.Add(new MeasureLineNote(new(i, 0), false));
            }
        }

        public static void GenerateBeatLineNotes(Chart chart, Layer layer, Timestamp end)
        {
            List<MetreChangeEvent> metreChangeEvents = chart.Events.OfType<MetreChangeEvent>().ToList();

            for (int i = 0; i < metreChangeEvents.Count; i++)
            {
                int startTick = metreChangeEvents[i].Timestamp.FullTick;
                int endTick = i == metreChangeEvents.Count - 1
                    ? end.FullTick
                    : metreChangeEvents[i + 1].Timestamp.FullTick;

                int step = 1920 / metreChangeEvents[i].Upper;
                
                for (int j = startTick; j < endTick; j += step)
                {
                    if (j % 1920 == 0) continue;
                    layer.GeneratedNotes.Add(new MeasureLineNote(new(j), true));
                }
            }
        }

        public static void GenerateSyncNotes(Layer layer)
        {
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
        }*/    
    }
}