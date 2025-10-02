using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
    internal static Chart ToChart(string[] lines, NotationReadArgs args, out List<Exception> exceptions)
    {
        Chart chart = new();
        exceptions = [];
        
        ReverseEffectEvent? tempReverseEvent = null;
        StopEffectEvent? tempStopEvent = null;
        
        // Hold note linking/creation
        List<MerReaderHoldNote> merHoldNotes = [];
        HashSet<MerReaderHoldNote> checkedMerHoldNotes = [];

        int startIndex = Array.IndexOf(lines, "#BODY");
        if (startIndex == -1)
        {
            Exception exception = new(ErrorList.ErrorMer001);
            Console.WriteLine(exception);
            exceptions.Add(exception);
            return chart;
        }
        
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
                        2 or 6 or 8 => BonusType.Bonus,
                        20 or 21 or 22 or 23 or 24 or 25 or 26 => BonusType.R,
                        _ => BonusType.Normal,
                    };

                    // Touch Note
                    if (noteType is 1 or 2 or 20)
                    {
                        TouchNote touchNote = new(timestamp, position, size, bonusType, JudgementType.Normal);
                        NotationUtils.AddOrCreate(chart.Layers, "Layer 0", touchNote);
                    }

                    // Snap Forward Note
                    if (noteType is 3 or 21)
                    {
                        SnapForwardNote snapForwardNote = new(timestamp, position, size, bonusType, JudgementType.Normal);
                        NotationUtils.AddOrCreate(chart.Layers, "Layer 0", snapForwardNote);
                    }

                    // Snap Backward Note
                    if (noteType is 4 or 22)
                    {
                        SnapBackwardNote snapBackwardNote = new(timestamp, position, size, bonusType, JudgementType.Normal);
                        NotationUtils.AddOrCreate(chart.Layers, "Layer 0", snapBackwardNote);
                    }

                    // Slide Clockwise
                    if (noteType is 5 or 6 or 23)
                    {
                        SlideClockwiseNote slideClockwiseNote = new(timestamp, position, size, bonusType, JudgementType.Normal);
                        NotationUtils.AddOrCreate(chart.Layers, "Layer 0", slideClockwiseNote);
                    }

                    // Slide Counterclockwise
                    if (noteType is 7 or 8 or 24)
                    {
                        SlideCounterclockwiseNote slideCounterclockwiseNote = new(timestamp, position, size, bonusType, JudgementType.Normal);
                        NotationUtils.AddOrCreate(chart.Layers, "Layer 0", slideCounterclockwiseNote);
                    }

                    // Chain
                    if (noteType is 16 or 26)
                    {
                        ChainNote chainNote = new(timestamp, position, size, bonusType, JudgementType.Normal);
                        NotationUtils.AddOrCreate(chart.Layers, "Layer 0", chainNote);
                    }
                    
                    // End of Chart handled in ToEntry().

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
                            BonusType = bonusType,
                            Render = render,
                            Reference = reference,
                        };

                        merHoldNotes.Add(merReaderHoldNote);
                    }

                    // Lane Show
                    if (noteType is 12)
                    {
                        LaneSweepDirection direction = (LaneSweepDirection)Convert.ToInt32(split[8], CultureInfo.InvariantCulture);
                        LaneShowNote laneShowNote = new(timestamp, position, size, direction);
                        chart.LaneToggles.Add(laneShowNote);
                    }

                    // Lane Hide
                    if (noteType is 13)
                    {
                        LaneSweepDirection direction = (LaneSweepDirection)Convert.ToInt32(split[8], CultureInfo.InvariantCulture);
                        LaneHideNote laneHideNote = new(timestamp, position, size, direction);
                        chart.LaneToggles.Add(laneHideNote);
                    }
                }

                // Bpm Change Event
                if (objectType is 2)
                {
                    float bpm = Convert.ToSingle(split[3], CultureInfo.InvariantCulture);
                    TempoChangeEvent tempoChangeEvent = new(timestamp, bpm);

                    chart.Events.Add(tempoChangeEvent);
                }

                // Time Signature Change Event
                if (objectType is 3)
                {
                    int upper = Convert.ToInt32(split[3], CultureInfo.InvariantCulture);
                    int lower = split.Length == 4 // Older files can omit the lower. In that case set it to 4.
                        ? 4
                        : Convert.ToInt32(split[4], CultureInfo.InvariantCulture);
                    MetreChangeEvent metreChangeEvent = new(timestamp, upper, lower);

                    chart.Events.Add(metreChangeEvent);
                }

                // HiSpeed Change Event
                if (objectType is 5)
                {
                    float hiSpeed = Convert.ToSingle(split[3], CultureInfo.InvariantCulture);
                    SpeedChangeEvent speedChangeEvent = new(timestamp, hiSpeed);

                    NotationUtils.AddOrCreate(chart.Layers, "Layer 0", speedChangeEvent);
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
                    NotationUtils.AddOrCreate(chart.Layers, "Layer 0", tempReverseEvent);

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
                    NotationUtils.AddOrCreate(chart.Layers, "Layer 0", tempStopEvent);

                    tempStopEvent = null;
                }
            }
            catch (Exception ex)
            {
                if (ex is IndexOutOfRangeException indexOutOfRangeException)
                {
                    Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorMer002}", indexOutOfRangeException);
                    exceptions.Add(exception);
                    
                    Console.WriteLine(exception);
                    continue;
                }

                if (ex is FormatException formatException)
                {
                    Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorMer003}", formatException);
                    exceptions.Add(exception);
                    
                    Console.WriteLine(exception);
                    continue;
                }

                if (ex is OverflowException overflowException)
                {
                    Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorMer004}", overflowException);
                    exceptions.Add(exception);
                    
                    Console.WriteLine(exception);
                    continue;
                }
                
                // don't throw.
                Console.WriteLine(ex);
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

            HoldNote holdNote = new(current.BonusType, JudgementType.Normal);

            steps = 0; // failsafe if there's a cyclic reference.
            while (current != null && steps < merHoldNotes.Count)
            {
                checkedMerHoldNotes.Add(current);
                steps++;
                
                HoldPointNote holdPointNote = new(new(current.Measure, current.Tick), current.Position, current.Size, holdNote, (HoldPointRenderType)current.Render);
                holdNote.Points.Add(holdPointNote);
                
                current = current.Reference == null
                    ? null
                    : merHoldNotes.FirstOrDefault(x => x.Index == current.Reference);
            }

            holdNote.Points = holdNote.Points
                .OrderBy(x => x.Timestamp)
                .ToList();
            
            NotationUtils.AddOrCreate(chart.Layers, "Layer 0", holdNote);
        }

        NotationUtils.PostProcessChart(chart, args);
        
        return chart;
    }

    /// <summary>
    /// Reads chart data and converts it into an entry.
    /// </summary>
    /// <param name="lines">Chart file data separated into individual lines.</param>
    /// <returns></returns>
    internal static Entry ToEntry(string[] lines, NotationReadArgs args, out List<Exception> exceptions, string path = "")
    {
        Entry entry = new()
        {
            RootDirectory = Path.GetDirectoryName(path) ?? "",
            ChartFile = Path.GetFileName(path),
        };
        exceptions = [];

        bool bodyReached = false;
        foreach (string line in lines)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                if (!bodyReached)
                {
                    string value;
                
                    // BAKKA COMPATIBILITY
                    if (NotationUtils.ContainsKey(line, "#X_BAKKA_MUSIC_FILENAME ", out value)) { entry.AudioFile = value; }
                
                    if (NotationUtils.ContainsKey(line, "#EDITOR_AUDIO ",           out value)) { entry.AudioFile = value; }
                    if (NotationUtils.ContainsKey(line, "#EDITOR_LEVEL ",           out value)) { entry.Level = Convert.ToSingle(value, CultureInfo.InvariantCulture); }
                    if (NotationUtils.ContainsKey(line, "#EDITOR_AUTHOR ",          out value)) { entry.NotesDesigner = value; }
                    if (NotationUtils.ContainsKey(line, "#EDITOR_PREVIEW_TIME ",    out value)) { entry.PreviewBegin = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }
                    if (NotationUtils.ContainsKey(line, "#EDITOR_PREVIEW_LENGTH ",  out value)) { entry.PreviewLength = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }
                    if (NotationUtils.ContainsKey(line, "#EDITOR_OFFSET ",          out value)) { entry.AudioOffset = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }
                    if (NotationUtils.ContainsKey(line, "#EDITOR_MOVIEOFFSET ",     out value)) { entry.VideoOffset = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }
            
                    // WACK COMPATIBILITY
                    if (NotationUtils.ContainsKey(line, "#LEVEL ",          out value)) { entry.Level = Convert.ToSingle(value); }
                    if (NotationUtils.ContainsKey(line, "#AUDIO ",          out value)) { entry.AudioFile = value; }
                    if (NotationUtils.ContainsKey(line, "#AUTHOR ",         out value)) { entry.NotesDesigner = value; }
                    if (NotationUtils.ContainsKey(line, "#PREVIEW_TIME ",   out value)) { entry.PreviewBegin = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }
                    if (NotationUtils.ContainsKey(line, "#PREVIEW_LENGTH ", out value)) { entry.PreviewLength = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }
                
                    // MER COMPATIBILITY
                    if (NotationUtils.ContainsKey(line, "#MUSIC_FILE_PATH ", out value)) { entry.AudioFile = value; }
                    if (NotationUtils.ContainsKey(line, "#OFFSET ",          out value)) { entry.AudioOffset = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }
                    if (NotationUtils.ContainsKey(line, "#MOVIEOFFSET ",     out value)) { entry.VideoOffset = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }
                
                    // PROTOTYPE MER COMPATIBILITY
                    if (NotationUtils.ContainsKey(line, "#MUSIC_NAME @JPN ",               out value)) { entry.Title = value; }
                    if (NotationUtils.ContainsKey(line, "#MUSIC_NAME_RUBY @JPN ",          out value)) { entry.Reading = value; }
                    if (NotationUtils.ContainsKey(line, "#ARTIST_NAME @JPN ",              out value)) { entry.Artist = value; }
                    if (NotationUtils.ContainsKey(line, "#MUSIC_SCORE_CREATOR_NAME @JPN ", out value)) { entry.NotesDesigner = value; }
                    if (NotationUtils.ContainsKey(line, "#JACKET_IMAGE_PATH ",             out value)) { entry.JacketFile = value; }
                    if (NotationUtils.ContainsKey(line, "#DIFFICULTY ",                    out value)) { entry.Level = Convert.ToSingle(value, CultureInfo.InvariantCulture); }
                    if (NotationUtils.ContainsKey(line, "#DISPLAY_BPM ",                   out value)) { entry.BpmMessage = value; }
                    if (NotationUtils.ContainsKey(line, "#CREAR_NORMA_RATE ",              out value)) { entry.ClearThreshold = Convert.ToSingle(value, CultureInfo.InvariantCulture); }
                
                    bodyReached = line.StartsWith("#BODY");
                    continue;
                }
                
                // Search for End of Chart note.
                string[] split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                int measure = Convert.ToInt32(split[0], CultureInfo.InvariantCulture);
                int tick = Convert.ToInt32(split[1], CultureInfo.InvariantCulture);
                int objectType = Convert.ToInt32(split[2], CultureInfo.InvariantCulture);
                if (objectType != 1) continue;
                
                int noteType = Convert.ToInt32(split[3], CultureInfo.InvariantCulture);
                if (noteType != 14) continue;
                
                entry.ChartEnd = new(measure, tick);
            }
            catch (Exception ex)
            {
                if (ex is IndexOutOfRangeException indexOutOfRangeException)
                {
                    Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorMer002}", indexOutOfRangeException);
                    exceptions.Add(exception);
                    
                    Console.WriteLine(exception);
                    continue;
                }

                if (ex is FormatException formatException)
                {
                    Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorMer003}", formatException);
                    exceptions.Add(exception);
                    
                    Console.WriteLine(exception);
                    continue;
                }

                if (ex is OverflowException overflowException)
                {
                    Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorMer004}", overflowException);
                    exceptions.Add(exception);
                    
                    Console.WriteLine(exception);
                    continue;
                }
                
                // don't throw.
                Console.WriteLine(ex);
            }
        }
        
        NotationUtils.PostProcessEntry(entry, args);
        
        return entry;
    }
}
