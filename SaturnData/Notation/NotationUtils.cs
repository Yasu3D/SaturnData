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
        return chart.Events.LastOrDefault(x => x is TempoChangeEvent t && t.Timestamp <= timestamp) as TempoChangeEvent;
    }
    
    public static TempoChangeEvent? LastTempoChange(Chart chart, float time)
    {
        return chart.Events.LastOrDefault(x => x is TempoChangeEvent t && t.Timestamp.Time <= time) as TempoChangeEvent;
    }

    public static MetreChangeEvent? LastMetreChange(Chart chart, Timestamp timestamp)
    {
        return chart.Events.LastOrDefault(x => x is MetreChangeEvent m && m.Timestamp <= timestamp) as MetreChangeEvent;
    }

    public static MetreChangeEvent? LastMetreChange(Chart chart, float time)
    {
        return chart.Events.LastOrDefault(x => x is MetreChangeEvent m && m.Timestamp.Time <= time) as MetreChangeEvent;
    }

    public static SpeedChangeEvent? LastSpeedChange(Layer layer, float time)
    {
        return layer.Events.LastOrDefault(x => x is SpeedChangeEvent s && s.Timestamp.Time <= time) as SpeedChangeEvent;
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
            if (@event is not ITimeable timeable) continue;
            timeable.Timestamp = timeable.Timestamp with { Time = Timestamp.TimeFromTimestamp(chart, timeable.Timestamp) };
        }
        
        foreach (Bookmark bookmark in chart.Bookmarks)
        {
            bookmark.Timestamp = bookmark.Timestamp with { Time = Timestamp.TimeFromTimestamp(chart, bookmark.Timestamp) };
        }

        foreach (Note laneToggle in chart.LaneToggles)
        {
            if (laneToggle is not ITimeable timeable) continue;
            timeable.Timestamp = timeable.Timestamp with { Time = Timestamp.TimeFromTimestamp(chart, timeable.Timestamp) };
        }

        foreach (Layer layer in chart.Layers)
        {
            foreach (Event @event in layer.Events)
            {
                if (@event is not ITimeable timeable) continue;
                timeable.Timestamp = timeable.Timestamp with { Time = Timestamp.TimeFromTimestamp(chart, timeable.Timestamp) };
            }
            
            foreach (Note note in layer.Notes)
            {
                if (note is not ITimeable timeable) continue;

                if (note is HoldNote holdNote)
                {
                    foreach (HoldPointNote point in holdNote.Points)
                    {
                        point.Timestamp = point.Timestamp with { Time = Timestamp.TimeFromTimestamp(chart, point.Timestamp) };
                    }
                }
                else
                {
                    timeable.Timestamp = timeable.Timestamp with { Time = Timestamp.TimeFromTimestamp(chart, timeable.Timestamp) };
                }
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
                if (@event is not ITimeable timeable) continue;
                timeable.Timestamp = timeable.Timestamp with { ScaledTime = Timestamp.ScaledTimeFromTime(layer, timeable.Timestamp.Time) };
            }
            
            foreach (Note note in layer.Notes)
            {
                if (note is not ITimeable timeable) continue;
                
                if (note is HoldNote holdNote)
                {
                    foreach (HoldPointNote point in holdNote.Points)
                    {
                        point.Timestamp = point.Timestamp with { ScaledTime = Timestamp.ScaledTimeFromTime(layer, point.Timestamp.Time) };
                    }
                }
                else
                {
                    timeable.Timestamp = timeable.Timestamp with { ScaledTime = Timestamp.ScaledTimeFromTime(layer, timeable.Timestamp.Time) };
                }
                
            }
        }
    }
}