using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SaturnData.Notation.Core;
using SaturnData.Notation.Events;
using SaturnData.Notation.Interfaces;
using SaturnData.Notation.Notes;

namespace SaturnData.Notation.Serialization.Mer;

public static class MerSerializer
{
    /// <summary>
    /// Converts a chart into a string.
    /// </summary>
    /// <param name="entry">The entry to serialize.</param>
    /// <param name="chart">The chart to serialize.</param>
    /// <returns></returns>
    public static string ToString(Entry entry, Chart chart)
    {
        return "";
    }

    /// <summary>
    /// Reads chart data and converts it into a chart.
    /// </summary>
    /// <param name="lines">Chart file data separated into individual lines.</param>
    /// <returns></returns>
    public static Chart ToChart(string[] lines)
    {
        Chart chart = new();

        ReverseEffectEvent? tempReverseEvent = null;
        StopEffectEvent? tempStopEvent = null;

        Dictionary<int, HoldNote> holdNotesByLastReference = new();
        HashSet<HoldPointNote> noRenderHoldPoints = [];
        
        foreach (string line in lines)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (split.Length < 3) continue;

                int measure = Convert.ToInt32(split[0], CultureInfo.InvariantCulture);
                int tick = Convert.ToInt32(split[1], CultureInfo.InvariantCulture);
                Timestamp timestamp = new(measure, tick);

                int objectType = Convert.ToInt32(split[2], CultureInfo.InvariantCulture);

                // Notes
                if (objectType is 1)
                {
                    int noteType = Convert.ToInt32(split[3], CultureInfo.InvariantCulture);
                    int position = Convert.ToInt32(split[5], CultureInfo.InvariantCulture);
                    int size = Convert.ToInt32(split[6], CultureInfo.InvariantCulture);

                    BonusType bonusType = noteType switch
                    {
                        2 or 3 or 6 or 8 => BonusType.Bonus,
                        20 or 21 or 22 or 23 or 24 or 25 or 26 => BonusType.R,
                        _ => BonusType.None,
                    };

                    // Touch Note
                    if (noteType is 1 or 2 or 20)
                    {
                        TouchNote touchNote = new(timestamp, position, size, bonusType, true);
                        NotationUtils.AddOrCreate(chart.NoteLayers, 0, touchNote);
                    }
                    
                    // Snap Forward Note
                    if (noteType is 3 or 21)
                    {
                        SnapForwardNote snapForwardNote = new(timestamp, position, size, bonusType, true);
                        NotationUtils.AddOrCreate(chart.NoteLayers, 0, snapForwardNote);
                    }
                    
                    // Snap Backward Note
                    if (noteType is 4 or 22)
                    {
                        SnapBackwardNote snapBackwardNote = new(timestamp, position, size, bonusType, true);
                        NotationUtils.AddOrCreate(chart.NoteLayers, 0, snapBackwardNote);
                    }
                    
                    // Slide Clockwise
                    if (noteType is 5 or 6 or 23)
                    {
                        SlideClockwiseNote slideClockwiseNote = new(timestamp, position, size, bonusType, true);
                        NotationUtils.AddOrCreate(chart.NoteLayers, 0, slideClockwiseNote);
                    }
                    
                    // Slide Counterclockwise
                    if (noteType is 7 or 8 or 24)
                    {
                        SlideCounterclockwiseNote slideCounterclockwiseNote = new(timestamp, position, size, bonusType, true);
                        NotationUtils.AddOrCreate(chart.NoteLayers, 0, slideCounterclockwiseNote);
                    }
                    
                    // Chain
                    if (noteType is 16 or 26)
                    {
                        ChainNote chainNote = new(timestamp, position, size, bonusType, true);
                        NotationUtils.AddOrCreate(chart.NoteLayers, 0, chainNote);
                    }
                    
                    // End of Chart
                    if (noteType is 14)
                    {
                        chart.ChartEnd = timestamp;
                    }
                    
                    // Hold Start
                    if (noteType is 9 or 25)
                    {
                        HoldNote holdNote = new(bonusType, true);
                        NotationUtils.AddOrCreate(chart.NoteLayers, 0, holdNote);
                        
                        // Add the index that this hold note references to the dictionary.
                        int reference = Convert.ToInt32(split[8],CultureInfo.InvariantCulture);
                        holdNotesByLastReference.Add(reference, holdNote);
                    }
                    
                    // Hold Point or Hold End
                    if (noteType is 10 or 11)
                    {
                        // Look for hold note in the dictionary that references this note's index.
                        // If it can't be found, skip.
                        int index = Convert.ToInt32(split[4], CultureInfo.InvariantCulture);
                        if (!holdNotesByLastReference.TryGetValue(index, out HoldNote holdNote)) continue;
                        
                        // Create and add hold point to parent hold.
                        int render = Convert.ToInt32(split[7], CultureInfo.InvariantCulture);
                        HoldPointNote holdPointNote = new(timestamp, position, size, holdNote);
                        holdNote.Points.Add(holdPointNote);
                        
                        // Ignore no render hold ends.
                        if (noteType == 10 && render == 0) noRenderHoldPoints.Add(holdPointNote);

                        // Update the parent hold note reference to the next segment.
                        int reference = Convert.ToInt32(split[8],CultureInfo.InvariantCulture);
                        holdNotesByLastReference.Add(reference, holdNote);
                    }
                }

                // Bpm Change Event
                if (objectType is 2)
                {
                    float bpm = Convert.ToSingle(split[3], CultureInfo.InvariantCulture);
                    BpmChangeEvent bpmChangeEvent = new(timestamp, bpm);

                    chart.GlobalEvents.Add(bpmChangeEvent);
                }

                // Time Signature Change Event
                if (objectType is 3)
                {
                    int upper = Convert.ToInt32(split[3], CultureInfo.InvariantCulture);
                    int lower = Convert.ToInt32(split[4], CultureInfo.InvariantCulture);
                    TimeSignatureChangeEvent timeSignatureChangeEvent = new(timestamp, upper, lower);

                    chart.GlobalEvents.Add(timeSignatureChangeEvent);
                }

                // HiSpeed Change Event
                if (objectType is 5)
                {
                    float hiSpeed = Convert.ToSingle(split[3], CultureInfo.InvariantCulture);
                    HiSpeedChangeEvent hiSpeedChangeEvent = new(timestamp, hiSpeed);

                    NotationUtils.AddOrCreate(chart.EventLayers, 0, hiSpeedChangeEvent);
                }

                // Reverse Effect Begin Event
                if (objectType is 6)
                {
                    tempReverseEvent = new();
                    tempReverseEvent.SubEvents[0] = new(timestamp, tempReverseEvent);
                }

                // Reverse Effect End Event / Reverse Note Capture Begin Event
                if (objectType is 7)
                {
                    if (tempReverseEvent?.SubEvents[0] == null) continue;
                    if (tempReverseEvent.SubEvents[0].Timestamp > timestamp) continue;

                    tempReverseEvent.SubEvents[1] = new(timestamp, tempReverseEvent);
                }

                // Reverse Note Capture End Event
                if (objectType is 8)
                {
                    if (tempReverseEvent?.SubEvents[0] == null) continue;
                    if (tempReverseEvent?.SubEvents[1] == null) continue;
                    if (tempReverseEvent.SubEvents[0].Timestamp > timestamp) continue;
                    if (tempReverseEvent.SubEvents[1].Timestamp > timestamp) continue;

                    tempReverseEvent.SubEvents[2] = new(timestamp, tempReverseEvent);
                    NotationUtils.AddOrCreate(chart.EventLayers, 0, tempReverseEvent);

                    tempReverseEvent = null;
                }

                // Stop Begin Event
                if (objectType is 9)
                {
                    tempStopEvent = new();
                    tempStopEvent.SubEvents[0] = new(timestamp, tempStopEvent);
                }

                // Stop End Event
                if (objectType is 10)
                {
                    if (tempStopEvent?.SubEvents[0] == null) continue;
                    if (tempStopEvent.SubEvents[0].Timestamp > timestamp) continue;

                    tempStopEvent.SubEvents[1] = new(timestamp, tempStopEvent);
                    NotationUtils.AddOrCreate(chart.EventLayers, 0, tempStopEvent);

                    tempStopEvent = null;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                // ignored
            }
        }

        // Filter out all no-render hold points.
        foreach (HoldPointNote holdPoint in noRenderHoldPoints)
        {
            holdPoint.Parent.Points.Remove(holdPoint);
        }
        
        return chart;
    }

    /// <summary>
    /// Reads chart data and converts it into an entry.
    /// </summary>
    /// <param name="lines">Chart file data separated into individual lines.</param>
    /// <returns></returns>
    public static Entry ToEntry(string[] lines)
    {
        Entry entry = new();
        
        foreach (string line in lines)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string value;
                
                // BAKKA COMPATIBILITY
                if (NotationUtils.ContainsKey(line, "#X_BAKKA_MUSIC_FILENAME ", out value)) { entry.AudioPath = value; }
                
                if (NotationUtils.ContainsKey(line, "#EDITOR_AUDIO ",           out value)) { entry.AudioPath = value; }
                if (NotationUtils.ContainsKey(line, "#EDITOR_LEVEL ",           out value)) { entry.Level = Convert.ToSingle(value); }
                if (NotationUtils.ContainsKey(line, "#EDITOR_AUTHOR ",          out value)) { entry.NotesDesigner = value; }
                if (NotationUtils.ContainsKey(line, "#EDITOR_PREVIEW_TIME ",    out value)) { entry.PreviewBegin = Convert.ToSingle(value) * 1000; }
                if (NotationUtils.ContainsKey(line, "#EDITOR_PREVIEW_LENGTH ",  out value)) { entry.PreviewDuration = Convert.ToSingle(value) * 1000; }
                if (NotationUtils.ContainsKey(line, "#EDITOR_OFFSET ",          out value)) { entry.AudioOffset = Convert.ToSingle(value) * 1000; }
                if (NotationUtils.ContainsKey(line, "#EDITOR_MOVIEOFFSET ",     out value)) { entry.VideoOffset = Convert.ToSingle(value) * 1000; }
            
                // WACK COMPATIBILITY
                if (NotationUtils.ContainsKey(line, "#LEVEL ",          out value)) { entry.Level = Convert.ToSingle(value); }
                if (NotationUtils.ContainsKey(line, "#AUDIO ",          out value)) { entry.AudioPath = value; }
                if (NotationUtils.ContainsKey(line, "#AUTHOR ",         out value)) { entry.NotesDesigner = value; }
                if (NotationUtils.ContainsKey(line, "#PREVIEW_TIME ",   out value)) { entry.PreviewBegin = Convert.ToSingle(value); }
                if (NotationUtils.ContainsKey(line, "#PREVIEW_LENGTH ", out value)) { entry.PreviewDuration = Convert.ToSingle(value); }
                
                // MER COMPATIBILITY
                if (NotationUtils.ContainsKey(line, "#MUSIC_FILE_PATH ", out value)) { entry.AudioPath = value; }
                if (NotationUtils.ContainsKey(line, "#OFFSET ",          out value)) { entry.AudioOffset = Convert.ToSingle(value); }
                if (NotationUtils.ContainsKey(line, "#MOVIEOFFSET ",     out value)) { entry.VideoOffset = Convert.ToSingle(value); }
            }
            catch
            {
                // ignored, continue
            }
        }
        
        return entry;
    }
}