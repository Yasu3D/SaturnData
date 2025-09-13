using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SaturnData.Notation.Core;
using SaturnData.Notation.Events;
using SaturnData.Notation.Interfaces;
using SaturnData.Notation.Notes;

namespace SaturnData.Notation.Serialization.SatV2;

internal static class SatV2Reader
{ 
    /// <summary>
    /// RegEx pattern to parse bookmarks.
    /// </summary>
    private const string BookmarkRegExPattern = @"^\s*(\d+)\s+(\d+)\s+(\d+)\s+(.*)";
    
    /// <summary>
    /// Reads chart data and converts it into a chart.
    /// </summary>
    /// <param name="lines">Chart file data separated into individual lines.</param>
    /// <returns></returns>
    internal static Chart ToChart(string[] lines, NotationReadArgs args, out List<Exception> exceptions)
    {
        Chart chart = new();
        exceptions = [];

        int bookmarkIndex = Array.IndexOf(lines, "@COMMENTS");
        int eventIndex = Array.IndexOf(lines, "@GIMMICKS");
        int objectIndex = Array.IndexOf(lines, "@OBJECTS");

        if (bookmarkIndex == -1 || eventIndex == -1 || objectIndex == -1)
        {
            Exception exception = new(ErrorList.ErrorSat201);
            exceptions.Add(exception);
            
            Console.WriteLine(exception);
            return chart;
        }

        string[] bookmarks = lines[(bookmarkIndex + 1)..eventIndex];
        string[] events = lines[(eventIndex + 1)..objectIndex];
        string[] objects = lines[(objectIndex + 1)..];
        
        foreach (string line in bookmarks)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith('#')) continue;

                Match match = Regex.Match(line, BookmarkRegExPattern);
                if (!match.Success)
                {
                    Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat006}");
                    exceptions.Add(exception);
                    
                    Console.WriteLine(exception);
                    continue;
                }

                // match 3 is the index and can be ignored.
                int measure = Convert.ToInt32(match.Groups[1].Value, CultureInfo.InvariantCulture);
                int tick = Convert.ToInt32(match.Groups[2].Value, CultureInfo.InvariantCulture);
                string message = match.Groups[4].Value;

                Bookmark bookmark = new(new(measure, tick), "DDDDDD", message);
                chart.Bookmarks.Add(bookmark);
            }
            catch (Exception ex)
            {
                if (ex is FormatException formatException)
                {
                    Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat008}", formatException);
                    exceptions.Add(exception);
                    
                    Console.WriteLine(exception);
                    continue;
                }

                if (ex is OverflowException overflowException)
                {
                    Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat009}", overflowException);
                    exceptions.Add(exception);
                    
                    Console.WriteLine(exception);
                    continue;
                }
                
                // don't throw.
                Console.WriteLine(ex);
            }
        }

        ReverseEffectEvent? tempReverseEvent = null;
        string tempReverseLayer = "Layer 0";

        StopEffectEvent? tempStopEvent = null;
        string tempStopLayer = "Layer 0";
        
        foreach (string line in events)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith('#')) continue;

                string[] split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (split.Length < 4)
                {
                    Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat001}");
                    exceptions.Add(exception);
                    
                    Console.WriteLine(exception);
                    continue;
                }

                int measure = Convert.ToInt32(split[0], CultureInfo.InvariantCulture);
                int tick = Convert.ToInt32(split[1], CultureInfo.InvariantCulture);
                Timestamp timestamp = new(measure, tick);

                string[] attributes = split[3].Split('.', StringSplitOptions.RemoveEmptyEntries);
                if (attributes.Length == 0) 
                {
                    Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat002}");
                    exceptions.Add(exception);
                
                    Console.WriteLine(exception);
                    continue;
                }
                
                string type = attributes[0];

                if (type == "BPM" && split.Length == 5)
                {
                    float bpm = Convert.ToSingle(split[4], CultureInfo.InvariantCulture);
                    TempoChangeEvent tempoChangeEvent = new(timestamp, bpm);

                    chart.Events.Add(tempoChangeEvent);
                    continue;
                }

                if (type == "TIMESIG" && split.Length == 6)
                {
                    int upper = Convert.ToInt32(split[4], CultureInfo.InvariantCulture);
                    int lower = Convert.ToInt32(split[5], CultureInfo.InvariantCulture);
                    MetreChangeEvent metreChangeEvent = new(timestamp, upper, lower);

                    chart.Events.Add(metreChangeEvent);
                    continue;
                }

                if (type == "HISPEED" && split.Length == 5)
                {
                    float hiSpeed = Convert.ToSingle(split[4], CultureInfo.InvariantCulture);
                    SpeedChangeEvent speedChangeEvent = new(timestamp, hiSpeed);

                    string layer = attributes2Layer(attributes);
                    NotationUtils.AddOrCreate(chart.Layers, layer, speedChangeEvent);
                    continue;
                }

                if (type == "REV_START")
                {
                    if (tempReverseEvent != null)
                    {
                        Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.WarningSat020}");
                        exceptions.Add(exception);
                        
                        Console.WriteLine(exception);
                    }
                    
                    tempReverseEvent = new();
                    tempReverseLayer = attributes2Layer(attributes);
                    
                    tempReverseEvent.SubEvents[0] = new(timestamp, tempReverseEvent);
                    continue;
                }

                if (type == "REV_END")
                {
                    if (tempReverseEvent?.SubEvents[0] == null)
                    {
                        Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat013}");
                        exceptions.Add(exception);
                        
                        Console.WriteLine(exception);
                        continue;
                    }
                    
                    if (tempReverseEvent.SubEvents[0].Timestamp >= timestamp)
                    {
                        Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat014}");
                        exceptions.Add(exception);
                        
                        Console.WriteLine(exception);
                        continue;
                    }

                    tempReverseEvent.SubEvents[1] = new(timestamp, tempReverseEvent);
                    continue;
                }

                if (type == "REV_ZONE_END")
                {
                    if (tempReverseEvent?.SubEvents[0] == null)
                    {
                        Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat013}");
                        exceptions.Add(exception);
                        
                        Console.WriteLine(exception);
                        continue;
                    }
                    
                    if (tempReverseEvent?.SubEvents[1] == null)
                    {
                        Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat015}");
                        exceptions.Add(exception);
                        
                        Console.WriteLine(exception);
                        continue;
                    }
                    
                    if (tempReverseEvent.SubEvents[0].Timestamp > timestamp)
                    {
                        Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat014}");
                        exceptions.Add(exception);
                        
                        Console.WriteLine(exception);
                        continue;
                    }
                    
                    if (tempReverseEvent.SubEvents[1].Timestamp > timestamp)
                    {
                        Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat016}");
                        exceptions.Add(exception);
                        
                        Console.WriteLine(exception);
                        continue;
                    }

                    tempReverseEvent.SubEvents[2] = new(timestamp, tempReverseEvent);
                    NotationUtils.AddOrCreate(chart.Layers, tempReverseLayer, tempReverseEvent);

                    tempReverseEvent = null;
                    tempReverseLayer = "Layer 0";
                    continue;
                }

                if (type == "STOP_START")
                {
                    if (tempStopEvent != null)
                    {
                        Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.WarningSat021}");
                        exceptions.Add(exception);
                        
                        Console.WriteLine(exception);
                    }
                    
                    tempStopEvent = new();
                    tempStopEvent.SubEvents[0] = new(timestamp, tempStopEvent);
                    tempStopLayer = attributes2Layer(attributes);
                    continue;
                }

                if (type == "STOP_END")
                {
                    if (tempStopEvent?.SubEvents[0] == null)
                    {
                        Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat017}");
                        exceptions.Add(exception);
                        
                        Console.WriteLine(exception);
                        continue;
                    }
                    
                    if (tempStopEvent.SubEvents[0].Timestamp > timestamp)
                    {
                        Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat018}");
                        exceptions.Add(exception);
                        
                        Console.WriteLine(exception);
                        continue;
                    }

                    tempStopEvent.SubEvents[1] = new(timestamp, tempStopEvent);
                    NotationUtils.AddOrCreate(chart.Layers, tempStopLayer, tempStopEvent);

                    tempStopEvent = null;
                    tempStopLayer = "Layer 0";
                    continue;
                }
                
                if (type == "CHART_END") continue; // Handled in entry read.
                
                // Type was not recognized
                Exception exception2 = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat010(type)}");
                exceptions.Add(exception2);
                    
                Console.WriteLine(exception2);
            }
            catch (Exception ex)
            {
                if (ex is IndexOutOfRangeException indexOutOfRangeException)
                {
                    Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat007}", indexOutOfRangeException);
                    exceptions.Add(exception);
                    
                    Console.WriteLine(exception);
                    continue;
                }
                
                if (ex is FormatException formatException)
                {
                    Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat008}", formatException);
                    exceptions.Add(exception);
                    
                    Console.WriteLine(exception);
                    continue;
                }

                if (ex is OverflowException overflowException)
                {
                    Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat009}", overflowException);
                    exceptions.Add(exception);
                    
                    Console.WriteLine(exception);
                    continue;
                }
                
                // don't throw.
                Console.WriteLine(ex);
            }
        }

        HoldNote? tempHoldNote = null;
        string tempHoldNoteLayer = "Layer 0";

        foreach (string line in objects)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith('#')) continue;
                
                string[] split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (split.Length < 6)
                {
                    Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat003}");
                    exceptions.Add(exception);
                    
                    Console.WriteLine(exception);
                    continue;
                }
                
                int measure = Convert.ToInt32(split[0], CultureInfo.InvariantCulture);
                int tick = Convert.ToInt32(split[1], CultureInfo.InvariantCulture);
                Timestamp timestamp = new(measure, tick);
                
                int position = Convert.ToInt32(split[3], CultureInfo.InvariantCulture);
                int size = Convert.ToInt32(split[4], CultureInfo.InvariantCulture);
                
                string[] attributes = split[5].Split('.', StringSplitOptions.RemoveEmptyEntries);
                if (attributes.Length == 0)
                {
                    Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat004}");
                    exceptions.Add(exception);
                
                    Console.WriteLine(exception);
                    continue;
                }
                
                string type = attributes[0];
                string layer = attributes2Layer(attributes);
                BonusType bonusType = attributes2BonusType(attributes);

                if (type == "TOUCH")
                {
                    TouchNote touchNote = new(timestamp, position, size, bonusType, JudgementType.Normal);
                    NotationUtils.AddOrCreate(chart.Layers, layer, touchNote);
                }

                if (type == "SNAP_FW")
                {
                    SnapForwardNote snapForwardNote = new(timestamp, position, size, bonusType, JudgementType.Normal);
                    NotationUtils.AddOrCreate(chart.Layers, layer, snapForwardNote);
                }
                
                if (type == "SNAP_BW")
                {
                    SnapBackwardNote snapBackwardNote = new(timestamp, position, size, bonusType, JudgementType.Normal);
                    NotationUtils.AddOrCreate(chart.Layers, layer, snapBackwardNote);
                }
                
                if (type == "SLIDE_CW")
                {
                    SlideClockwiseNote slideClockwiseNote = new(timestamp, position, size, bonusType, JudgementType.Normal);
                    NotationUtils.AddOrCreate(chart.Layers, layer, slideClockwiseNote);
                }
                
                if (type == "SLIDE_CCW")
                {
                    SlideCounterclockwiseNote slideCounterclockwiseNote = new(timestamp, position, size, bonusType, JudgementType.Normal);
                    NotationUtils.AddOrCreate(chart.Layers, layer, slideCounterclockwiseNote);
                }
                
                if (type == "CHAIN")
                {
                    ChainNote chainNote = new(timestamp, position, size, bonusType, JudgementType.Normal);
                    NotationUtils.AddOrCreate(chart.Layers, layer, chainNote);
                }
                
                if (type == "HOLD_START")
                {
                    if (tempHoldNote != null)
                    {
                        Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.WarningSat022}");
                        exceptions.Add(exception);
                        
                        Console.WriteLine(exception);
                    }
                    
                    tempHoldNote = new(bonusType, JudgementType.Normal);
                    tempHoldNoteLayer = layer;

                    HoldPointRenderType renderType = attributes2RenderBehaviour(attributes);
                    tempHoldNote.Points.Add(new(timestamp, position, size, tempHoldNote, renderType));
                }
                
                if (type == "HOLD_POINT")
                {
                    if (tempHoldNote == null)
                    {
                        Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat019}");
                        exceptions.Add(exception);
                        
                        Console.WriteLine(exception);
                        continue;
                    }
                    
                    HoldPointRenderType renderType = attributes2RenderBehaviour(attributes);
                    tempHoldNote.Points.Add(new(timestamp, position, size, tempHoldNote, renderType));
                }
                
                if (type == "HOLD_END")
                {
                    if (tempHoldNote == null)
                    {
                        Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat019}");
                        exceptions.Add(exception);
                        
                        Console.WriteLine(exception);
                        continue;
                    }
                    
                    HoldPointRenderType renderType = attributes2RenderBehaviour(attributes);
                    tempHoldNote.Points.Add(new(timestamp, position, size, tempHoldNote, renderType));
                    tempHoldNote.Points = tempHoldNote.Points
                        .OrderBy(x => x.Timestamp)
                        .ToList();
                    
                    NotationUtils.AddOrCreate(chart.Layers, tempHoldNoteLayer, tempHoldNote);
                    
                    tempHoldNote = null;
                    tempHoldNoteLayer = "Layer 0";
                }
                
                if (type == "MASK_ADD")
                {
                    LaneSweepDirection direction = attributes2LaneToggleDirection(attributes);
                    LaneShowNote laneShowNote = new(timestamp, position, size, direction);
                    chart.LaneToggles.Add(laneShowNote);
                }
                
                if (type == "MASK_SUB")
                {
                    LaneSweepDirection direction = attributes2LaneToggleDirection(attributes);
                    LaneHideNote laneHideNote = new(timestamp, position, size, direction);
                    chart.LaneToggles.Add(laneHideNote);
                }
            }
            catch (Exception ex)
            {
                if (ex is IndexOutOfRangeException indexOutOfRangeException)
                {
                    Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat007}", indexOutOfRangeException);
                    exceptions.Add(exception);
                    
                    Console.WriteLine(exception);
                    continue;
                }
                
                if (ex is FormatException formatException)
                {
                    Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat008}", formatException);
                    exceptions.Add(exception);
                    
                    Console.WriteLine(exception);
                    continue;
                }

                if (ex is OverflowException overflowException)
                {
                    Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat009}", overflowException);
                    exceptions.Add(exception);
                    
                    Console.WriteLine(exception);
                    continue;
                }
                
                // don't throw.
                Console.WriteLine(ex);
            }
        }

        NotationUtils.PostProcessChart(chart, args);

        return chart;

        string attributes2Layer(string[] attributes)
        {
            foreach (string a in attributes)
            {
                if (a == "L0") return "Layer 0";
                if (a == "L1") return "Layer 1";
                if (a == "L2") return "Layer 2";
                if (a == "L3") return "Layer 3";
                if (a == "L4") return "Layer 4";
                if (a == "L5") return "Layer 5";
                if (a == "L6") return "Layer 6";
                if (a == "L7") return "Layer 7";
                if (a == "L8") return "Layer 8";
                if (a == "L9") return "Layer 9";
            }

            return "Layer 0";
        }

        BonusType attributes2BonusType(string[] attributes)
        {
            foreach (string a in attributes)
            {
                if (a == "NORMAL") return BonusType.Normal;
                if (a == "BONUS") return BonusType.Bonus;
                if (a == "RNOTE") return BonusType.R;
            }

            return BonusType.Normal;
        }

        LaneSweepDirection attributes2LaneToggleDirection(string[] attributes)
        {
            foreach (string a in attributes)
            {
                if (a == "CW") return LaneSweepDirection.Clockwise;
                if (a == "CCW") return LaneSweepDirection.Counterclockwise;
                if (a == "CENTER") return LaneSweepDirection.Center;
            }

            return LaneSweepDirection.Clockwise;
        }

        HoldPointRenderType attributes2RenderBehaviour(string[] attributes)
        {
            foreach (string a in attributes)
            {
                if (a == "NR") return HoldPointRenderType.Hidden;
            }
            
            return HoldPointRenderType.Visible;
        }
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

        foreach (string line in lines)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith('#')) continue;
                if (line.StartsWith("@OBJECTS")) break;

                string value;

                if (NotationUtils.ContainsKey(line, "@GUID ",     out value)) { entry.Guid = value; }
                if (NotationUtils.ContainsKey(line, "@VERSION ",  out value)) { entry.Revision = value; }
                if (NotationUtils.ContainsKey(line, "@TITLE ",    out value)) { entry.Title = value; }
                if (NotationUtils.ContainsKey(line, "@RUBI ",     out value)) { entry.Reading = value; }
                if (NotationUtils.ContainsKey(line, "@ARTIST ",   out value)) { entry.Artist = value; }
                if (NotationUtils.ContainsKey(line, "@AUTHOR ",   out value)) { entry.NotesDesigner = value; }
                if (NotationUtils.ContainsKey(line, "@BPM_TEXT ", out value)) { entry.BpmMessage = value; }

                if (NotationUtils.ContainsKey(line, "@BACKGROUND ", out value)) { entry.Background = (BackgroundOption)Convert.ToInt32(value, CultureInfo.InvariantCulture); }

                if (NotationUtils.ContainsKey(line, "@DIFF ",  out value)) { entry.Difficulty = (Difficulty)Convert.ToInt32(value, CultureInfo.InvariantCulture); }
                if (NotationUtils.ContainsKey(line, "@LEVEL ", out value)) { entry.Level = Convert.ToSingle(value, CultureInfo.InvariantCulture); }
                if (NotationUtils.ContainsKey(line, "@CLEAR ", out value)) { entry.ClearThreshold = Convert.ToSingle(value, CultureInfo.InvariantCulture); }

                if (NotationUtils.ContainsKey(line, "@PREVIEW_START ", out value)) { entry.PreviewBegin = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }
                if (NotationUtils.ContainsKey(line, "@PREVIEW_TIME ",  out value)) { entry.PreviewLength = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }

                if (NotationUtils.ContainsKey(line, "@JACKET ",     out value)) { entry.JacketFile = value; }
                if (NotationUtils.ContainsKey(line, "@BGM ",        out value)) { entry.AudioFile  = value; }
                if (NotationUtils.ContainsKey(line, "@BGA ",        out value)) { entry.VideoFile  = value; }
                if (NotationUtils.ContainsKey(line, "@BGM_OFFSET ", out value)) { entry.AudioOffset = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }
                if (NotationUtils.ContainsKey(line, "@BGA_OFFSET ", out value)) { entry.VideoOffset = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }

                if (line.EndsWith("CHART_END"))
                {
                    string[] split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    int measure = Convert.ToInt32(split[0], CultureInfo.InvariantCulture);
                    int tick = Convert.ToInt32(split[1], CultureInfo.InvariantCulture);

                    entry.ChartEnd = new(measure, tick);
                }
            }
            catch (Exception ex)
            {
                if (ex is IndexOutOfRangeException indexOutOfRangeException)
                {
                    Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat007}", indexOutOfRangeException);
                    exceptions.Add(exception);
                    
                    Console.WriteLine(exception);
                    continue;
                }

                if (ex is FormatException formatException)
                {
                    Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat008}", formatException);
                    exceptions.Add(exception);
                    
                    Console.WriteLine(exception);
                    continue;
                }

                if (ex is OverflowException overflowException)
                {
                    Exception exception = new($"{Array.IndexOf(lines, line) + 1} : {ErrorList.ErrorSat009}", overflowException);
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