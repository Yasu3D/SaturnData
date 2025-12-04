using System;
using System.Collections.Generic;
using System.Linq;
using SaturnData.Notation.Core;
using SaturnData.Notation.Interfaces;
using SaturnData.Notation.Notes;
using SaturnData.Utilities;

namespace SaturnData.Notation.Serialization;

internal static class NotationHelpers
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
    
    internal static void PostProcessChart(Chart chart, NotationReadArgs args)
    {
        if (args.SortCollections)
        {
            chart.Events = chart.Events.OrderBy(x => x.Timestamp.FullTick).ToList();
            chart.LaneToggles = chart.LaneToggles.OrderBy(x => x.Timestamp.FullTick).ToList();
            chart.Bookmarks = chart.Bookmarks.OrderBy(x => x.Timestamp.FullTick).ToList();

            foreach (Layer layer in chart.Layers)
            {
                layer.Events = layer.Events.OrderBy(x => x.Timestamp.FullTick).ToList();
                layer.Notes = layer.Notes.OrderBy(x => x.Timestamp.FullTick).ToList();
                layer.GeneratedNotes = layer.GeneratedNotes.OrderBy(x => x.Timestamp.FullTick).ToList();
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
    
    internal static HoldNote BakeHoldNote(HoldNote source)
    {
        if (source.Points.Count < 2) return source;
        
        HoldNote holdNote = new(source.BonusType, source.JudgementType);

        for (int i = 0; i < source.Points.Count - 1; i++)
        {
            HoldPointNote sourceStart = source.Points[i];
            HoldPointNote sourceEnd = source.Points[i + 1];

            HoldPointNote start = new(sourceStart.Timestamp, sourceStart.Position, sourceStart.Size, holdNote, sourceStart.RenderType);
            HoldPointNote end = new(sourceEnd.Timestamp, sourceEnd.Position, sourceEnd.Size, holdNote, sourceEnd.RenderType);

            holdNote.Points.Add(start);

            float startCenter = (start.Position + start.Size * 0.5f) * -6;
            float endCenter   = (end.Position   + end.Size   * 0.5f) * -6;

            bool flip = SaturnMath.FlipHoldInterpolation(startCenter, endCenter);
            int steps;

            int startLeft = start.Position;
            int endLeft = end.Position;
            int startRight = start.Position + start.Size;
            int endRight = end.Position + end.Size;
            
            int deltaLeft = Math.Abs(startLeft - endLeft);
            int deltaRight = Math.Abs(startRight - endRight);
            deltaLeft = flip ? 60 - deltaLeft : deltaLeft;
            deltaRight = flip ? 60 - deltaRight : deltaRight;

            int signLeft = Math.Sign(startLeft - endLeft);
            int signRight = Math.Sign(startRight - endRight);
            signLeft = flip ? signLeft : -signLeft;
            signRight = flip ? signRight : -signRight;
            
            steps = Math.Max(deltaLeft, deltaRight);
            
            for (int j = 1; j < steps; j++)
            {
                float t = (float)j / steps;

                int fullTick = (int)SaturnMath.Lerp(start.Timestamp.FullTick, end.Timestamp.FullTick, t);
                int leftEdge = (int)Math.Round(SaturnMath.Lerp(startLeft, startLeft + deltaLeft * signLeft, t));
                int rightEdge = (int)Math.Round(SaturnMath.Lerp(startRight, startRight + deltaRight * signRight, t));
                
                int position = SaturnMath.Modulo(leftEdge, 60);
                int size = Math.Clamp(rightEdge - leftEdge, 1, 60);

                holdNote.Points.Add(new(new(fullTick), position, size, holdNote, HoldPointRenderType.Hidden));
            }
            
            if (i == source.Points.Count - 2)
            {
                holdNote.Points.Add(end);
            }
        }
        
        return holdNote;
    }

    internal static List<ILaneToggle> BakeLaneToggle(ILaneToggle source)
    {
        if (source.Direction != LaneSweepDirection.Instant) return [source];

        ITimeable sourceTimeable = (ITimeable)source;
        IPositionable sourcePositionable = (IPositionable)source;
        bool laneShow = source is LaneShowNote;
        
        List<ILaneToggle> result = [];
        
        for (int i = sourcePositionable.Position; i < sourcePositionable.Position + sourcePositionable.Size; i++)
        {
            ILaneToggle part = laneShow
                ? new LaneShowNote(new(sourceTimeable.Timestamp), i % 60, 1, LaneSweepDirection.Center)
                : new LaneHideNote(new(sourceTimeable.Timestamp), i % 60, 1, LaneSweepDirection.Center);

            result.Add(part);
        }

        return result;
    }
}