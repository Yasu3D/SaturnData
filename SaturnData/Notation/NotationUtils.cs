using System;
using System.Collections.Generic;
using System.Linq;
using SaturnData.Notation.Core;
using SaturnData.Notation.Events;
using SaturnData.Notation.Interfaces;
using SaturnData.Notation.Notes;
using SaturnData.Notation.Serialization;

namespace SaturnData.Notation;

public static class NotationUtils
{
    internal static void AddOrCreate(List<Layer> layers, string key, Note note)
    {
        Layer layer = layers.FirstOrDefault(x => x.Name == key) ?? new(key);
        layer.Notes.Add(note);

        if (!layers.Contains(layer))
        {
            layers.Add(layer);
        }
    }
    
    internal static void AddOrCreate(List<Layer> layers, string key, Event @event)
    {
        Layer layer = layers.FirstOrDefault(x => x.Name == key) ?? new(key);
        layer.Events.Add(@event);
        
        if (!layers.Contains(layer))
        {
            layers.Add(layer);
        }
    }
    
    internal static bool ContainsKey(string input, string key, out string value)
    {
        if (input.StartsWith(key))
        {
            value = input[(input.IndexOf(key, StringComparison.Ordinal) + key.Length)..].Trim();
            return true;
        }

        value = "";
        return false;
    }

    internal static void PreProcessChart(Chart chart, NotationWriteArgs args)
    {
        // TODO bake hold notes
    }

    internal static void PreProcessEntry(Entry entry, NotationWriteArgs args)
    {
        //entry.Level = MathF.Floor(entry.Level * 10) / 10;
        //entry.ClearThreshold = MathF.Floor(entry.ClearThreshold * 100) / 100;
        //
        //if (entry.AutoChartEnd)
        //{
        //    // TODO
        //}
    }
    
    internal static void PostProcessChart(Chart chart, NotationReadArgs args)
    {
        if (args.SortCollections)
        {
            chart.Events = chart.Events.OrderBy(x => ((ITimeable)x).Timestamp.FullTick).ToList();
            chart.LaneToggles = chart.LaneToggles.OrderBy(x => ((ITimeable)x).Timestamp.FullTick).ToList();
            chart.Bookmarks = chart.Bookmarks.OrderBy(x => x.Timestamp.FullTick).ToList();

            foreach (Layer layer in chart.Layers)
            {
                layer.Events = layer.Events.OrderBy(x => ((ITimeable)x).Timestamp.FullTick).ToList();
                layer.Notes = layer.Notes.OrderBy(x => ((ITimeable)x).Timestamp.FullTick).ToList();
            }
        }
        
        if (args.OptimizeHoldNotes)
        {
            foreach (Layer layer in chart.Layers)
            foreach (Note note in layer.Notes)
            {
                if (note is not HoldNote holdNote) continue;

                foreach (HoldPointNote point in holdNote.Points.ToArray())
                {
                    if (point.RenderType == HoldPointRenderType.Visible) continue;
                    
                    holdNote.Points.Remove(point);
                }
            }
        }
    }

    internal static void PostProcessEntry(Entry entry, NotationReadArgs args)
    {
        if (args.InferClearThresholdFromDifficulty)
        {
            entry.ClearThreshold = entry.Difficulty switch
            {
                Difficulty.None => 0.45f,
                Difficulty.Normal => 0.45f,
                Difficulty.Hard => 0.55f,
                Difficulty.Expert => 0.8f,
                Difficulty.Inferno => 0.8f,
                Difficulty.WorldsEnd => 0.8f,
                _ => 0.45f,
            };
        }  
    }

    public static TempoChangeEvent? LastTempoChange(Chart chart, Timestamp timestamp)
    {
        if (timestamp.FullTick == 0)
        {
            return chart.Events.LastOrDefault(x => x is TempoChangeEvent && x.Timestamp.FullTick == 0) as TempoChangeEvent;
        }
        
        return chart.Events.OrderBy(x => x.Timestamp).LastOrDefault(x => x is TempoChangeEvent && x.Timestamp < timestamp) as TempoChangeEvent;
    }
    
    public static TempoChangeEvent? LastTempoChange(Chart chart, float time)
    {
        if (time == 0)
        {
            return chart.Events.LastOrDefault(x => x is TempoChangeEvent && x.Timestamp.Time == 0) as TempoChangeEvent;
        }
        
        return chart.Events.OrderBy(x => x.Timestamp).LastOrDefault(x => x is TempoChangeEvent && x.Timestamp.Time < time) as TempoChangeEvent;
    }

    public static MetreChangeEvent? LastMetreChange(Chart chart, Timestamp timestamp)
    {
        if (timestamp.FullTick == 0)
        {
            return chart.Events.LastOrDefault(x => x is MetreChangeEvent && x.Timestamp.FullTick == 0) as MetreChangeEvent;
        }
        
        return chart.Events.OrderBy(x => x.Timestamp).LastOrDefault(x => x is MetreChangeEvent && x.Timestamp < timestamp) as MetreChangeEvent;
    }

    public static MetreChangeEvent? LastMetreChange(Chart chart, float time)
    {
        if (time == 0)
        {
            return chart.Events.LastOrDefault(x => x is MetreChangeEvent && x.Timestamp.Time == 0) as MetreChangeEvent;
        }
        
        return chart.Events.OrderBy(x => x.Timestamp).LastOrDefault(x => x is MetreChangeEvent && x.Timestamp.Time < time) as MetreChangeEvent;
    }

    public static SpeedChangeEvent? LastSpeedChange(Layer layer, float time)
    {
        if (time == 0)
        {
            return layer.Events.LastOrDefault(x => x is SpeedChangeEvent && x.Timestamp.Time == 0) as SpeedChangeEvent;
        }
        
        return layer.Events.OrderBy(x => x.Timestamp).LastOrDefault(x => x is SpeedChangeEvent && x.Timestamp.Time < time) as SpeedChangeEvent;
    }

    public static VisibilityChangeEvent? LastVisibilityChange(Layer layer, float time)
    {
        if (time == 0)
        {
            return layer.Events.LastOrDefault(x => x is VisibilityChangeEvent && x.Timestamp.Time == 0) as VisibilityChangeEvent;
        }
        
        return layer.Events.OrderBy(x => x.Timestamp).LastOrDefault(x => x is VisibilityChangeEvent && x.Timestamp.Time < time) as VisibilityChangeEvent;
    }
    
    /// <summary>
    /// Calculates the ideal chart end timestamp, based on all objects in a chart and the length of audio.
    /// </summary>
    /// <param name="chart">The chart to base the timestamp off of.</param>
    /// <param name="audioLength">The length of the audio file in milliseconds.(optional)</param>
    /// <returns></returns>
    public static Timestamp CalculateIdealChartEnd(Chart chart, float audioLength = 0)
    {
        Timestamp audioEnd = Timestamp.TimestampFromTime(chart, audioLength, 1920);
        Timestamp chartEnd = Timestamp.Zero;

        foreach (Layer layer in chart.Layers)
        foreach (Note note in layer.Notes)
        {
            if (note is not ITimeable timeable) continue;
            if (note is not IPlayable playable) continue;

            if (playable.JudgementType is JudgementType.Fake) continue; // Allow for fake notes behind end
            
            chartEnd = Timestamp.Max(chartEnd, timeable.Timestamp);
        }
        
        chartEnd = Timestamp.TimestampFromTime(chart, chartEnd.Time + 2000, 1920);
        
        // Round up to next measure.
        if (chartEnd.Tick != 0)
        {
            chartEnd.Measure += 1;
            chartEnd.Tick = 0;
            chartEnd.Time = Timestamp.TimeFromTimestamp(chart, chartEnd);
        }
        
        if (audioEnd.Tick != 0)
        {
            audioEnd.Measure += 1;
            audioEnd.Tick = 0;
            audioEnd.Time = Timestamp.TimeFromTimestamp(chart, audioEnd);
        }

        return Timestamp.Max(chartEnd, audioEnd);
    }
    
    /// <summary>
    /// Calculates millisecond timestamps for all objects in an entry and a chart.
    /// </summary>
    /// <param name="entry">The entry to calculate timestamps for.</param>
    /// <param name="chart">The chart to calculate timestamps for.</param>
    public static void CalculateTime(Entry entry, Chart chart)
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
    public static void CalculateScaledTime(Chart chart)
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
    
    /// <summary>
    /// Determines if two notes should have a "Sync" outline or not.
    /// </summary>
    public static bool IsSync(Note current, Note? other)
    {
        if (other == null) return false;

        if (current is ChainNote chain0 && chain0.BonusType != BonusType.R) return false;
        if (other is ChainNote chain1 && chain1.BonusType != BonusType.R) return false;

        if (current is not IPositionable pos0) return false;
        if (other is not IPositionable pos1) return false;

        if (pos0.Position == pos1.Position && pos0.Size == pos1.Size) return false;

        if (current is not ITimeable time0) return false;
        if (other is not ITimeable time1) return false;

        if (time0.Timestamp != time1.Timestamp) return false;

        return true;
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

            
            if (!IsSync(current, previous)) continue;
            if (current is not IPositionable currentPositionable) continue;
            if (previous is not IPositionable previousPositionable) continue;

            if (current is not ITimeable currentTimeable) continue;

            // -1 + 60
            // => + 59
            int position0 = (currentPositionable.Position + currentPositionable.Size + 59) % 60;
            int position1 = (previousPositionable.Position + previousPositionable.Size + 59) % 60;

            int size0 = ((previousPositionable.Position - position0 + 60) % 60) + 1;
            int size1 = ((currentPositionable.Position - position1 + 60) % 60) + 1;

            int finalPosition = size0 < size1 ? position0 : position1;
            int finalSize = Math.Min(size0, size1);

            if (finalSize > 30) continue;

            SyncNote syncNote = new(currentTimeable.Timestamp, finalPosition, finalSize);
            layer.GeneratedNotes.Add(syncNote);
        }
    }
}