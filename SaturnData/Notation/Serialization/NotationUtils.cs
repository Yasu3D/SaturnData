using System;
using System.Collections.Generic;
using System.Linq;
using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;
using SaturnData.Notation.Notes;

namespace SaturnData.Notation.Serialization;

internal static class NotationUtils
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
    
    internal static FormatVersion DetectFormatVersion(string[] lines)
    {
        FormatVersion version = FormatVersion.Unknown;
        foreach (string line in lines)
        {
            if (ContainsKey(line, "@SAT_VERSION ", out string value))
            {
                version = value switch
                {
                    "1" => FormatVersion.SatV1,
                    "2" => FormatVersion.SatV2,
                    "3" => FormatVersion.SatV3,
                    _ => FormatVersion.Unknown,
                };
                break;
            }

            if (line.StartsWith("#BODY"))
            {
                version = FormatVersion.Mer;
                break;
            }
        }
        
        return version;
    }

    internal static void PreProcessChart(Chart chart, NotationWriteArgs args)
    {
        if (args.BakeHoldNotes)
        {
            // TODO
        }
    }

    internal static void PreProcessEntry(Entry entry, NotationWriteArgs args)
    {
        entry.Level = MathF.Floor(entry.Level * 10) / 10;
        entry.ClearThreshold = MathF.Floor(entry.ClearThreshold * 100) / 100;
        
        if (entry.ChartEnd == null && entry.AutoChartEnd)
        {
            // TODO
        }
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
        entry.Level = MathF.Floor(entry.Level * 10) / 10;
        entry.ClearThreshold = MathF.Floor(entry.ClearThreshold * 100) / 100;
        
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
}