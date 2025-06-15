using System;
using System.Globalization;
using System.IO;
using System.Text;
using SaturnData.Notation.Core;
using SaturnData.Notation.Events;
using SaturnData.Notation.Interfaces;
using SaturnData.Notation.Notes;

namespace SaturnData.Notation.Serialization.SatV3;

public static class SatV3Writer
{
    /// <summary>
    /// Converts a chart into a string.
    /// </summary>
    /// <param name="entry">The entry to serialize.</param>
    /// <param name="chart">The chart to serialize.</param>
    /// <returns></returns>
    public static string ToString(Entry entry, Chart chart, NotationWriteOptions options)
    {
        StringBuilder sb = new();
        NotationUtils.PreProcessChart(chart, options);

        WriteMetadata(sb, entry, options);
        WriteBookmarks(sb, chart, options);
        WriteEvents(sb, chart, options);
        WriteLanes(sb, chart, options);
        WriteLayers(sb, chart, options);

        return sb.ToString();
    }

    public static void WriteMetadata(StringBuilder sb, Entry entry, NotationWriteOptions options)
    {
        if (options.ExportWatermark != null)
        {
            sb.Append($"# {options.ExportWatermark}\n");
        }
        
        sb.Append($"{"@SAT_VERSION",-16}2\n");
        sb.Append($"{"@REVISION",-16}{entry.Revision}\n");
        sb.Append($"{"@GUID",-16}{entry.Guid}\n");
        sb.Append($"{"@TITLE",-16}{entry.Title}\n");
        sb.Append($"{"@READING",-16}{entry.Reading}\n");
        sb.Append($"{"@ARTIST",-16}{entry.Artist}\n");
        sb.Append($"{"@NOTES_DESIGNER",-16}{entry.NotesDesigner}\n");
        sb.Append($"{"@BPM_MESSAGE",-16}{entry.BpmMessage}\n");
        sb.Append('\n');
        sb.Append($"{"@BACKGROUND",-16}{background2String(entry.Background)}\n");
        sb.Append('\n');
        sb.Append($"{"@DIFFICULTY",-16}{difficulty2String(entry.Difficulty)}\n");
        sb.Append($"{"@LEVEL",-16}{entry.Level.ToString("F1", CultureInfo.InvariantCulture)}\n");
        sb.Append($"{"@CLEAR",-16}{entry.ClearThreshold.ToString("F2", CultureInfo.InvariantCulture)}\n");
        sb.Append('\n');
        sb.Append($"{"@PREVIEW_START",-16}{(entry.PreviewBegin / 1000).ToString("F6", CultureInfo.InvariantCulture)}\n");
        sb.Append($"{"@PREVIEW_TIME",-16}{(entry.PreviewDuration / 1000).ToString("F6", CultureInfo.InvariantCulture)}\n");
        sb.Append('\n');
        sb.Append($"{"@JACKET",-16}{Path.GetFileName(entry.JacketPath)}\n");
        sb.Append($"{"@AUDIO",-16}{Path.GetFileName(entry.AudioPath)}\n");
        sb.Append($"{"@VIDEO",-16}{Path.GetFileName(entry.VideoPath)}\n");
        sb.Append($"{"@AUDIO_OFFSET",-16}{(entry.AudioOffset / 1000).ToString("F6", CultureInfo.InvariantCulture)}\n");
        sb.Append($"{"@VIDEO_OFFSET",-16}{(entry.VideoOffset / 1000).ToString("F6", CultureInfo.InvariantCulture)}\n");
        sb.Append('\n');
        sb.Append($"{"@TUTORIAL",-16}{bool2String(entry.IsTutorial)}\n");
        sb.Append('\n');
        return;

        string background2String(BackgroundOption background)
        {
            return background switch
            {
                BackgroundOption.Auto      => "AUTO",
                BackgroundOption.Saturn    => "SATURN",
                BackgroundOption.Version3  => "VERSION3",
                BackgroundOption.Version2  => "VERSION2",
                BackgroundOption.Version1  => "VERSION1",
                BackgroundOption.Boss      => "BOSS",
                BackgroundOption.StageUp   => "STAGEUP",
                BackgroundOption.WorldsEnd => "WORLDSEND",
                BackgroundOption.Jacket    => "JACKET",
                _ => throw new ArgumentOutOfRangeException(nameof(background), background, null),
            };
        }

        string difficulty2String(Difficulty difficulty)
        {
            return difficulty switch
            {
                Difficulty.None      => "NONE",
                Difficulty.Normal    => "NORMAL",
                Difficulty.Hard      => "HARD",
                Difficulty.Expert    => "EXPERT",
                Difficulty.Inferno   => "INFERNO",
                Difficulty.WorldsEnd => "WORLDSEND",
                _ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null),
            };
        }

        string bool2String(bool b)
        {
            return b ? "TRUE" : "FALSE";
        }
    }
    
    public static void WriteBookmarks(StringBuilder sb, Chart chart, NotationWriteOptions options)
    {
        sb.Append("@BOOKMARKS\n");
        
        foreach (Bookmark bookmark in chart.Bookmarks)
        {
            sb.Append($"{bookmark.Color,-9} {bookmark.Timestamp.Measure,-4} {bookmark.Timestamp.Tick,-4} {bookmark.Message}\n");
        }

        sb.Append('\n');
    }
    
    public static void WriteEvents(StringBuilder sb, Chart chart, NotationWriteOptions options)
    {
        sb.Append("@EVENTS\n");

        foreach (Event @event in chart.Events)
        {
            if (@event is BpmChangeEvent bpmChangeEvent)
            {
                sb.Append($"{"TEMPO",-9} {bpmChangeEvent.Timestamp.Measure,-4} {bpmChangeEvent.Timestamp.Tick,-4} {bpmChangeEvent.Bpm.ToString("F6", CultureInfo.InvariantCulture),11}\n");
            }

            if (@event is TimeSignatureChangeEvent timeSignatureChangeEvent)
            {
                sb.Append($"{"TEMPO",-9} {timeSignatureChangeEvent.Timestamp.Measure,-4} {timeSignatureChangeEvent.Timestamp.Tick,-4} {timeSignatureChangeEvent.Upper,4}   {timeSignatureChangeEvent.Lower,4}\n");
            }

            if (@event is TutorialTagEvent tutorialTagEvent)
            {
                sb.Append($"{"TUTORIAL",-9} {tutorialTagEvent.Timestamp.Measure,-4} {tutorialTagEvent.Timestamp.Tick,-4} {tutorialTagEvent.Key}");
            }
        }
        
        if (chart.ChartEnd != null)
        {
            sb.Append($"{"END",-9}{chart.ChartEnd.Value.Measure,-4} {chart.ChartEnd.Value.Tick,-4}\n");
        }
        
        sb.Append('\n');
    }
    
    public static void WriteLanes(StringBuilder sb, Chart chart, NotationWriteOptions options)
    {
        sb.Append("@LANE\n");

        foreach (Note note in chart.LaneToggles)
        {
            if (note is LaneShowNote laneShowNote)
            {
                sb.Append($"{"SHOW" + direction2String(laneShowNote.Direction),-9} {laneShowNote.Timestamp.Measure,-4} {laneShowNote.Timestamp.Tick,-4} {laneShowNote.Position,-4} {laneShowNote.Size,-4}\n");
            }

            if (note is LaneHideNote laneHideNote)
            {
                sb.Append($"{"HIDE" + direction2String(laneHideNote.Direction),-9} {laneHideNote.Timestamp.Measure,-4} {laneHideNote.Timestamp.Tick,-4} {laneHideNote.Position,-4} {laneHideNote.Size,-4}\n");
            }
        }
        
        sb.Append('\n');
        return;
        
        string direction2String(LaneSweepDirection direction)
        {
            return direction switch
            {
                LaneSweepDirection.Counterclockwise => ".CCW",
                LaneSweepDirection.Clockwise => ".CLW",
                LaneSweepDirection.Center => ".CTR",
                LaneSweepDirection.Instant => "",
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null),
            };
        }
    }
    
    public static void WriteLayers(StringBuilder sb, Chart chart, NotationWriteOptions options)
    {
        
    }
}