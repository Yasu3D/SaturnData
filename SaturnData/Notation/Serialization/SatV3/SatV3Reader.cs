using System;
using System.Globalization;
using System.Text.RegularExpressions;
using SaturnData.Notation.Core;
using SaturnData.Notation.Events;
using SaturnData.Notation.Interfaces;
using SaturnData.Notation.Notes;

namespace SaturnData.Notation.Serialization.SatV3;

public static class SatV3Reader
{
    private enum Region
    {
        None = 0,
        Bookmarks = 1,
        Events = 2,
        Lane = 3,
        Layer = 4,
    }
    
    private const string LayerRegexPattern = "^@LAYER (.+)";
    private const string BookmarkRegexPattern = @"^([0-9A-F]{6})\s+([0-9]+)\s+([0-9]+)\s+(.+)";
    
    /// <summary>
    /// Reads chart data and converts it into a chart.
    /// </summary>
    /// <param name="lines">Chart file data separated into individual lines.</param>
    /// <returns></returns>
    public static Chart ToChart(string[] lines, NotationReadArgs args)
    {
        Chart chart = new();

        Region currentRegion = Region.None;
        Layer? currentLayer = null;
        ITimeable? currentMultilineObject = null;
        
        foreach (string line in lines)
        {
            try
            {
                // Skip comments.
                if (line.StartsWith('#')) continue;
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                string[] split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                
                // Detect Bookmark Region
                if (split[0] == "@BOOKMARKS")
                {
                    currentRegion = Region.Bookmarks;
                    currentLayer = null;
                    setCurrentMultiLineObject(null);
                    continue;
                }
                
                // Detect Events Region
                if (split[0] == "@EVENTS")
                {
                    currentRegion = Region.Events;
                    currentLayer = null;
                    setCurrentMultiLineObject(null);
                    continue;
                }
                
                // Detect Lane Toggle Region
                if (split[0] == "@LANE")
                {
                    currentRegion = Region.Lane;
                    currentLayer = null;
                    setCurrentMultiLineObject(null);
                    continue;
                }
                
                // Detect Layer Region
                if (split[0] == "@LAYER")
                {
                    Match layerMatch = Regex.Match(line, LayerRegexPattern);
                    if (!layerMatch.Success) continue;
                    
                    Layer layer = new(layerMatch.Groups[1].Value);
                    
                    currentRegion = Region.Layer;
                    currentLayer = layer;
                    setCurrentMultiLineObject(null);

                    chart.Layers.Add(layer);
                    continue;
                }
                
                
                // Parse Bookmark Region
                if (currentRegion is Region.Bookmarks)
                {
                    Match bookmarkMatch = Regex.Match(line, BookmarkRegexPattern);
                    if (!bookmarkMatch.Success) continue;
                    
                    string color = bookmarkMatch.Groups[1].Value;
                    int measure = Convert.ToInt32(bookmarkMatch.Groups[2].Value, CultureInfo.InvariantCulture);
                    int tick = Convert.ToInt32(bookmarkMatch.Groups[3].Value, CultureInfo.InvariantCulture);
                    Timestamp timestamp = new(measure, tick);
                    
                    string message = bookmarkMatch.Groups[4].Value;

                    Bookmark bookmark = new(timestamp, color, message);
                    chart.Bookmarks.Add(bookmark);
                    continue;
                }
                
                // Parse Events Region
                if (currentRegion is Region.Events)
                {
                    int measure = Convert.ToInt32(split[1], CultureInfo.InvariantCulture);
                    int tick = Convert.ToInt32(split[2], CultureInfo.InvariantCulture);
                    Timestamp timestamp = new(measure, tick);
                    
                    if (split[0] == "TEMPO")
                    {
                        float bpm = Convert.ToSingle(split[3], CultureInfo.InvariantCulture);
                        TempoChangeEvent tempoChangeEvent = new(timestamp, bpm);
                        chart.Events.Add(tempoChangeEvent);
                        continue;
                    }

                    if (split[0] == "METRE")
                    { 
                        int upper = Convert.ToInt32(split[3], CultureInfo.InvariantCulture);
                        int lower = Convert.ToInt32(split[4], CultureInfo.InvariantCulture);
                        MetreChangeEvent metreChangeEvent = new(timestamp, upper, lower);
                        chart.Events.Add(metreChangeEvent);
                        continue;
                    }

                    if (split[0] == "TUTORIAL")
                    {
                        string key = split[3];
                        TutorialMarkerEvent tutorialMarkerEvent = new(timestamp, key);
                        chart.Events.Add(tutorialMarkerEvent);
                        continue;
                    }
                }

                // Parse Lane Toggle Region
                if (currentRegion is Region.Lane)
                {
                    LaneSweepDirection direction = string2LaneSweepDirection(split[1]);
                    
                    int measure = Convert.ToInt32(split[2], CultureInfo.InvariantCulture);
                    int tick = Convert.ToInt32(split[3], CultureInfo.InvariantCulture);
                    Timestamp timestamp = new(measure, tick);
                    
                    int position = Convert.ToInt32(split[4], CultureInfo.InvariantCulture);
                    int size = Convert.ToInt32(split[5], CultureInfo.InvariantCulture);
                    
                    if (split[0] == "SHOW")
                    {
                        LaneShowNote laneShowNote = new(timestamp, position, size, direction);
                        chart.LaneToggles.Add(laneShowNote);
                    }

                    if (split[0] == "HIDE")
                    {
                        LaneHideNote laneHideNote = new(timestamp, position, size, direction);
                        chart.LaneToggles.Add(laneHideNote);
                    }

                    continue;
                }
                
                // Parse Layer Region
                if (currentRegion is Region.Layer)
                {
                    if (currentLayer == null)
                    {
                        throw new("No active layer found while attempting to add items to layers.");
                    }

                    bool isEvent = split[0] is "SPEED" or "VISIBLE" or "REVERSE" or "STOP" 
                                   || (split[0] == "|" && currentMultilineObject is ReverseEffectEvent or StopEffectEvent);
                    
                    Timestamp timestamp = isEvent
                        ? new(Convert.ToInt32(split[1], CultureInfo.InvariantCulture), Convert.ToInt32(split[2], CultureInfo.InvariantCulture))
                        : new(Convert.ToInt32(split[3], CultureInfo.InvariantCulture), Convert.ToInt32(split[4], CultureInfo.InvariantCulture));
                    
                    if (split[0] == "SPEED")
                    {
                        float speed = Convert.ToSingle(split[3], CultureInfo.InvariantCulture);
                        SpeedChangeEvent speedChangeEvent = new(timestamp, speed);
                        setCurrentMultiLineObject(null);
                        
                        currentLayer.Events.Add(speedChangeEvent);
                        
                        continue;
                    }

                    if (split[0] == "VISIBLE")
                    {
                        bool visible = split[3] == "TRUE";
                        VisibilityChangeEvent visibilityChangeEvent = new(timestamp, visible);
                        setCurrentMultiLineObject(null);
                        
                        currentLayer.Events.Add(visibilityChangeEvent);
                        
                        continue;
                    }

                    if (split[0] == "REVERSE")
                    {
                        ReverseEffectEvent reverseEffectEvent = new();
                        reverseEffectEvent.SubEvents[0] = new(timestamp, reverseEffectEvent);
                        setCurrentMultiLineObject(reverseEffectEvent);
                        
                        continue;
                    }

                    if (split[0] == "STOP")
                    {
                        StopEffectEvent stopEffectEvent = new();
                        stopEffectEvent.SubEvents[0] = new(timestamp, stopEffectEvent);
                        setCurrentMultiLineObject(stopEffectEvent);
                        
                        continue;
                    }

                    int position;
                    int size;
                    
                    if (split[0] == "|")
                    {
                        if (currentMultilineObject is HoldNote holdNote)
                        {
                            HoldPointRenderType type = string2HoldPointRenderType(split[1]);
                            
                            position = Convert.ToInt32(split[5], CultureInfo.InvariantCulture);
                            size = Convert.ToInt32(split[6], CultureInfo.InvariantCulture);
                    
                            holdNote.Points.Add(new(timestamp, position, size, holdNote, type));
                        }

                        if (currentMultilineObject is ReverseEffectEvent reverseEffectEvent)
                        {
                            if (reverseEffectEvent?.SubEvents[0] == null) continue;
                            if (reverseEffectEvent.SubEvents[1] == null!)
                            {
                                reverseEffectEvent.SubEvents[1] = new(timestamp, reverseEffectEvent);
                                continue;
                            }

                            if (reverseEffectEvent.SubEvents[2] == null!)
                            {
                                reverseEffectEvent.SubEvents[2] = new(timestamp, reverseEffectEvent);
                                setCurrentMultiLineObject(null);
                                continue;
                            }
                        }

                        if (currentMultilineObject is StopEffectEvent stopEffectEvent)
                        {
                            if (stopEffectEvent?.SubEvents[0] == null) continue;

                            if (stopEffectEvent.SubEvents[1] == null!)
                            {
                                stopEffectEvent.SubEvents[1] = new(timestamp, stopEffectEvent);
                                setCurrentMultiLineObject(null);
                                continue;
                            }
                        }
                    }
                    
                    BonusType bonusType = string2BonusType(split[1]);
                    JudgementType judgementType = string2JudgementType(split[2]);
                    position = Convert.ToInt32(split[5], CultureInfo.InvariantCulture);
                    size = Convert.ToInt32(split[6], CultureInfo.InvariantCulture);
                    
                    if (split[0] == "TOUCH")
                    {
                        TouchNote touchNote = new(timestamp, position, size, bonusType, judgementType);
                        setCurrentMultiLineObject(null);
                        
                        currentLayer.Notes.Add(touchNote);
                    }
                    
                    if (split[0] == "SNFWD")
                    {
                        SnapForwardNote snapForwardNote = new(timestamp, position, size, bonusType, judgementType);
                        setCurrentMultiLineObject(null);
                        
                        currentLayer.Notes.Add(snapForwardNote);
                    }
                    
                    if (split[0] == "SNBWD")
                    {
                        SnapBackwardNote snapBackwardNote = new(timestamp, position, size, bonusType, judgementType);
                        setCurrentMultiLineObject(null);
                        
                        currentLayer.Notes.Add(snapBackwardNote);
                    }
                    
                    if (split[0] == "SLCLW")
                    {
                        SlideClockwiseNote slideClockwiseNote = new(timestamp, position, size, bonusType, judgementType);
                        setCurrentMultiLineObject(null);
                        
                        currentLayer.Notes.Add(slideClockwiseNote);
                    }
                    
                    if (split[0] == "SLCCW")
                    {
                        SlideCounterclockwiseNote slideCounterclockwiseNote = new(timestamp, position, size, bonusType, judgementType);
                        setCurrentMultiLineObject(null);
                        
                        currentLayer.Notes.Add(slideCounterclockwiseNote);
                    }
                    
                    if (split[0] == "CHAIN")
                    {
                        ChainNote chainNote = new(timestamp, position, size, bonusType, judgementType);
                        setCurrentMultiLineObject(null);
                        
                        currentLayer.Notes.Add(chainNote);
                    }
                    
                    if (split[0] == "HOLD")
                    {
                        HoldNote holdNote = new(bonusType, judgementType);
                        setCurrentMultiLineObject(holdNote);
                        
                        holdNote.Points.Add(new(timestamp, position, size, holdNote, HoldPointRenderType.Visible));
                    }

                    if (split[0] == "SYNC")
                    {
                        SyncNote syncNote = new(timestamp, position, size);
                        setCurrentMultiLineObject(null);

                        currentLayer.Notes.Add(syncNote);
                    }
                    
                    if (split[0] == "MLINE")
                    {
                        MeasureLineNote measureLineNote = new(timestamp);
                        setCurrentMultiLineObject(null);
                        
                        currentLayer.Notes.Add(measureLineNote);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine($"Error occurred here: {line}");
                // don't throw.
            }
        }
        
        return chart;

        LaneSweepDirection string2LaneSweepDirection(string input)
        {
            return input switch
            {
                "<" => LaneSweepDirection.Counterclockwise,
                ">" => LaneSweepDirection.Clockwise,
                "X" => LaneSweepDirection.Center,
                "!" => LaneSweepDirection.Instant,
                _ => LaneSweepDirection.Instant,
            };
        }

        BonusType string2BonusType(string input)
        {
            return input switch
            {
                "N" => BonusType.Normal,
                "B" => BonusType.Bonus,
                "R" => BonusType.R,
                _ => BonusType.Normal,
            };
        }

        JudgementType string2JudgementType(string input)
        {
            return input switch
            {
                "N" => JudgementType.Normal,
                "F" => JudgementType.Fake,
                "A" => JudgementType.Autoplay,
                _ => JudgementType.Normal,
            };
        }

        HoldPointRenderType string2HoldPointRenderType(string input)
        {
            return input switch
            {
                "V" => HoldPointRenderType.Visible,
                "H" => HoldPointRenderType.Hidden,
                _ => HoldPointRenderType.Visible,
            };
        }
        
        void setCurrentMultiLineObject(ITimeable? newMultilineObject)
                {
                    if (currentLayer == null)
                    {
                        currentMultilineObject = newMultilineObject;
                        return;
                    }
                    
                    if (currentMultilineObject is StopEffectEvent stopEffectEvent 
                        && stopEffectEvent.SubEvents[0] != null! 
                        && stopEffectEvent.SubEvents[1] != null!)
                    {
                        currentLayer.Events.Add(stopEffectEvent);
                    }

                    if (currentMultilineObject is ReverseEffectEvent reverseEffectEvent
                        && reverseEffectEvent.SubEvents[0] != null!
                        && reverseEffectEvent.SubEvents[1] != null!
                        && reverseEffectEvent.SubEvents[2] != null!)
                    {
                        currentLayer.Events.Add(reverseEffectEvent);
                    }

                    if (currentMultilineObject is HoldNote holdNote
                        && holdNote.Points.Count > 1)
                    {
                        currentLayer.Notes.Add(holdNote);
                    }
                    
                    currentMultilineObject = newMultilineObject;
                }
    }

    /// <summary>
    /// Reads chart data and converts it into an entry.
    /// </summary>
    /// <param name="lines">Chart file data separated into individual lines.</param>
    /// <returns></returns>
    internal static Entry ToEntry(string[] lines, NotationReadArgs args)
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
                
                if (NotationUtils.ContainsKey(line, "@GUID ",           out value)) { entry.Guid = value; }
                if (NotationUtils.ContainsKey(line, "@TITLE ",          out value)) { entry.Title = value; }
                if (NotationUtils.ContainsKey(line, "@READING ",        out value)) { entry.Reading = value; }
                if (NotationUtils.ContainsKey(line, "@ARTIST ",         out value)) { entry.Artist = value; }
                if (NotationUtils.ContainsKey(line, "@BPM_MESSAGE ",    out value)) { entry.BpmMessage = value; }
                
                if (NotationUtils.ContainsKey(line, "@REVISION ",       out value)) { entry.Revision = value; }
                if (NotationUtils.ContainsKey(line, "@NOTES_DESIGNER ", out value)) { entry.NotesDesigner = value; }
                if (NotationUtils.ContainsKey(line, "@DIFFICULTY ",     out value)) { entry.Difficulty = string2Difficulty(value); }
                if (NotationUtils.ContainsKey(line, "@LEVEL ",          out value)) { entry.Level = Convert.ToSingle(value, CultureInfo.InvariantCulture); }
                if (NotationUtils.ContainsKey(line, "@CLEAR",           out value)) { entry.ClearThreshold = Convert.ToSingle(value, CultureInfo.InvariantCulture); }

                if (NotationUtils.ContainsKey(line, "@PREVIEW_BEGIN ",  out value)) { entry.PreviewBegin = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }
                if (NotationUtils.ContainsKey(line, "@PREVIEW_LENGTH ", out value)) { entry.PreviewLength = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }
                if (NotationUtils.ContainsKey(line, "@BACKGROUND ",     out value)) { entry.Background = string2BackgroundOption(value); }

                if (NotationUtils.ContainsKey(line, "@JACKET ",         out value)) { entry.JacketPath = value; }
                if (NotationUtils.ContainsKey(line, "@AUDIO ",          out value)) { entry.AudioPath = value; }
                if (NotationUtils.ContainsKey(line, "@VIDEO ",          out value)) { entry.VideoPath = value; }
                if (NotationUtils.ContainsKey(line, "@AUDIO_OFFSET ",   out value)) { entry.AudioOffset = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }
                if (NotationUtils.ContainsKey(line, "@VIDEO_OFFSET ",   out value)) { entry.VideoOffset = Convert.ToSingle(value, CultureInfo.InvariantCulture) * 1000; }

                if (NotationUtils.ContainsKey(line, "@TUTORIAL ",       out value)) { entry.TutorialMode       = value == "TRUE"; }
                if (NotationUtils.ContainsKey(line, "@AUTO_READING ",   out value)) { entry.AutoReading        = value == "TRUE"; }
                if (NotationUtils.ContainsKey(line, "@AUTO_BPM_MSG ",   out value)) { entry.AutoBpmMessage     = value == "TRUE"; }
                if (NotationUtils.ContainsKey(line, "@AUTO_CLEAR ",     out value)) { entry.AutoClearThreshold = value == "TRUE"; }
                if (NotationUtils.ContainsKey(line, "@AUTO_END ",       out value)) { entry.AutoChartEnd       = value == "TRUE"; }

                if (NotationUtils.ContainsKey(line, "@END ",            out value))
                {
                    string[] split = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    int measure = Convert.ToInt32(split[0], CultureInfo.InvariantCulture);
                    int tick = Convert.ToInt32(split[1], CultureInfo.InvariantCulture);

                    entry.ChartEnd = new(measure, tick);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine($"Error occurred here: {line}");
                // don't throw.
            }
        }

        NotationUtils.PostProcessEntry(entry, args);

        return entry;

        BackgroundOption string2BackgroundOption(string input)
        {
            return input switch
            {
                "AUTO" => BackgroundOption.Auto,
                "SATURN" => BackgroundOption.Saturn,
                "VERSION3" => BackgroundOption.Version3,
                "VERSION2" => BackgroundOption.Version2,
                "VERSION1" => BackgroundOption.Version1,
                "BOSS" => BackgroundOption.Boss,
                "STAGEUP" => BackgroundOption.StageUp,
                "WORLDSEND" => BackgroundOption.WorldsEnd,
                "JACKET" => BackgroundOption.Jacket,
                _ => BackgroundOption.Auto,
            };
        }

        Difficulty string2Difficulty(string input)
        {
            return input switch
            {
                "NORMAL" => Difficulty.Normal,
                "HARD" => Difficulty.Hard,
                "EXPERT" => Difficulty.Expert,
                "INFERNO" => Difficulty.Inferno,
                "WORLDSEND" => Difficulty.WorldsEnd,
                _ => Difficulty.Normal,
            };
        }
    }
}