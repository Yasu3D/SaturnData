using System;
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
    internal static Chart ToChart(string[] lines, NotationReadOptions options)
    {
        Chart chart = new();

        int bookmarkIndex = Array.IndexOf(lines, "@COMMENTS");
        int eventIndex = Array.IndexOf(lines, "@GIMMICKS");
        int objectIndex = Array.IndexOf(lines, "@OBJECTS");

        if (bookmarkIndex == -1 || eventIndex == -1 || objectIndex == -1) return chart;

        string[] bookmarks = lines[(bookmarkIndex + 1)..eventIndex];
        string[] events = lines[(eventIndex + 1)..objectIndex];
        string[] objects = lines[(objectIndex + 1)..];
        
        foreach (string line in bookmarks)
        {
            try
            {
                if (line.StartsWith('#')) continue;

                Match match = Regex.Match(line, BookmarkRegExPattern);
                if (!match.Success) continue;

                // match 3 is the index and can be ignored.
                int measure = Convert.ToInt32(match.Groups[1].Value, CultureInfo.InvariantCulture);
                int tick = Convert.ToInt32(match.Groups[2].Value, CultureInfo.InvariantCulture);
                string message = match.Groups[4].Value;

                Bookmark bookmark = new(new(measure, tick), "#DDDDDD", message);
                chart.Bookmarks.Add(bookmark);
            }
            catch
            {
                // ignored
            }
        }

        ReverseEffectEvent? tempReverseEvent = null;
        int tempReverseLayer = 0;

        StopEffectEvent? tempStopEvent = null;
        int tempStopLayer = 0;
        
        foreach (string line in events)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith('#')) continue;

                string[] split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (split.Length < 4) continue;

                int measure = Convert.ToInt32(split[0], CultureInfo.InvariantCulture);
                int tick = Convert.ToInt32(split[1], CultureInfo.InvariantCulture);
                Timestamp timestamp = new(measure, tick);

                string[] attributes = split[3].Split('.', StringSplitOptions.RemoveEmptyEntries);
                if (attributes.Length == 0) continue;

                string type = attributes[0];

                if (type == "BPM" && split.Length == 5)
                {
                    float bpm = Convert.ToSingle(split[4], CultureInfo.InvariantCulture);
                    BpmChangeEvent bpmChangeEvent = new(timestamp, bpm);

                    chart.GlobalEvents.Add(bpmChangeEvent);
                }

                if (type == "TIMESIG" && split.Length == 6)
                {
                    int upper = Convert.ToInt32(split[4], CultureInfo.InvariantCulture);
                    int lower = Convert.ToInt32(split[5], CultureInfo.InvariantCulture);
                    TimeSignatureChangeEvent timeSignatureChangeEvent = new(timestamp, upper, lower);

                    chart.GlobalEvents.Add(timeSignatureChangeEvent);
                }

                if (type == "HISPEED" && split.Length == 5)
                {
                    float hiSpeed = Convert.ToSingle(split[4], CultureInfo.InvariantCulture);
                    HiSpeedChangeEvent hiSpeedChangeEvent = new(timestamp, hiSpeed);

                    int layer = attributes2Layer(attributes);
                    NotationUtils.AddOrCreate(chart.EventLayers, layer, hiSpeedChangeEvent);
                }

                if (type == "REV_START")
                {
                    tempReverseEvent = new();
                    tempReverseLayer = attributes2Layer(attributes);
                    
                    tempReverseEvent.SubEvents[0] = new(timestamp, tempReverseEvent);
                }

                if (type == "REV_END")
                {
                    if (tempReverseEvent?.SubEvents[0] == null) continue;
                    if (tempReverseEvent.SubEvents[0].Timestamp > timestamp) continue;

                    tempReverseEvent.SubEvents[1] = new(timestamp, tempReverseEvent);
                }

                if (type == "REV_ZONE_END")
                {
                    if (tempReverseEvent?.SubEvents[0] == null) continue;
                    if (tempReverseEvent?.SubEvents[1] == null) continue;
                    if (tempReverseEvent.SubEvents[0].Timestamp > timestamp) continue;
                    if (tempReverseEvent.SubEvents[1].Timestamp > timestamp) continue;

                    tempReverseEvent.SubEvents[2] = new(timestamp, tempReverseEvent);
                    NotationUtils.AddOrCreate(chart.EventLayers, tempReverseLayer, tempReverseEvent);

                    tempReverseEvent = null;
                    tempReverseLayer = 0;
                }

                if (type == "STOP_START")
                {
                    tempStopEvent = new();
                    tempStopEvent.SubEvents[0] = new(timestamp, tempStopEvent);
                    tempStopLayer = attributes2Layer(attributes);
                }

                if (type == "STOP_END")
                {
                    if (tempStopEvent?.SubEvents[0] == null) continue;
                    if (tempStopEvent.SubEvents[0].Timestamp > timestamp) continue;

                    tempStopEvent.SubEvents[1] = new(timestamp, tempStopEvent);
                    NotationUtils.AddOrCreate(chart.EventLayers, tempStopLayer, tempStopEvent);

                    tempStopEvent = null;
                    tempStopLayer = 0;
                }

                if (type == "CHART_END")
                {
                    chart.ChartEnd = timestamp;
                }
            }
            catch
            {
                // ignored
            }
        }

        HoldNote? tempHoldNote = null;
        int tempHoldNoteLayer = 0;

        foreach (string line in objects)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith('#')) continue;
                
                string[] split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (split.Length < 6) continue;
                
                int measure = Convert.ToInt32(split[0], CultureInfo.InvariantCulture);
                int tick = Convert.ToInt32(split[1], CultureInfo.InvariantCulture);
                Timestamp timestamp = new(measure, tick);
                
                int position = Convert.ToInt32(split[3], CultureInfo.InvariantCulture);
                int size = Convert.ToInt32(split[4], CultureInfo.InvariantCulture);
                
                string[] attributes = split[5].Split('.', StringSplitOptions.RemoveEmptyEntries);
                if (attributes.Length == 0) continue;
                
                string type = attributes[0];
                int layer = attributes2Layer(attributes);
                BonusType bonusType = attributes2BonusType(attributes);

                if (type == "TOUCH")
                {
                    TouchNote touchNote = new(timestamp, position, size, bonusType, true);
                    NotationUtils.AddOrCreate(chart.NoteLayers, layer, touchNote);
                }

                if (type == "SNAP_FW")
                {
                    SnapForwardNote snapForwardNote = new(timestamp, position, size, bonusType, true);
                    NotationUtils.AddOrCreate(chart.NoteLayers, layer, snapForwardNote);
                }
                
                if (type == "SNAP_BW")
                {
                    SnapBackwardNote snapBackwardNote = new(timestamp, position, size, bonusType, true);
                    NotationUtils.AddOrCreate(chart.NoteLayers, layer, snapBackwardNote);
                }
                
                if (type == "SLIDE_CW")
                {
                    SlideClockwiseNote slideClockwiseNote = new(timestamp, position, size, bonusType, true);
                    NotationUtils.AddOrCreate(chart.NoteLayers, layer, slideClockwiseNote);
                }
                
                if (type == "SLIDE_CCW")
                {
                    SlideCounterclockwiseNote slideCounterclockwiseNote = new(timestamp, position, size, bonusType, true);
                    NotationUtils.AddOrCreate(chart.NoteLayers, layer, slideCounterclockwiseNote);
                }
                
                if (type == "CHAIN")
                {
                    ChainNote chainNote = new(timestamp, position, size, bonusType, true);
                    NotationUtils.AddOrCreate(chart.NoteLayers, layer, chainNote);
                }
                
                if (type == "HOLD_START")
                {
                    tempHoldNote = new(bonusType, true);
                    tempHoldNoteLayer = layer;

                    HoldPointRenderBehaviour renderBehaviour = attributes2RenderBehaviour(attributes);
                    tempHoldNote.Points.Add(new(timestamp, position, size, renderBehaviour));
                }
                
                if (type == "HOLD_POINT")
                {
                    if (tempHoldNote == null) continue;
                    
                    HoldPointRenderBehaviour renderBehaviour = attributes2RenderBehaviour(attributes);
                    tempHoldNote.Points.Add(new(timestamp, position, size, renderBehaviour));
                }
                
                if (type == "HOLD_END")
                {
                    if (tempHoldNote == null) continue;
                    
                    HoldPointRenderBehaviour renderBehaviour = attributes2RenderBehaviour(attributes);
                    tempHoldNote.Points.Add(new(timestamp, position, size, renderBehaviour));
                    tempHoldNote.Points = tempHoldNote.Points
                        .OrderBy(x => x.Timestamp)
                        .ToList();
                    
                    NotationUtils.AddOrCreate(chart.NoteLayers, tempHoldNoteLayer, tempHoldNote);
                    
                    tempHoldNote = null;
                    tempHoldNoteLayer = 0;
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
            catch
            {
                // ignored
            }
        }

        NotationUtils.PostProcessChart(chart, options);

        return chart;

        int attributes2Layer(string[] attributes)
        {
            foreach (string a in attributes)
            {
                if (a == "L0") return 0;
                if (a == "L1") return 1;
                if (a == "L2") return 2;
                if (a == "L3") return 3;
                if (a == "L4") return 4;
                if (a == "L5") return 5;
                if (a == "L6") return 6;
                if (a == "L7") return 7;
                if (a == "L8") return 8;
                if (a == "L9") return 9;
            }

            return 0;
        }

        BonusType attributes2BonusType(string[] attributes)
        {
            foreach (string a in attributes)
            {
                if (a == "NORMAL") return BonusType.None;
                if (a == "BONUS") return BonusType.Bonus;
                if (a == "RNOTE") return BonusType.R;
            }

            return BonusType.None;
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

        HoldPointRenderBehaviour attributes2RenderBehaviour(string[] attributes)
        {
            foreach (string a in attributes)
            {
                if (a == "NR") return HoldPointRenderBehaviour.Hidden;
            }
            
            return HoldPointRenderBehaviour.Visible;
        }
    }

    /// <summary>
    /// Reads chart data and converts it into an entry.
    /// </summary>
    /// <param name="lines">Chart file data separated into individual lines.</param>
    /// <returns></returns>
    internal static Entry ToEntry(string[] lines, NotationReadOptions options)
    {
        Entry entry = new();

        foreach (string line in lines)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith('#')) continue;
                if (line.StartsWith("@COMMENTS")) break;

                string value;

                if (NotationUtils.ContainsKey(line, "@GUID ", out value)) entry.Guid = value;
                if (NotationUtils.ContainsKey(line, "@VERSION ", out value)) entry.Revision = value;
                if (NotationUtils.ContainsKey(line, "@TITLE ", out value)) entry.Title = value;
                if (NotationUtils.ContainsKey(line, "@RUBI ", out value)) entry.Reading = value;
                if (NotationUtils.ContainsKey(line, "@ARTIST ", out value)) entry.Artist = value;
                if (NotationUtils.ContainsKey(line, "@AUTHOR ", out value)) entry.NotesDesigner = value;
                if (NotationUtils.ContainsKey(line, "@BPM_TEXT ", out value)) entry.BpmMessage = value;

                if (NotationUtils.ContainsKey(line, "@BACKGROUND ", out value)) entry.Background = (BackgroundOption)Convert.ToInt32(value, CultureInfo.InvariantCulture);

                if (NotationUtils.ContainsKey(line, "@DIFF ", out value)) entry.Diff = (Difficulty)Convert.ToInt32(value, CultureInfo.InvariantCulture);
                if (NotationUtils.ContainsKey(line, "@LEVEL ", out value)) entry.Level = Convert.ToSingle(value, CultureInfo.InvariantCulture);

                if (NotationUtils.ContainsKey(line, "@PREVIEW_START ", out value)) entry.PreviewBegin = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000;
                if (NotationUtils.ContainsKey(line, "@PREVIEW_TIME ", out value)) entry.PreviewDuration = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000;

                if (NotationUtils.ContainsKey(line, "@BGM ", out value)) entry.AudioPath = value;
                if (NotationUtils.ContainsKey(line, "@BGM_OFFSET ", out value)) entry.AudioOffset = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000;
                if (NotationUtils.ContainsKey(line, "@BGA ", out value)) entry.VideoPath = value;
                if (NotationUtils.ContainsKey(line, "@BGA_OFFSET ", out value)) entry.VideoOffset = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000;
                if (NotationUtils.ContainsKey(line, "@JACKET ", out value)) entry.JacketPath = value;
            }
            catch
            {
                // ignored, continue
            }
        }

        NotationUtils.PostProcessEntry(entry, options);

        return entry;
    }
}