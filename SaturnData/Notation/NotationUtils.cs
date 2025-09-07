using System;
using System.Collections.Generic;
using System.Linq;
using SaturnData.Notation.Core;
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

    
    
    public static void CalculateTime(Chart chart)
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
                timeable.Timestamp = timeable.Timestamp with { Time = Timestamp.TimeFromTimestamp(chart, timeable.Timestamp) };
            }
        }
    }

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
                timeable.Timestamp = timeable.Timestamp with { ScaledTime = Timestamp.ScaledTimeFromTime(layer, timeable.Timestamp.Time) };
            }
        }
    }
}