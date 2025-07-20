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
        NotationUtils.PreProcessEntry(entry, options);
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
        try
        {
            if (options.ExportWatermark != null)
            {
                sb.Append($"# {options.ExportWatermark}\n");
            }

            sb.Append($"{"@SAT_VERSION",-16}3\n");
            sb.Append('\n');
            sb.Append($"{"@GUID",-16}{entry.Guid}\n");
            sb.Append($"{"@TITLE",-16}{entry.Title}\n");
            sb.Append($"{"@READING",-16}{entry.Reading}\n");
            sb.Append($"{"@ARTIST",-16}{entry.Artist}\n");
            sb.Append($"{"@BPM_MESSAGE",-16}{entry.BpmMessage}\n");
            sb.Append('\n');
            sb.Append($"{"@REVISION",-16}{entry.Revision}\n");
            sb.Append($"{"@NOTES_DESIGNER",-16}{entry.NotesDesigner}\n");
            sb.Append($"{"@DIFFICULTY",-16}{difficulty2String(entry.Difficulty)}\n");
            sb.Append($"{"@LEVEL",-16}{entry.Level.ToString("F1", CultureInfo.InvariantCulture)}\n");
            sb.Append($"{"@CLEAR",-16}{entry.ClearThreshold.ToString("F2", CultureInfo.InvariantCulture)}\n");
            sb.Append('\n');
            sb.Append($"{"@PREVIEW_BEGIN",-16}{(entry.PreviewBegin / 1000).ToString("F6", CultureInfo.InvariantCulture)}\n");
            sb.Append($"{"@PREVIEW_LENGTH",-16}{(entry.PreviewLength / 1000).ToString("F6", CultureInfo.InvariantCulture)}\n");
            sb.Append($"{"@BACKGROUND",-16}{background2String(entry.Background)}\n");
            sb.Append('\n');
            sb.Append($"{"@JACKET",-16}{Path.GetFileName(entry.JacketPath)}\n");
            sb.Append($"{"@AUDIO",-16}{Path.GetFileName(entry.AudioPath)}\n");
            sb.Append($"{"@VIDEO",-16}{Path.GetFileName(entry.VideoPath)}\n");
            sb.Append($"{"@AUDIO_OFFSET",-16}{(entry.AudioOffset / 1000).ToString("F6", CultureInfo.InvariantCulture)}\n");
            sb.Append($"{"@VIDEO_OFFSET",-16}{(entry.VideoOffset / 1000).ToString("F6", CultureInfo.InvariantCulture)}\n");
            sb.Append('\n');
            sb.Append($"{"@END",-16}{entry.ChartEnd!.Value.Measure,-4}{entry.ChartEnd!.Value.Tick,-4}\n");
            sb.Append('\n');
            sb.Append($"{"@TUTORIAL",-16}{bool2String(entry.TutorialMode)}\n");
            sb.Append($"{"@AUTO_READING",-16}{bool2String(entry.AutoReading)}\n");
            sb.Append($"{"@AUTO_BPM_MSG",-16}{bool2String(entry.AutoBpmMessage)}\n");
            sb.Append($"{"@AUTO_CLEAR",-16}{bool2String(entry.AutoClearThreshold)}\n");
            sb.Append($"{"@AUTO_END",-16}{bool2String(entry.AutoChartEnd)}\n");
            sb.Append('\n');
        }
        catch
        {
            // ignored
        }

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
                sb.Append($"{"METRE",-9} {timeSignatureChangeEvent.Timestamp.Measure,-4} {timeSignatureChangeEvent.Timestamp.Tick,-4} {timeSignatureChangeEvent.Upper,4}   {timeSignatureChangeEvent.Lower,4}\n");
            }

            if (@event is TutorialTagEvent tutorialTagEvent)
            {
                sb.Append($"{"TUTORIAL",-9} {tutorialTagEvent.Timestamp.Measure,-4} {tutorialTagEvent.Timestamp.Tick,-4} {tutorialTagEvent.Key}");
            }
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
        for (int l = 0; l < chart.Layers.Count; l++)
        {
            Layer layer = chart.Layers[l];
            
            sb.Append($"@LAYER {layer.Name}\n");

            foreach (Event @event in layer.Events)
            {
                if (@event is HiSpeedChangeEvent hiSpeedChangeEvent)
                {
                    sb.Append($"{"SPEED",-9} {hiSpeedChangeEvent.Timestamp.Measure,-4} {hiSpeedChangeEvent.Timestamp.Tick,-4} {hiSpeedChangeEvent.HiSpeed.ToString("F6", CultureInfo.InvariantCulture),11}\n");
                }

                if (@event is InvisibleEffectEvent invisibleEffectEvent)
                {
                    sb.Append($"{"INVIS",-9} {invisibleEffectEvent.Timestamp.Measure,-4} {invisibleEffectEvent.Timestamp.Tick,-4} {bool2String(invisibleEffectEvent.Visible),11}\n");
                }

                if (@event is ReverseEffectEvent reverseEffectEvent)
                {
                    sb.Append($"{"REVERSE",-9} {reverseEffectEvent.SubEvents[0].Timestamp.Measure,-4} {reverseEffectEvent.SubEvents[0].Timestamp.Tick,-4}\n");
                    sb.Append($"{"|",-9} {reverseEffectEvent.SubEvents[1].Timestamp.Measure,-4} {reverseEffectEvent.SubEvents[1].Timestamp.Tick,-4}\n");
                    sb.Append($"{"|",-9} {reverseEffectEvent.SubEvents[2].Timestamp.Measure,-4} {reverseEffectEvent.SubEvents[2].Timestamp.Tick,-4}\n");
                }

                if (@event is StopEffectEvent stopEffectEvent)
                {
                    sb.Append($"{"STOP",-9} {stopEffectEvent.SubEvents[0].Timestamp.Measure,-4} {stopEffectEvent.SubEvents[0].Timestamp.Tick,-4}\n");
                    sb.Append($"{"|",-9} {stopEffectEvent.SubEvents[1].Timestamp.Measure,-4} {stopEffectEvent.SubEvents[1].Timestamp.Tick,-4}\n");
                }
            }

            sb.Append('\n');

            for (int n = 0; n < layer.Notes.Count; n++)
            {
                Note note = layer.Notes[n];
                
                if (note is TouchNote touchNote)
                {
                    sb.Append($"{"TOUCH" + attributes2String(touchNote),-9} {touchNote.Timestamp.Measure,-4} {touchNote.Timestamp.Tick,-4} {touchNote.Position,-4} {touchNote.Size,-4}\n");
                }

                if (note is SnapForwardNote snapForwardNote)
                {
                    sb.Append($"{"SNFWD" + attributes2String(snapForwardNote),-9} {snapForwardNote.Timestamp.Measure,-4} {snapForwardNote.Timestamp.Tick,-4} {snapForwardNote.Position,-4} {snapForwardNote.Size,-4}\n");
                }

                if (note is SnapBackwardNote snapBackwardNote)
                {
                    sb.Append($"{"SNBWD" + attributes2String(snapBackwardNote),-9} {snapBackwardNote.Timestamp.Measure,-4} {snapBackwardNote.Timestamp.Tick,-4} {snapBackwardNote.Position,-4} {snapBackwardNote.Size,-4}\n");
                }

                if (note is SlideClockwiseNote slideClockwiseNote)
                {
                    sb.Append($"{"SLCLW" + attributes2String(slideClockwiseNote),-9} {slideClockwiseNote.Timestamp.Measure,-4} {slideClockwiseNote.Timestamp.Tick,-4} {slideClockwiseNote.Position,-4} {slideClockwiseNote.Size,-4}\n");
                }

                if (note is SlideCounterclockwiseNote slideCounterclockwiseNote)
                {
                    sb.Append($"{"SLCCW" + attributes2String(slideCounterclockwiseNote),-9} {slideCounterclockwiseNote.Timestamp.Measure,-4} {slideCounterclockwiseNote.Timestamp.Tick,-4} {slideCounterclockwiseNote.Position,-4} {slideCounterclockwiseNote.Size,-4}\n");
                }

                if (note is ChainNote chainNote)
                {
                    sb.Append($"{"CHAIN" + attributes2String(chainNote),-9} {chainNote.Timestamp.Measure,-4} {chainNote.Timestamp.Tick,-4} {chainNote.Position,-4} {chainNote.Size,-4}\n");
                }

                if (note is HoldNote holdNote)
                {
                    sb.Append($"{"HOLD" + attributes2String(holdNote),-9} {holdNote.Points[0].Timestamp.Measure,-4} {holdNote.Points[0].Timestamp.Tick,-4} {holdNote.Points[0].Position,-4} {holdNote.Points[0].Size,-4}\n");

                    for (int p = 1; p < holdNote.Points.Count; p++)
                    {
                        HoldPointNote point = holdNote.Points[p];
                        sb.Append($"{(point.RenderType == HoldPointRenderType.Visible ? "|" : "~"),-9} {point.Timestamp.Measure,-4} {point.Timestamp.Tick,-4} {point.Position,-4} {point.Size,-4}\n");
                    }
                }

                if (note is MeasureLineNote measureLineNote)
                {
                    sb.Append($"{"MLINE",-9} {measureLineNote.Timestamp.Measure,-4} {measureLineNote.Timestamp.Tick,-4}\n");
                }

                // Remove line break on the very last note.
                if (l == chart.Layers.Count - 1 && n == layer.Notes.Count - 1 && sb[^1] == '\n')
                {
                    sb.Remove(sb.Length - 1, 1);
                }
            }

            // Only add line break if it's not the last layer.
            if (l != chart.Layers.Count - 1)
            {
                sb.Append('\n');
            }
        }

        return;

        string bool2String(bool b)
        {
            return b ? "TRUE" : "FALSE";
        }

        string attributes2String(IPlayable playable)
        {
            string result = playable.BonusType switch
            {
                BonusType.None => "",
                BonusType.Bonus => ".B",
                BonusType.R => ".R",
                _ => throw new ArgumentOutOfRangeException(),
            };

            result += playable.JudgementType switch
            {
                JudgementType.Normal => "",
                JudgementType.Autoplay => ".A",
                JudgementType.Fake => ".F",
                _ => throw new ArgumentOutOfRangeException(),
            };
            return result;
        }
    }
}