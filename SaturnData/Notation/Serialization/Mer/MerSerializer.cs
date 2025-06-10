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

namespace SaturnData.Notation.Serialization.Mer;

public static class MerSerializer
{
    /// <summary>
    /// Converts a chart into a string.
    /// </summary>
    /// <param name="entry">The entry to serialize.</param>
    /// <param name="chart">The chart to serialize.</param>
    /// <returns></returns>
    public static string ToString(Entry entry, Chart chart, NotationSerializerOptions options)
    {
        /*try
        {
            StringBuilder sb = new();

            List<Event> events = [];
            List<Note> notes = [];
        
            // Add all global events
            events.AddRange(chart.GlobalEvents);
        
            // Add all layer-specific events from the first layer.
            // Discard all events from other layers.
            Dictionary<EffectSubEvent, string> subEventTypes = new();
            if (chart.EventLayers.Count != 0)
            {
                foreach (KeyValuePair<int, Layer<Event>> layer in chart.EventLayers)
                foreach (Event @event in layer.Value.Items)
                {
                    if (@event is InvisibleEffectEvent) continue; // Mer does not have Invisible Effect Events
                    if (@event is ReverseEffectEvent reverse)
                    {
                        subEventTypes.Add(reverse.SubEvents[0], "   6");
                        subEventTypes.Add(reverse.SubEvents[1], "   7");
                        subEventTypes.Add(reverse.SubEvents[2], "   8");
                        continue;
                    }

                    if (@event is StopEffectEvent stop)
                    {
                        subEventTypes.Add(stop.SubEvents[0], "   9");
                        subEventTypes.Add(stop.SubEvents[1], "  10");
                        continue;
                    }

                    events.Add(@event);
                }
                
                events.AddRange(chart.EventLayers.First().Value.Items
                    .Where(x => x is not InvisibleEffectEvent));
            }

            // Order events by timestamp.
            events = events
                .Where(x => x is ITimeable)
                .OrderBy(x => ((ITimeable)x).Timestamp.FullTick)
                .ToList();

            // Add all notes from all layers.
            // Break out hold points from their parent note and
            // save the note they will "reference" in the .mer file.
            Dictionary<HoldPointNote, HoldPointNote> holdPointReferences = new();
            Dictionary<HoldPointNote, int> holdPointTypes = new();
            foreach (KeyValuePair<int, Layer<Note>> layer in chart.NoteLayers)
            foreach (Note note in layer.Value.Items)
            {
                if (note is HoldNote hold)
                {
                    for (int i = 0; i < hold.Points.Count; i++)
                    {
                        notes.Add(hold.Points[i]);

                        int type;
                        if      (i == 0 && hold.BonusType is BonusType.None or BonusType.Bonus)  type = 9;
                        else if (i == 0 && hold.BonusType is BonusType.R)     type = 25;
                        else if (i == hold.Points.Count - 1) type = 11;
                        else type = 10;
                            
                        holdPointTypes.Add(hold.Points[i], type);
                        
                        if (i == hold.Points.Count - 1) continue;
                        
                        holdPointReferences.Add(hold.Points[i], hold.Points[i + 1]);
                    }
                    
                    continue;
                }

                notes.Add(note);
            }
            
            notes.AddRange(chart.Masks);
        
            // Order notes by timestamp.
            notes = notes
                .Where(x => x is ITimeable)
                .OrderBy(x => ((ITimeable)x).Timestamp.FullTick)
                .ToList();
            
            // Write metadata
            sb.Append("#MUSIC_SCORE_ID 0\n");
            sb.Append("#MUSIC_SCORE_VERSION 0\n");
            sb.Append("#GAME_VERSION\n");
            sb.Append(options.WriteMerMusicFilePath switch
            {
                NotationSerializerOptions.WriteMerMusicFilePathOption.None => "#MUSIC_FILE_PATH\n",
                NotationSerializerOptions.WriteMerMusicFilePathOption.NoExtension => $"#MUSIC_FILE_PATH {Path.GetFileNameWithoutExtension(entry.AudioPath)}\n",
                NotationSerializerOptions.WriteMerMusicFilePathOption.WithExtension => $"#MUSIC_FILE_PATH {Path.GetFileName(entry.AudioPath)}\n",
                _ => throw new ArgumentOutOfRangeException(),
            });
            sb.Append($"#OFFSET {entry.AudioOffset / 1000}\n");
            sb.Append($"#MOVIEOFFSET {entry.VideoOffset / 1000}\n");
            sb.Append("#BODY\n");
            
            // Write Events
            foreach (Event @event in events)
            {
                if (@event is not ITimeable timeable) continue;
                Timestamp timestamp = timeable.Timestamp;

                sb.Append($"{timestamp.Measure,4} {timestamp.FullTick,4} ");
                
                if (@event is BpmChangeEvent bpmChangeEvent)
                {
                    sb.Append($"   2 {bpmChangeEvent.Bpm:F6}");
                }
                
                if (@event is TimeSignatureChangeEvent timeSignatureChangeEvent)
                {
                    sb.Append($"   3 {timeSignatureChangeEvent} {timeSignatureChangeEvent.Lower,4}");
                }
                
                if (@event is HiSpeedChangeEvent highSpeedChangeEvent)
                {
                    sb.Append($"   4 {highSpeedChangeEvent.HiSpeed:F6}");
                }
                
                if (@event is EffectSubEvent subEvent)
                {
                    sb.Append($"{subEventTypes[subEvent]}");
                }
                
                sb.Append("\n");
            }
            
            // Write Notes
            for (int i = 0; i < notes.Count; i++)
            {
                Note note = notes[i];
                if (note is not ITimeable timeable) continue;
                if (note is not IPositionable positionable) continue;

                BonusType bonus = note is IPlayable playable ? playable.BonusType : BonusType.None;

                int type = (note, bonus) switch
                {
                    (TouchNote, BonusType.None) => 1,
                    (TouchNote, BonusType.Bonus) => 2,
                    (TouchNote, BonusType.R) => 20,
                    (SnapForwardNote, BonusType.None) => 3,
                    (SnapForwardNote, BonusType.Bonus) => 3,
                    (SnapForwardNote, BonusType.R) => 21,
                    (SnapBackwardNote, BonusType.None) => 4,
                    (SnapBackwardNote, BonusType.Bonus) => 4,
                    (SnapBackwardNote, BonusType.R) => 22,
                    (SlideClockwiseNote, BonusType.None) => 5,
                    (SlideClockwiseNote, BonusType.Bonus) => 6,
                    (SlideClockwiseNote, BonusType.R) => 23,
                    (SlideCounterclockwiseNote, BonusType.None) => 7,
                    (SlideCounterclockwiseNote, BonusType.Bonus) => 8,
                    (SlideCounterclockwiseNote, BonusType.R) => 24,
                    (ChainNote, BonusType.None) => 16,
                    (ChainNote, BonusType.Bonus) => 16,
                    (ChainNote, BonusType.R) => 26,
                    (HoldPointNote point, _) => holdPointTypes[point],
                    (HoldNote, BonusType.None) => 9,
                    (HoldNote, BonusType.Bonus) => 9,
                    (HoldNote, BonusType.R) => 25,
                    
                    _ => 1,
                };
                
                sb.Append($"{timeable.Timestamp.Measure,4} {timeable.Timestamp.FullTick,4}    1 ");
                sb.Append($"{type,4} {i,4} {positionable.Position,4} {positionable.Size,4} ");

                if (note is HoldPointNote holdPointNote)
                {
                    if (!holdPointReferences.TryGetValue(holdPointNote, out HoldPointNote reference))
                    {
                        //throw new Exception("HoldPointNote references nothing.");
                        continue;
                    }

                    sb.Append($"{notes.IndexOf(reference),4}");
                }

                else if (note is MaskAddNote maskAddNote)
                {
                    sb.Append($"{(int)maskAddNote.Direction,4}");
                }

                else if (note is MaskSubtractNote maskSubtractNote)
                {
                    sb.Append($"{(int)maskSubtractNote.Direction,4}");
                }

                else
                {
                    sb.Append("   1");
                }
                
                sb.Append("\n");
            }

            // Write Chart End
            if (chart.ChartEnd == null)
            {
                
            }
            else
            {
                
            }
        
            return sb.ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // ignored
        }

        return "";*/
        return "";
    }

    /// <summary>
    /// Reads chart data and converts it into a chart.
    /// </summary>
    /// <param name="lines">Chart file data separated into individual lines.</param>
    /// <returns></returns>
    public static Chart ToChart(string[] lines, NotationSerializerOptions options)
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

                        HoldPointNote holdPointNote = new(timestamp, position, size, holdNote, HoldPointRenderBehaviour.Visible);
                        holdNote.Points.Add(holdPointNote);
                        
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
                        HoldPointRenderBehaviour renderBehaviour = (HoldPointRenderBehaviour)Convert.ToInt32(split[7], CultureInfo.InvariantCulture);
                        HoldPointNote holdPointNote = new(timestamp, position, size, holdNote, renderBehaviour);
                        holdNote.Points.Add(holdPointNote);
                        
                        // Only handle render behaviour and references on hold points.
                        if (noteType is 10)
                        {
                            if (renderBehaviour == HoldPointRenderBehaviour.Hidden) noRenderHoldPoints.Add(holdPointNote);
                            
                            // Update the parent hold note reference to the next segment.
                            int reference = Convert.ToInt32(split[8],CultureInfo.InvariantCulture);
                            holdNotesByLastReference.Add(reference, holdNote);
                        }
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
                // ignored, continue to next line.
            }
        }

        // Trim all no-render hold points.
        if (options.TrimHoldNotes)
        {
            foreach (HoldPointNote holdPoint in noRenderHoldPoints)
            {
                holdPoint.Parent.Points.Remove(holdPoint);
            }
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
                if (NotationUtils.ContainsKey(line, "#PREVIEW_TIME ",   out value)) { entry.PreviewBegin = Convert.ToSingle(value) * 1000; }
                if (NotationUtils.ContainsKey(line, "#PREVIEW_LENGTH ", out value)) { entry.PreviewDuration = Convert.ToSingle(value) * 1000; }
                
                // MER COMPATIBILITY
                if (NotationUtils.ContainsKey(line, "#MUSIC_FILE_PATH ", out value)) { entry.AudioPath = value; }
                if (NotationUtils.ContainsKey(line, "#OFFSET ",          out value)) { entry.AudioOffset = Convert.ToSingle(value) * 1000; }
                if (NotationUtils.ContainsKey(line, "#MOVIEOFFSET ",     out value)) { entry.VideoOffset = Convert.ToSingle(value) * 1000; }
            }
            catch
            {
                // ignored, continue
            }
        }
        
        return entry;
    }
}