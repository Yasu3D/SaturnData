using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SaturnData.Notation.Core;
using SaturnData.Notation.Events;
using SaturnData.Notation.Interfaces;
using SaturnData.Notation.Notes;

namespace SaturnData.Notation.Serialization.Mer;

public static class MerReader
{
    private class MerReaderHoldNote
    {
        public int Measure;
        
        public int Tick;
        
        public int Index;
        
        public int Position;
        
        public int Size;

        public BonusType BonusType;
        
        public int Render;
        
        public int? Reference;
    }

    /// <summary>
    /// Reads chart data and converts it into a chart.
    /// </summary>
    /// <param name="lines">Chart file data separated into individual lines.</param>
    /// <returns></returns>
    internal static Chart ToChart(string[] lines, NotationReadOptions options)
    {
        Chart chart = new();
        
        ReverseEffectEvent? tempReverseEvent = null;
        StopEffectEvent? tempStopEvent = null;
        
        // Hold note linking/creation
        List<MerReaderHoldNote> merHoldNotes = [];
        HashSet<MerReaderHoldNote> checkedMerHoldNotes = [];

        int startIndex = Array.IndexOf(lines, "#BODY");
        if (startIndex == -1) return chart;
        
        for (int i = startIndex + 1; i < lines.Length; i++)
        {
            string line = lines[i];
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

                    // Hold Start / Point / End
                    if (noteType is 9 or 10 or 11 or 25)
                    {
                        int index = Convert.ToInt32(split[4], CultureInfo.InvariantCulture);
                        int render = Convert.ToInt32(split[7], CultureInfo.InvariantCulture);
                        int? reference = null;
                        if (split.Length == 9)
                        {
                            reference = Convert.ToInt32(split[8], CultureInfo.InvariantCulture);
                        }

                        MerReaderHoldNote merReaderHoldNote = new()
                        {
                            Measure = measure,
                            Tick = tick,
                            Index = index,
                            Position = position,
                            Size = size,
                            BonusType = noteType == 25 ? BonusType.R : BonusType.None,
                            Render = render,
                            Reference = reference,
                        };

                        merHoldNotes.Add(merReaderHoldNote);
                    }

                    // Mask Add
                    if (noteType is 12)
                    {
                        MaskDirection direction = (MaskDirection)Convert.ToInt32(split[8], CultureInfo.InvariantCulture);
                        MaskAddNote maskAddNote = new(timestamp, position, size, direction);
                        chart.Masks.Add(maskAddNote);
                    }

                    // Mask Remove
                    if (noteType is 13)
                    {
                        MaskDirection direction = (MaskDirection)Convert.ToInt32(split[8], CultureInfo.InvariantCulture);
                        MaskSubtractNote maskSubtractNote = new(timestamp, position, size, direction);
                        chart.Masks.Add(maskSubtractNote);
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
                    int lower = split.Length == 4 // Older files can omit the lower. In that case set it to 4.
                        ? 4
                        : Convert.ToInt32(split[4], CultureInfo.InvariantCulture);
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                // ignored, continue to the next line.
            }
        }

        // Create real hold notes from temporary Mer holds.
        // This is *incredibly* inefficient...
        foreach (MerReaderHoldNote merHoldNote in merHoldNotes)
        {
            if (checkedMerHoldNotes.Contains(merHoldNote)) continue;

            // Find first note in the chain.
            MerReaderHoldNote? current = merHoldNote;
            MerReaderHoldNote? previousReference = merHoldNotes.FirstOrDefault(x => current.Index == x.Reference);
            
            int steps = 0; // failsafe if there's a cyclic reference.
            while (previousReference != null && steps < merHoldNotes.Count)
            {
                steps++;
                
                current = previousReference;
                previousReference = merHoldNotes.FirstOrDefault(x => current.Index == x.Reference);
            }

            HoldNote holdNote = new(current.BonusType, true);

            steps = 0; // failsafe if there's a cyclic reference.
            while (current != null && steps < merHoldNotes.Count)
            {
                checkedMerHoldNotes.Add(current);
                steps++;
                
                HoldPointNote holdPointNote = new(new(current.Measure, current.Tick), current.Position, current.Size, (HoldPointRenderBehaviour)current.Render);
                holdNote.Points.Add(holdPointNote);
                
                current = current.Reference == null
                    ? null
                    : merHoldNotes.FirstOrDefault(x => x.Index == current.Reference);
            }

            holdNote.Points = holdNote.Points
                .OrderBy(x => x.Timestamp)
                .ToList();
            
            NotationUtils.AddOrCreate(chart.NoteLayers, 0, holdNote);
        }

        NotationUtils.PostProcessChart(chart, options);
        
        return chart;
    }

    /// <summary>
    /// Reads chart data and converts it into an entry.
    /// </summary>
    /// <param name="lines">Chart file data separated into individual lines.</param>
    /// <returns></returns>
    internal static Entry ToEntry(string[] lines)
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
                if (NotationUtils.ContainsKey(line, "#EDITOR_LEVEL ",           out value)) { entry.Level = Convert.ToSingle(value, CultureInfo.InvariantCulture); }
                if (NotationUtils.ContainsKey(line, "#EDITOR_AUTHOR ",          out value)) { entry.NotesDesigner = value; }
                if (NotationUtils.ContainsKey(line, "#EDITOR_PREVIEW_TIME ",    out value)) { entry.PreviewBegin = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }
                if (NotationUtils.ContainsKey(line, "#EDITOR_PREVIEW_LENGTH ",  out value)) { entry.PreviewDuration = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }
                if (NotationUtils.ContainsKey(line, "#EDITOR_OFFSET ",          out value)) { entry.AudioOffset = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }
                if (NotationUtils.ContainsKey(line, "#EDITOR_MOVIEOFFSET ",     out value)) { entry.VideoOffset = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }
            
                // WACK COMPATIBILITY
                if (NotationUtils.ContainsKey(line, "#LEVEL ",          out value)) { entry.Level = Convert.ToSingle(value); }
                if (NotationUtils.ContainsKey(line, "#AUDIO ",          out value)) { entry.AudioPath = value; }
                if (NotationUtils.ContainsKey(line, "#AUTHOR ",         out value)) { entry.NotesDesigner = value; }
                if (NotationUtils.ContainsKey(line, "#PREVIEW_TIME ",   out value)) { entry.PreviewBegin = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }
                if (NotationUtils.ContainsKey(line, "#PREVIEW_LENGTH ", out value)) { entry.PreviewDuration = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }
                
                // MER COMPATIBILITY
                if (NotationUtils.ContainsKey(line, "#MUSIC_FILE_PATH ", out value)) { entry.AudioPath = value; }
                if (NotationUtils.ContainsKey(line, "#OFFSET ",          out value)) { entry.AudioOffset = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }
                if (NotationUtils.ContainsKey(line, "#MOVIEOFFSET ",     out value)) { entry.VideoOffset = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }
                
                // PROTOTYPE MER COMPATIBILITY
                if (NotationUtils.ContainsKey(line, "#MUSIC_NAME @JPN ",               out value)) { entry.Title = value; }
                if (NotationUtils.ContainsKey(line, "#MUSIC_NAME_RUBY @JPN ",          out value)) { entry.Reading = value; }
                if (NotationUtils.ContainsKey(line, "#ARTIST_NAME @JPN ",              out value)) { entry.Artist = value; }
                if (NotationUtils.ContainsKey(line, "#MUSIC_SCORE_CREATOR_NAME @JPN ", out value)) { entry.NotesDesigner = value; }
                if (NotationUtils.ContainsKey(line, "#JACKET_IMAGE_PATH ",             out value)) { entry.JacketPath = value; }
                if (NotationUtils.ContainsKey(line, "#DIFFICULTY ",                    out value)) { entry.Level = Convert.ToSingle(value, CultureInfo.InvariantCulture); }
                if (NotationUtils.ContainsKey(line, "#DISPLAY_BPM ",                   out value)) { entry.BpmMessage = value; }
            }
            catch
            {
                // ignored, continue
            }
        }
        
        return entry;
    }
}