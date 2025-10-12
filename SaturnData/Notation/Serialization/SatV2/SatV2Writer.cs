using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using SaturnData.Notation.Core;
using SaturnData.Notation.Events;
using SaturnData.Notation.Interfaces;
using SaturnData.Notation.Notes;

namespace SaturnData.Notation.Serialization.SatV2;

public static class SatV2Writer
{
    /// <summary>
    /// Converts a chart into a string.
    /// </summary>
    /// <param name="entry">The entry to serialize.</param>
    /// <param name="chart">The chart to serialize.</param>
    /// <returns></returns>
    public static string ToString(Entry entry, Chart chart, NotationWriteArgs args)
    {
        StringBuilder sb = new();

        WriteMetadata(sb, entry, args);
        WriteBookmarks(sb, chart, args);
        WriteEvents(sb, chart, entry, args);
        WriteNotes(sb, chart, args);

        return sb.ToString();
    }

    public static void WriteMetadata(StringBuilder sb, Entry entry, NotationWriteArgs args)
    {
        if (args.ExportWatermark != null)
        {
            sb.Append($"# {args.ExportWatermark}\n");
        }
        
        sb.Append($"{"@SAT_VERSION",-16}2\n");
        sb.Append($"\n" + $"{"@VERSION",-16}{entry.Revision}\n");
        sb.Append($"{"@GUID", -16}SAT{Guid.NewGuid()}\n");
        sb.Append($"{"@TITLE",-16}{entry.Title}\n");
        sb.Append($"{"@RUBI",-16}{entry.Reading}\n");
        sb.Append($"{"@ARTIST",-16}{entry.Artist}\n");
        sb.Append($"{"@AUTHOR",-16}{entry.NotesDesigner}\n");
        sb.Append($"{"@BPM_TEXT",-16}{entry.BpmMessage}\n");
        sb.Append('\n');
        sb.Append($"{"@BACKGROUND",-16}{(int)entry.Background}\n");
        sb.Append('\n');
        sb.Append($"{"@DIFF",-16}{(int)entry.Difficulty}\n");
        sb.Append($"{"@LEVEL",-16}{entry.Level.ToString("F6", CultureInfo.InvariantCulture)}\n");
        sb.Append($"{"@CLEAR",-16}{entry.ClearThreshold.ToString("F6", CultureInfo.InvariantCulture)}\n");
        sb.Append('\n');
        sb.Append($"{"@PREVIEW_START",-16}{(entry.PreviewBegin / 1000).ToString("F6", CultureInfo.InvariantCulture)}\n");
        sb.Append($"{"@PREVIEW_TIME",-16}{(entry.PreviewLength / 1000).ToString("F6", CultureInfo.InvariantCulture)}\n");
        sb.Append('\n');
        sb.Append($"{"@JACKET",-16}{entry.JacketFile}\n");
        sb.Append($"{"@BGM",-16}{entry.AudioFile}\n");
        sb.Append($"{"@BGA",-16}{entry.VideoFile}\n");
        sb.Append($"{"@BGM_OFFSET",-16}{(entry.AudioOffset / 1000).ToString("F6", CultureInfo.InvariantCulture)}\n");
        sb.Append($"{"@BGA_OFFSET",-16}{(entry.VideoOffset / 1000).ToString("F6", CultureInfo.InvariantCulture)}\n");
        sb.Append('\n');
    }
    
    public static void WriteBookmarks(StringBuilder sb, Chart chart, NotationWriteArgs args)
    {
        sb.Append("@COMMENTS\n");
        
        int index = 0;
        foreach (Bookmark bookmark in chart.Bookmarks)
        {
            sb.Append($"{bookmark.Timestamp.Measure,-4} {bookmark.Timestamp.Tick,-4} {index,-4} {bookmark.Message}\n");
            index++;
        }
        
        sb.Append('\n');
    }
    
    public static void WriteEvents(StringBuilder sb, Chart chart, Entry entry, NotationWriteArgs args)
    {
        List<Event> events = [];
        Dictionary<Event, int> layerIndices = new();
        
        events.AddRange(chart.Events);

        int layerIndex = 0;
        foreach (Layer layer in chart.Layers)
        {
            foreach (Event @event in layer.Events)
            {
                events.Add(@event);
                layerIndices.Add(@event, layerIndex);
            }
            
            layerIndex++;
            if (layerIndex > 9) break;
        }

        events = events
            .OrderBy(x => ((ITimeable)x).Timestamp)
            .ToList();
        
        sb.Append("@GIMMICKS\n");
        
        int index = 0;
        foreach (Event @event in events)
        {
            if (@event is TempoChangeEvent bpmChangeEvent)
            {
                sb.Append($"{bpmChangeEvent.Timestamp.Measure,-4} {bpmChangeEvent.Timestamp.Tick,-4} {index,-4} {"BPM",-16} {bpmChangeEvent.Tempo.ToString("F6", CultureInfo.InvariantCulture),11}\n");
            }

            if (@event is MetreChangeEvent timeSignatureChangeEvent)
            {
                sb.Append($"{timeSignatureChangeEvent.Timestamp.Measure,-4} {timeSignatureChangeEvent.Timestamp.Tick,-4} {index,-4} {"TIMESIG",-16} {timeSignatureChangeEvent.Upper,4}   {timeSignatureChangeEvent.Lower,4}\n");
            }

            if (@event is SpeedChangeEvent hiSpeedChangeEvent)
            {
                int? layer = layerIndices.TryGetValue(hiSpeedChangeEvent, out int value) ? value : null;
                string layerAttribute = LayerAttribute2String(layer, args);
                sb.Append($"{hiSpeedChangeEvent.Timestamp.Measure,-4} {hiSpeedChangeEvent.Timestamp.Tick,-4} {index,-4} {"HISPEED" + layerAttribute,-16} {hiSpeedChangeEvent.Speed.ToString("F6", CultureInfo.InvariantCulture),11}\n");
            }

            if (@event is ReverseEffectEvent reverseEffectEvent)
            {
                int? layer = layerIndices.TryGetValue(reverseEffectEvent, out int value) ? value : null;
                string layerAttribute = LayerAttribute2String(layer, args);
                sb.Append($"{reverseEffectEvent.SubEvents[0].Timestamp.Measure,-4} {reverseEffectEvent.SubEvents[0].Timestamp.Tick,-4} {index,-4} {"REV_START" + layerAttribute,-16}\n");
                sb.Append($"{reverseEffectEvent.SubEvents[1].Timestamp.Measure,-4} {reverseEffectEvent.SubEvents[1].Timestamp.Tick,-4} {index,-4} {"REV_END",-16}\n");
                sb.Append($"{reverseEffectEvent.SubEvents[2].Timestamp.Measure,-4} {reverseEffectEvent.SubEvents[2].Timestamp.Tick,-4} {index,-4} {"REV_ZONE_END",-16}\n");
            }

            if (@event is StopEffectEvent stopEffectEvent)
            {
                int? layer = layerIndices.TryGetValue(stopEffectEvent, out int value) ? value : null;
                string layerAttribute = LayerAttribute2String(layer, args);
                sb.Append($"{stopEffectEvent.SubEvents[0].Timestamp.Measure,-4} {stopEffectEvent.SubEvents[0].Timestamp.Tick,-4} {index,-4} {"STOP_START" + layerAttribute,-16}\n");
                sb.Append($"{stopEffectEvent.SubEvents[1].Timestamp.Measure,-4} {stopEffectEvent.SubEvents[1].Timestamp.Tick,-4} {index,-4} {"STOP_END",-16}\n");
            }
            
            index++;
        }

        sb.Append($"{entry.ChartEnd.Measure,-4} {entry.ChartEnd.Tick,-4} {index,-4} {"CHART_END",-16}\n");
        sb.Append('\n');
    }
    
    public static void WriteNotes(StringBuilder sb, Chart chart, NotationWriteArgs args)
    {
        List<Note> notes = [];
        Dictionary<Note, int> layerIndices = new();
        
        int layerIndex = 0;
        foreach (Layer layer in chart.Layers)
        {
            foreach (Note note in layer.Notes)
            {
                notes.Add(note);
                layerIndices.Add(note, layerIndex);
            }
            
            layerIndex++;
            if (layerIndex > 9) break;
        }
        
        notes.AddRange(chart.LaneToggles);

        notes = notes
            .OrderBy(x => ((ITimeable)x).Timestamp)
            .ToList();
        
        sb.Append("@OBJECTS\n");
        
        int index = 0;
        foreach (Note note in notes)
        {
            if (note is HoldNote holdNote)
            {
                for (int i = 0; i < holdNote.Points.Count; i++)
                {
                    HoldPointNote point = holdNote.Points[i];

                    string type = i == 0 ? "HOLD_START" : "HOLD_POINT";
                    type = i == holdNote.Points.Count - 1 ? "HOLD_END" : type;

                    string attributes = i != 0
                        ? ""
                        : holdNote.BonusType switch
                        {
                            BonusType.Normal => "",
                            BonusType.Bonus => ".BONUS",
                            BonusType.R => ".RNOTE",
                            _ => "",
                        };
                    int? layer = layerIndices.TryGetValue(holdNote, out int value) ? value : null;
                    attributes += LayerAttribute2String(layer, args);
                    attributes += point.RenderType == HoldPointRenderType.Hidden ? ".NR" : "";

                    sb.Append($"{point.Timestamp.Measure,-4} {point.Timestamp.Tick,-4} {index,-4} {point.Position,-4} {point.Size,-4} {type}{attributes}\n");
                    
                    index++;
                }
            }
            else
            {
                Timestamp timestamp = ((ITimeable)note).Timestamp;
                int position = ((IPositionable)note).Position;
                int size = ((IPositionable)note).Size;
                string type = note switch
                {
                    TouchNote => "TOUCH",
                    SnapForwardNote => "SNAP_FW",
                    SnapBackwardNote => "SNAP_BW",
                    SlideClockwiseNote => "SLIDE_CW",
                    SlideCounterclockwiseNote => "SLIDE_CCW",
                    ChainNote => "CHAIN",
                    LaneShowNote => "MASK_ADD",
                    LaneHideNote => "MASK_SUB",
                    _ => "UNKNOWN",
                };

                string attributes = "";

                if (note is IPlayable playable)
                {
                    attributes += playable.BonusType switch
                    {
                        BonusType.Normal => "",
                        BonusType.Bonus => ".BONUS",
                        BonusType.R => ".RNOTE",
                        _ => "",
                    };
                }

                if (note is ILaneToggle laneToggle)
                {
                    attributes += laneToggle.Direction switch
                    {
                        LaneSweepDirection.Counterclockwise => ".CCW",
                        LaneSweepDirection.Clockwise => ".CW",
                        LaneSweepDirection.Center => ".CENTER",
                        LaneSweepDirection.Instant => ".CENTER",
                        _ => "",
                    };
                }
                
                int? layer = layerIndices.TryGetValue(note, out int value) ? value : null;
                attributes += LayerAttribute2String(layer, args);
                sb.Append($"{timestamp.Measure,-4} {timestamp.Tick,-4} {index,-4} {position,-4} {size,-4} {type}{attributes}\n");
                
                index++;
            }
        }
    }
    
    private static string LayerAttribute2String(int? layer, NotationWriteArgs args)
    {
        if (layer is null) return "";
        return $".L{layer}";
    }
}