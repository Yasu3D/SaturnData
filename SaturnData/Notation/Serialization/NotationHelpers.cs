using System;
using System.Collections.Generic;
using System.Linq;
using SaturnData.Notation.Core;
using SaturnData.Notation.Notes;

namespace SaturnData.Notation.Serialization;

internal static class NotationHelpers
{
    private enum HoldDirection
    {
        Clockwise,
        Counterclockwise,
        Symmetrical,
        None,
    }

    private enum HoldSide
    {
        Left = 0,
        Right = 1,
    }
    
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
        // TODO: Optimize this?
        // Also maybe find a way to make it behave 1:1 like the renderer.
        
        if (source.Points.Count < 2) return source;
        
        HoldNote holdNote = new(source.BonusType, source.JudgementType);

        for (int i = 0; i < source.Points.Count - 1; i++)
        {
            HoldPointNote sourceStart = source.Points[i];
            HoldPointNote sourceEnd = source.Points[i + 1];

            HoldPointNote start = new(sourceStart.Timestamp, sourceStart.Position, sourceStart.Size, holdNote, sourceStart.RenderType);
            HoldPointNote end = new(sourceEnd.Timestamp, sourceEnd.Position, sourceEnd.Size, holdNote, sourceEnd.RenderType);

            holdNote.Points.Add(start);

            // Get global position.
            int startLeftEdge = start.Position;
            int startRightEdge = start.Position + start.Size;

            int endLeftEdge = end.Position;
            int endRightEdge = end.Position + end.Size;
            
            // Calculate offsets from start note edges.
            int leftEdgeOffsetCcw = modulo(endLeftEdge - startLeftEdge, 60);
            int leftEdgeOffsetCw = modulo(startLeftEdge - endLeftEdge, 60);
            
            int rightEdgeOffsetCcw = modulo(endRightEdge - startRightEdge, 60);
            int rightEdgeOffsetCw = modulo(startRightEdge - endRightEdge, 60);
            
            // Find the shortest direction for each edge.
            HoldDirection leftDirection = leftEdgeOffsetCcw < leftEdgeOffsetCw ? HoldDirection.Counterclockwise : HoldDirection.Clockwise;
            if (leftEdgeOffsetCcw == 0 && leftEdgeOffsetCw == 0) leftDirection = HoldDirection.None;
            
            HoldDirection rightDirection = rightEdgeOffsetCcw < rightEdgeOffsetCw ? HoldDirection.Counterclockwise : HoldDirection.Clockwise;
            if (rightEdgeOffsetCcw == 0 && rightEdgeOffsetCw == 0) rightDirection = HoldDirection.None;

            // Direction that's NOT set to none with the smallest offset takes priority.
            // If they're equal, default to left edge's preferred direction.
            // Again, it's quite verbose and could be simplified, but I'd rather
            // keep the logic very readable.
            int leftEdgeOffsetMin = Math.Min(leftEdgeOffsetCcw, leftEdgeOffsetCw);
            int rightEdgeOffsetMin = Math.Min(rightEdgeOffsetCcw, rightEdgeOffsetCw);
            
            HoldDirection shortestDirection;
            HoldSide shortestSide;

            if (leftDirection == HoldDirection.None)
            {
                shortestDirection = start.Size > end.Size ? HoldDirection.Clockwise : HoldDirection.Counterclockwise;
                shortestSide = HoldSide.Right;
            }
            else if (rightDirection == HoldDirection.None)
            {
                shortestDirection = start.Size > end.Size ? HoldDirection.Counterclockwise : HoldDirection.Clockwise;
                shortestSide = HoldSide.Left;
            }
            else if (leftEdgeOffsetMin < rightEdgeOffsetMin)
            {
                shortestDirection = leftDirection;
                shortestSide = HoldSide.Left;
            }
            else
            {
                shortestDirection = rightDirection;
                shortestSide = HoldSide.Right;
            }
            
            // If one hold completely encases the other (overlaps),
            // a special third HoldDirection needs to be used.
            bool isOverlapping = isFullyOverlapping(startLeftEdge, startRightEdge, endLeftEdge, endRightEdge);
            HoldDirection finalDirection = isOverlapping ? HoldDirection.Symmetrical : shortestDirection;
            
            // Get final signed offsets
            int signedLeftEdgeOffset = finalDirection switch
            {
                HoldDirection.Clockwise => -leftEdgeOffsetCw,
                HoldDirection.Counterclockwise => leftEdgeOffsetCcw,
                HoldDirection.Symmetrical => shortestSide == HoldSide.Left
                ? shortestDirection == HoldDirection.Counterclockwise ? leftEdgeOffsetCcw : -leftEdgeOffsetCw
                : shortestDirection != HoldDirection.Counterclockwise ? leftEdgeOffsetCcw : -leftEdgeOffsetCw,
                _ => throw new ArgumentOutOfRangeException(),
            };

            int signedRightEdgeOffset = finalDirection switch
            {
                HoldDirection.Clockwise => -rightEdgeOffsetCw,
                HoldDirection.Counterclockwise => rightEdgeOffsetCcw,
                HoldDirection.Symmetrical => shortestSide == HoldSide.Right
                ? shortestDirection == HoldDirection.Counterclockwise ? rightEdgeOffsetCcw : -rightEdgeOffsetCw
                : shortestDirection != HoldDirection.Counterclockwise ? rightEdgeOffsetCcw : -rightEdgeOffsetCw,
                _ => throw new ArgumentOutOfRangeException(),
            };
            
            // Create local positions relative to start note.
            int localEndLeftEdge = startLeftEdge + signedLeftEdgeOffset;
            int localEndRightEdge = startRightEdge + signedRightEdgeOffset;
            
            // Get number of steps between start and end
            int steps = Math.Max(Math.Abs(signedLeftEdgeOffset), Math.Abs(signedRightEdgeOffset));
            
            for (int j = 1; j < steps; j++)
            {
                float t = (float)j / steps;

                int fullTick = (int)lerp(start.Timestamp.FullTick, end.Timestamp.FullTick, t);
                int leftEdge = (int)Math.Round(lerp(startLeftEdge, localEndLeftEdge, t));
                int rightEdge = (int)Math.Round(lerp(startRightEdge, localEndRightEdge, t));
                
                int position = modulo(leftEdge, 60);
                int size = Math.Clamp(rightEdge - leftEdge, 1, 60);

                holdNote.Points.Add(new(new(fullTick), position, size, holdNote, HoldPointRenderType.Hidden));
            }
            
            if (i == source.Points.Count - 2)
            {
                holdNote.Points.Add(end);
            }
        }
        
        return holdNote;

        float lerp(float a, float b, float t)
        {
            return a + t * (b - a);
        }

        int modulo(int x, int m)
        {
            if (m <= 0) return 0;
        
            int r = x % m;
            return r < 0 ? r + m : r;
        }
        
        bool isFullyOverlapping(int startLeftEdge, int startRightEdge, int endLeftEdge, int endRightEdge)
        {
            // Start and End are identical
            if (startLeftEdge == endLeftEdge && startRightEdge == endRightEdge) return true;
            
            // Size 60 start
            if (startLeftEdge == startRightEdge)
            {
                startRightEdge -= 1;
            }
            
            // Size 60 end
            if (endLeftEdge == endRightEdge)
            {
                endRightEdge -= 1;
            }
            
            // Start overflows - End does not overflow
            if (startRightEdge >= 60 && endRightEdge < 60 && Math.Abs(startRightEdge - endRightEdge) >= 60)
            {
                endLeftEdge += 60;
            }
            
            // End overflows - Start does not overflow
            else if (endRightEdge >= 60 && startRightEdge < 60 && Math.Abs(startRightEdge - endRightEdge) >= 60)
            {
                startLeftEdge += 60;
            }
            
            bool caseA = startLeftEdge >= endLeftEdge && startRightEdge <= endRightEdge; // start smaller than end
            bool caseB = startLeftEdge <= endLeftEdge && startRightEdge >= endRightEdge; // start bigger than end
            
            return caseA || caseB;
        }
    }
}