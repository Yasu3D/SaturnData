using System;
using System.Collections.Generic;
using System.Linq;
using SaturnData.Notation.Events;
using SaturnData.Notation.Interfaces;
using SaturnData.Notation.Notes;
using SaturnData.Utilities;

namespace SaturnData.Notation.Core;

/// <summary>
/// Holds all data associated with gameplay.
/// </summary>
public class Chart
{
    /// <summary>
    /// All global events that aren't bound to layers.
    /// </summary>
    public List<Event> Events { get; set; } = [];
    
    /// <summary>
    /// All local events and notes, grouped by layer.
    /// </summary>
    public List<Layer> Layers { get; set; } = [];
    
    /// <summary>
    /// All lane toggle notes.
    /// </summary>
    public List<Note> LaneToggles { get; set; } = [];
    
    /// <summary>
    /// Editor-only bookmarks.
    /// </summary>
    public List<Bookmark> Bookmarks { get; set; } = [];
    
    /// <summary>
    /// Finds the last tempo change before a given timestamp.
    /// </summary>
    public TempoChangeEvent? LastTempoChange(Timestamp timestamp)
    {
        if (timestamp.FullTick == 0)
        {
            return Events.LastOrDefault(x => x is TempoChangeEvent && x.Timestamp.FullTick == 0) as TempoChangeEvent;
        }
        
        return Events.OrderBy(x => x.Timestamp).LastOrDefault(x => x is TempoChangeEvent && x.Timestamp < timestamp) as TempoChangeEvent;
    }
    
    /// <summary>
    /// Finds the last tempo change before a given time.
    /// </summary>
    public TempoChangeEvent? LastTempoChange(float time)
    {
        if (time == 0)
        {
            return Events.LastOrDefault(x => x is TempoChangeEvent && x.Timestamp.Time == 0) as TempoChangeEvent;
        }
        
        return Events.OrderBy(x => x.Timestamp).LastOrDefault(x => x is TempoChangeEvent && x.Timestamp.Time < time) as TempoChangeEvent;
    }

    /// <summary>
    /// Finds the last metre change before a given timestamp.
    /// </summary>
    public MetreChangeEvent? LastMetreChange(Timestamp timestamp)
    {
        if (timestamp.FullTick == 0)
        {
            return Events.LastOrDefault(x => x is MetreChangeEvent && x.Timestamp.FullTick == 0) as MetreChangeEvent;
        }
        
        return Events.OrderBy(x => x.Timestamp).LastOrDefault(x => x is MetreChangeEvent && x.Timestamp < timestamp) as MetreChangeEvent;
    }

    /// <summary>
    /// Finds the last metre change before a given time.
    /// </summary>
    public MetreChangeEvent? LastMetreChange(float time)
    {
        if (time == 0)
        {
            return Events.LastOrDefault(x => x is MetreChangeEvent && x.Timestamp.Time == 0) as MetreChangeEvent;
        }
        
        return Events.OrderBy(x => x.Timestamp).LastOrDefault(x => x is MetreChangeEvent && x.Timestamp.Time < time) as MetreChangeEvent;
    }

    /// <summary>
    /// Returns the "parent" <see cref="Layer"/> that contains the specified <see cref="ITimeable"/>.<br/>
    /// If the ITimeable is not in any layer, <c>null</c> is returned instead.
    /// </summary>
    /// <param name="obj">The ITimeable to find the parent of.</param>
    public Layer? ParentLayer(ITimeable obj)
    {
        if (Layers.Count == 0) return null;
        if (obj is TempoChangeEvent) return null;
        if (obj is MetreChangeEvent) return null;
        if (obj is TutorialMarkerEvent) return null;
        if (obj is Bookmark) return null;
        if (obj is ILaneToggle) return null;

        if (obj is Note note)
        {
            return Layers.FirstOrDefault(x => x.Notes.Contains(note));
        }

        if (obj is Event @event)
        {
            return Layers.FirstOrDefault(x => x.Events.Contains(@event));
        }
        
        return null;
    }
    
    /// <summary>
    /// Calculates the ideal chart end timestamp, based on all objects in a chart and the length of audio.
    /// </summary>
    /// <param name="audioLength">The length of the audio file in milliseconds.(optional)</param>
    /// <returns></returns>
    public Timestamp FindIdealChartEnd(float audioLength = 0)
    {
        Timestamp audioEnd = Timestamp.TimestampFromTime(this, audioLength);
        Timestamp chartEnd = Timestamp.Zero;

        foreach (Layer layer in Layers)
        foreach (Note note in layer.Notes)
        {
            if (note is not IPlayable playable) continue;
            if (playable.JudgementType is JudgementType.Fake) continue; // Allow for fake notes behind end
            
            chartEnd = Timestamp.Max(chartEnd, note.Timestamp);

            if (note is HoldNote holdNote && holdNote.Points.Count > 1)
            {
                chartEnd = Timestamp.Max(chartEnd, holdNote.Points[^1].Timestamp);
            }
        }
        
        chartEnd = Timestamp.TimestampFromTime(this, chartEnd.Time + 2000);
        
        // Round up to next measure.
        if (chartEnd.Tick != 0)
        {
            chartEnd.Measure += 1;
            chartEnd.Tick = 0;
            chartEnd.Time = Timestamp.TimeFromTimestamp(this, chartEnd);
        }
        
        if (audioEnd.Tick != 0)
        {
            audioEnd.Measure += 1;
            audioEnd.Tick = 0;
            audioEnd.Time = Timestamp.TimeFromTimestamp(this, audioEnd);
        }

        return Timestamp.Max(chartEnd, audioEnd);
    }

    /// <summary>
    /// Returns an appropriate bpm message based on tempo changes in the chart.
    /// </summary>
    /// <returns></returns>
    public string GetAutoBpmMessage()
    {
        try
        {
            List<Event> tempoChanges = Events.Where(x => x is TempoChangeEvent).ToList();
            float minTempo = tempoChanges.Min(x => ((TempoChangeEvent)x).Tempo);
            float maxTempo = tempoChanges.Max(x => ((TempoChangeEvent)x).Tempo);

            return minTempo == maxTempo 
                ? $"{minTempo}" 
                : $"{minTempo} - {maxTempo}";
        }
        catch (Exception ex)
        {
            // don't throw
            Console.WriteLine(ex);
        }

        return "???";
    }
    
    /// <summary>
    /// Pre-calculates all values for rendering or gameplay to function properly.
    /// </summary>
    /// <param name="entry"></param>
    public void Build(Entry entry, float audioLength = 0, bool saturnJudgeAreas = false)
    {
        lock (this)
        {
            // Sort everything.
            Events = Events.OrderBy(x => x.Timestamp.FullTick).ToList();
            LaneToggles = LaneToggles.OrderBy(x => x.Timestamp.FullTick).ToList();
            Bookmarks = Bookmarks.OrderBy(x => x.Timestamp.FullTick).ToList();
            
            foreach (Layer layer in Layers)
            {
                layer.Notes = layer.Notes.OrderBy(x => x.Timestamp.FullTick).ToList();
                layer.Events = layer.Events.OrderBy(x => x.Timestamp.FullTick).ToList();
            }
            
            // Update Millisecond Time & ScaledTime.
            // Clear Generated Notes on all layers.
            foreach (Event @event in Events)
            {
                float time = Timestamp.TimeFromTimestamp(this, @event.Timestamp);
                @event.Timestamp.Time = time;
                @event.Timestamp.ScaledTime = time;
            }

            foreach (Bookmark bookmark in Bookmarks)
            {
                float time = Timestamp.TimeFromTimestamp(this, bookmark.Timestamp);
                bookmark.Timestamp.Time = time;
                bookmark.Timestamp.ScaledTime = time;
            }

            foreach (Note laneToggle in LaneToggles)
            {
                float time = Timestamp.TimeFromTimestamp(this, laneToggle.Timestamp);
                laneToggle.Timestamp.Time = time;
                laneToggle.Timestamp.ScaledTime = time;
            }

            // Collection of all playable notes for judge area calculations done later.
            List<Note> playableNotes = [];
            List<HoldNote> playableHoldNotes = [];
            Dictionary<int, List<HoldPointNote>> holdEndNotes = [];
            
            foreach (Layer layer in Layers)
            {
                layer.GeneratedNotes.Clear(); // Clear Generated Notes here to save an enumeration.
            
                // Generate Time and ScaledTime in separate loops, because Timestamp.ScaledTimeFromTime() relies on all events having a valid Time.
                foreach (Event @event in layer.Events)
                {
                    if (@event is StopEffectEvent stopEffectEvent)
                    {
                        stopEffectEvent.SubEvents = stopEffectEvent.SubEvents.OrderBy(x => x.Timestamp.FullTick).ToArray();
                    
                        foreach (EffectSubEvent subEvent in stopEffectEvent.SubEvents)
                        {
                            float time = Timestamp.TimeFromTimestamp(this, subEvent.Timestamp);
                            subEvent.Timestamp.Time = time;
                        }
                    }
                    else if (@event is ReverseEffectEvent reverseEffectEvent)
                    {
                        reverseEffectEvent.SubEvents = reverseEffectEvent.SubEvents.OrderBy(x => x.Timestamp.FullTick).ToArray();
                    
                        foreach (EffectSubEvent subEvent in reverseEffectEvent.SubEvents)
                        {
                            float time = Timestamp.TimeFromTimestamp(this, subEvent.Timestamp);
                            subEvent.Timestamp.Time = time;
                        }
                    }
                    else
                    {
                        float time = Timestamp.TimeFromTimestamp(this, @event.Timestamp);
                        @event.Timestamp.Time = time;
                    }
                }
                
                foreach (Event @event in layer.Events)
                {
                    if (@event is StopEffectEvent stopEffectEvent)
                    {
                        stopEffectEvent.SubEvents = stopEffectEvent.SubEvents.OrderBy(x => x.Timestamp.FullTick).ToArray();
                    
                        foreach (EffectSubEvent subEvent in stopEffectEvent.SubEvents)
                        {
                            subEvent.Timestamp.ScaledTime = Timestamp.ScaledTimeFromTime(layer, subEvent.Timestamp.Time);
                        }
                    }
                    else if (@event is ReverseEffectEvent reverseEffectEvent)
                    {
                        reverseEffectEvent.SubEvents = reverseEffectEvent.SubEvents.OrderBy(x => x.Timestamp.FullTick).ToArray();
                    
                        foreach (EffectSubEvent subEvent in reverseEffectEvent.SubEvents)
                        {
                            subEvent.Timestamp.ScaledTime = Timestamp.ScaledTimeFromTime(layer, subEvent.Timestamp.Time, true);
                        }
                    }
                    else
                    {
                        @event.Timestamp.ScaledTime = Timestamp.ScaledTimeFromTime(layer, @event.Timestamp.Time);
                    }
                }

                foreach (Note note in layer.Notes)
                {
                    if (note is HoldNote holdNote)
                    {
                        foreach (HoldPointNote point in holdNote.Points)
                        {
                            float time = Timestamp.TimeFromTimestamp(this, point.Timestamp);
                            point.Timestamp.Time = time;
                            point.Timestamp.ScaledTime = Timestamp.ScaledTimeFromTime(layer, time);
                        }

                        holdNote.Points = holdNote.Points.OrderBy(x => x.Timestamp.FullTick).ToList();
                        
                        if (holdNote.Points.Count > 1)
                        {
                            playableHoldNotes.Add(holdNote);
                            
                            int holdEndFullTick = holdNote.Points[^1].Timestamp.FullTick;
                            if (holdEndNotes.TryGetValue(holdEndFullTick, out List<HoldPointNote>? holdEndsOnTick))
                            {
                                holdEndsOnTick.Add(holdNote.Points[^1]);
                            }
                            else
                            {
                                holdEndNotes.Add(holdEndFullTick, [holdNote.Points[^1]]);
                            }
                        }
                    }
                    else
                    {
                        float time = Timestamp.TimeFromTimestamp(this, note.Timestamp);
                        note.Timestamp.Time = time;
                        note.Timestamp.ScaledTime = Timestamp.ScaledTimeFromTime(layer, time);
                    }
                    
                    if (note is IPlayable playable)
                    {
                        // Reset judge areas.
                        playable.JudgeArea = playable.JudgeAreaTemplate;
                        playable.JudgeArea.MarvelousEarly += note.Timestamp.Time;
                        playable.JudgeArea.MarvelousLate  += note.Timestamp.Time;
                        playable.JudgeArea.GreatEarly     += note.Timestamp.Time;
                        playable.JudgeArea.GreatLate      += note.Timestamp.Time;
                        playable.JudgeArea.GoodEarly      += note.Timestamp.Time;
                        playable.JudgeArea.GoodLate       += note.Timestamp.Time;
                        
                        // Collect playable notes for judge area processing later.
                        if (playable.JudgementType is not JudgementType.Fake)
                        {
                            playableNotes.Add(note);
                        }
                    }
                }
            }

            // Update Chart End.
            if (entry.AutoChartEnd)
            {
                entry.ChartEnd = FindIdealChartEnd(audioLength);
            }
            else
            {
                entry.ChartEnd.Time = Timestamp.TimeFromTimestamp(this, entry.ChartEnd);
            }
        
            // Create new generated notes.
            for (int l = 0; l < Layers.Count; l++)
            {
                Layer layer = Layers[l];
            
                // Create Measure and Beat Lines
                if (l == 0)
                {
                    for (int i = 0; i <= entry.ChartEnd.Measure; i++)
                    {
                        Timestamp timestamp = new(i, 0);
                        timestamp.Time = Timestamp.TimeFromTimestamp(this, timestamp);
                        timestamp.ScaledTime = Timestamp.ScaledTimeFromTime(layer, timestamp.Time);
                    
                        layer.GeneratedNotes.Add(new MeasureLineNote(timestamp, false));
                    }
                
                    List<MetreChangeEvent> metreChangeEvents = Events.OfType<MetreChangeEvent>().ToList();

                    for (int i = 0; i < metreChangeEvents.Count; i++)
                    {
                        int step = 1920 / metreChangeEvents[i].Upper;
                        float m = 1920.0f / metreChangeEvents[i].Upper;
                        
                        int roundedStartTick = (int)(Math.Floor(metreChangeEvents[i].Timestamp.FullTick / m) * m);

                        int startTick = roundedStartTick < metreChangeEvents[i].Timestamp.FullTick 
                            ? roundedStartTick + step 
                            : roundedStartTick;
                        
                        int endTick = i == metreChangeEvents.Count - 1
                            ? entry.ChartEnd.FullTick
                            : metreChangeEvents[i + 1].Timestamp.FullTick;
                        
                        for (int j = startTick; j < endTick; j += step)
                        {
                            // I prefer giving people the option to have only beat lines.
                            // if (j % 1920 == 0) continue;
                        
                            Timestamp timestamp = new(j);
                            timestamp.Time = Timestamp.TimeFromTimestamp(this, timestamp);
                            timestamp.ScaledTime = Timestamp.ScaledTimeFromTime(layer, timestamp.Time);
                        
                            layer.GeneratedNotes.Add(new MeasureLineNote(timestamp, true));
                        }
                    }
                }
            
                // Create Sync Notes
                for (int i = 1; i < layer.Notes.Count; i++)
                {
                    Note current = layer.Notes[i];
                    Note previous = layer.Notes[i - 1];

                    if (!current.IsSync(previous)) continue;
                    if (current is not IPositionable currentPositionable) continue;
                    if (previous is not IPositionable previousPositionable) continue;

                    // -1 + 60
                    // => + 59
                    int position0 = (currentPositionable.Position + currentPositionable.Size + 59) % 60;
                    int position1 = (previousPositionable.Position + previousPositionable.Size + 59) % 60;

                    int size0 = (previousPositionable.Position - position0 + 60) % 60 + 1;
                    int size1 = (currentPositionable.Position  - position1 + 60) % 60 + 1;

                    int finalPosition = size0 < size1 ? position0 : position1;
                    int finalSize = Math.Min(size0, size1);

                    if (finalSize > 30) continue;

                    SyncNote syncNote = new(current.Timestamp, finalPosition, finalSize);
                    layer.GeneratedNotes.Add(syncNote);
                }

                layer.GeneratedNotes = layer.GeneratedNotes.OrderBy(x => x.Timestamp.FullTick).ToList();
            }
        
            // Re-build reverse effect note lists.
            foreach (Layer layer in Layers)
            {
                foreach (Event @event in layer.Events)
                {
                    if (@event is not ReverseEffectEvent reverseEffectEvent) continue;
                    if (reverseEffectEvent.SubEvents.Length != 3) continue;

                    reverseEffectEvent.ContainedNotes.Clear();
                
                    foreach (Note note in layer.Notes)
                    {
                        if (note.Timestamp.FullTick <= reverseEffectEvent.SubEvents[1].Timestamp.FullTick) continue;
                        if (note.Timestamp.FullTick >= reverseEffectEvent.SubEvents[2].Timestamp.FullTick) continue;

                        if (note is HoldNote holdNote && holdNote.Points.Count != 0)
                        {
                            int end = holdNote.Points[^1].Timestamp.FullTick;
                            if (end >= reverseEffectEvent.SubEvents[2].Timestamp.FullTick) continue;
                        }
                    
                        reverseEffectEvent.ContainedNotes.Add(note);
                    }

                    // Not source-game accurate, but looks nicer to include measure/beat lines and sync notes :)
                    foreach (Note note in layer.GeneratedNotes)
                    {
                        // It looks nicer if measure lines on top of the reverse sub-events are included as well.
                        // Do the == checks only for sync notes.
                        if (note is SyncNote)
                        {
                            if (note.Timestamp.FullTick <= reverseEffectEvent.SubEvents[1].Timestamp.FullTick) continue;
                            if (note.Timestamp.FullTick >= reverseEffectEvent.SubEvents[2].Timestamp.FullTick) continue;
                        }
                        else
                        {
                            if (note.Timestamp.FullTick < reverseEffectEvent.SubEvents[1].Timestamp.FullTick) continue;
                            if (note.Timestamp.FullTick > reverseEffectEvent.SubEvents[2].Timestamp.FullTick) continue;
                        }
                    
                        reverseEffectEvent.ContainedNotes.Add(note);
                    }
                }
            }
            
            // Process judge areas.
            playableNotes = playableNotes.OrderBy(x => x.Timestamp.FullTick).ToList();
            
            // Follow Saturn-spec
            if (saturnJudgeAreas)
            {
                // Notes on hold ends
                // - Early GREAT/GOOD areas are removed.
                foreach (Note note in playableNotes)
                {
                    if (!holdEndNotes.TryGetValue(note.Timestamp.FullTick, out List<HoldPointNote>? holdEndsOnTick)) continue;

                    foreach (HoldPointNote holdEnd in holdEndsOnTick)
                    {
                        if (!IPositionable.IsAnyOverlap((IPositionable)note, holdEnd)) continue;
                        
                        JudgeArea judgeArea = ((IPlayable)note).JudgeArea;
                        judgeArea.GoodEarly = judgeArea.MarvelousEarly;
                        judgeArea.GreatEarly = judgeArea.MarvelousEarly;
                        
                        break;
                    }
                }

                // Slide & Snap notes on hold starts
                // - All early areas of the hold note are converted to MARVELOUS.
                for (int i = 0; i < playableNotes.Count; i++)
                {
                    Note note = playableNotes[i];
                    if (note is not (SlideClockwiseNote or SlideCounterclockwiseNote or SnapForwardNote or SnapBackwardNote)) continue;
                    if (note is not IPositionable positionable) continue;

                    // Look backward for notes on same tick.
                    for (int j = i; j > 0; j--)
                    {
                        Note other = playableNotes[j];
                        if (other.Timestamp.FullTick != note.Timestamp.FullTick) break;

                        if (other is not HoldNote holdNote) continue;
                        if (!IPositionable.IsAnyOverlap(positionable, holdNote)) continue;
                        
                        JudgeArea judgeArea = holdNote.JudgeArea;
                        judgeArea.MarvelousEarly = judgeArea.GoodEarly;
                        judgeArea.GreatEarly = judgeArea.GoodEarly;
                    }

                    // Look forward for notes on same tick.
                    for (int j = i; j < playableNotes.Count; j++)
                    {
                        Note other = playableNotes[j];
                        if (other.Timestamp.FullTick != note.Timestamp.FullTick) break;

                        if (other is not HoldNote holdNote) continue;
                        if (!IPositionable.IsAnyOverlap(positionable, holdNote)) continue;
                        
                        JudgeArea judgeArea = holdNote.JudgeArea;
                        judgeArea.MarvelousEarly = judgeArea.GoodEarly;
                        judgeArea.GreatEarly = judgeArea.GoodEarly;
                    }
                }

                // Notes inside holds
                // - Early GREAT/GOOD areas are cut in half.
                for (int i = 0; i < playableHoldNotes.Count; i++)
                {
                    HoldNote holdNote = playableHoldNotes[i];
                    
                    for (int j = 0; j < playableNotes.Count; j++)
                    {
                        Note note = playableNotes[j];
                        if (note is not IPositionable positionable) continue;
                        if (note is not IPlayable playable) continue;

                        if (note.Timestamp.FullTick <= holdNote.Points[0].Timestamp.FullTick) continue;
                        if (note.Timestamp.FullTick >= holdNote.Points[^1].Timestamp.FullTick) continue;

                        HoldPointNote? start = null;
                        HoldPointNote? end = null;

                        // Find first point that's later than note.
                        for (int k = 0; k < holdNote.Points.Count; k++)
                        {
                            HoldPointNote point = holdNote.Points[k];
                            if (point.Timestamp.FullTick <= note.Timestamp.FullTick) continue;

                            end = point;
                            break;
                        }
                        
                        // Find last point that's earlier than note.
                        for (int k = holdNote.Points.Count - 1; k >= 0; k--)
                        {
                            HoldPointNote point = holdNote.Points[k];
                            if (point.Timestamp.FullTick >= note.Timestamp.FullTick) continue;

                            start = point;
                            break;
                        }
                        
                        if (start == null) continue;
                        if (end == null) continue;

                        float t = SaturnMath.InverseLerp(start.Timestamp.FullTick, end.Timestamp.FullTick, note.Timestamp.FullTick);
                        
                        int position = (int)Math.Round(SaturnMath.LerpCyclic(start.Position, end.Position, t, 60));
                        int size = (int)Math.Round(SaturnMath.Lerp(start.Size, end.Size, t));

                        if (!IPositionable.IsAnyOverlap(positionable.Position, positionable.Size, position, size)) continue;

                        playable.JudgeArea.GreatEarly += (playable.JudgeArea.MarvelousEarly - playable.JudgeArea.GreatEarly) * 0.5f;
                        playable.JudgeArea.GoodEarly = playable.JudgeArea.GreatEarly;
                    }
                }
                
                // Overlapping judge areas
                // - Do not truncate MARVELOUS area.
                for (int i = 0; i < playableNotes.Count; i++)
                {
                    Note note = playableNotes[i];
                    
                    if (note is not IPlayable playable) continue;
                    if (note is not IPositionable positionable) continue;
                    
                    for (int j = i + 1; j < playableNotes.Count; j++)
                    {
                        Note other = playableNotes[j];

                        if (note.Timestamp.FullTick == other.Timestamp.FullTick) continue;
                        
                        // Stop processing if judge areas are far enough out of range.
                        // Use a +200ms "safety net" for any larger judge areas. Hacky and inefficient, but keeps things simple.
                        if (other is not IPlayable otherPlayable) continue;
                        if (otherPlayable.JudgeArea.MaxEarly > playable.JudgeArea.MaxLate + 200) break;
                        if (otherPlayable.JudgeArea.MaxEarly > playable.JudgeArea.MaxLate) continue;
                        
                        if (other is not IPositionable otherPositionable) continue;
                        if (!IPositionable.IsAnyOverlap(positionable, otherPositionable)) continue;

                        // Valid overlapping note was found.
                        float centerTime = (note.Timestamp.Time + other.Timestamp.Time) * 0.5f;
                        centerTime = Math.Min(centerTime, playable.JudgeArea.MaxLate);
                        centerTime = Math.Max(centerTime, otherPlayable.JudgeArea.MaxEarly);

                        playable.JudgeArea.GoodLate  = Math.Min(centerTime, playable.JudgeArea.GoodLate);
                        playable.JudgeArea.GreatLate = Math.Min(centerTime, playable.JudgeArea.GreatLate);
                        
                        playable.JudgeArea.GoodLate  = Math.Max(playable.JudgeArea.MarvelousLate, playable.JudgeArea.GoodLate);
                        playable.JudgeArea.GreatLate = Math.Max(playable.JudgeArea.MarvelousLate, playable.JudgeArea.GreatLate);
                        
                        
                        otherPlayable.JudgeArea.GoodEarly  = Math.Max(centerTime, otherPlayable.JudgeArea.GoodEarly);
                        otherPlayable.JudgeArea.GreatEarly = Math.Max(centerTime, otherPlayable.JudgeArea.GreatEarly);
                        
                        otherPlayable.JudgeArea.GoodEarly  = Math.Min(otherPlayable.JudgeArea.MarvelousEarly, otherPlayable.JudgeArea.GoodEarly);
                        otherPlayable.JudgeArea.GreatEarly = Math.Min(otherPlayable.JudgeArea.MarvelousEarly, otherPlayable.JudgeArea.GreatEarly);
                    }
                }
            }
            // Follow Mer-spec
            else
            {
                // Notes on hold ends
                // - Early GREAT/GOOD areas are removed.
                foreach (Note note in playableNotes)
                {
                    if (!holdEndNotes.TryGetValue(note.Timestamp.FullTick, out List<HoldPointNote>? holdEndsOnTick)) continue;

                    foreach (HoldPointNote holdEnd in holdEndsOnTick)
                    {
                        if (!IPositionable.IsAnyOverlap((IPositionable)note, holdEnd)) continue;
                        
                        JudgeArea judgeArea = ((IPlayable)note).JudgeArea;
                        judgeArea.GoodEarly = judgeArea.MarvelousEarly;
                        judgeArea.GreatEarly = judgeArea.MarvelousEarly;
                        
                        break;
                    }
                }
                
                // Overlapping judge areas
                for (int i = 0; i < playableNotes.Count; i++)
                {
                    Note note = playableNotes[i];
                    
                    if (note is not IPlayable playable) continue;
                    if (note is not IPositionable positionable) continue;
                    
                    for (int j = i + 1; j < playableNotes.Count; j++)
                    {
                        Note other = playableNotes[j];

                        if (note.Timestamp.FullTick == other.Timestamp.FullTick) continue;
                        
                        // Stop processing if judge areas are far enough out of range.
                        // Use a +200ms "safety net" for any larger judge areas. Hacky and inefficient, but keeps things simple.
                        if (other is not IPlayable otherPlayable) continue;
                        if (otherPlayable.JudgeArea.MaxEarly > playable.JudgeArea.MaxLate + 200) break;
                        if (otherPlayable.JudgeArea.MaxEarly > playable.JudgeArea.MaxLate) continue;
                        
                        if (other is not IPositionable otherPositionable) continue;
                        if (!IPositionable.IsAnyOverlap(positionable, otherPositionable)) continue;

                        // Valid overlapping note was found.
                        float centerTime = (note.Timestamp.Time + other.Timestamp.Time) * 0.5f;
                        centerTime = Math.Min(centerTime, playable.JudgeArea.MaxLate);
                        centerTime = Math.Max(centerTime, otherPlayable.JudgeArea.MaxEarly);

                        playable.JudgeArea.GoodLate      = Math.Min(centerTime, playable.JudgeArea.GoodLate);
                        playable.JudgeArea.GreatLate     = Math.Min(centerTime, playable.JudgeArea.GreatLate);
                        playable.JudgeArea.MarvelousLate = Math.Min(centerTime, playable.JudgeArea.MarvelousLate);
                        otherPlayable.JudgeArea.GoodEarly      = Math.Max(centerTime, otherPlayable.JudgeArea.GoodEarly);
                        otherPlayable.JudgeArea.GreatEarly     = Math.Max(centerTime, otherPlayable.JudgeArea.GreatEarly);
                        otherPlayable.JudgeArea.MarvelousEarly = Math.Max(centerTime, otherPlayable.JudgeArea.MarvelousEarly);
                    }
                }
            }
            
            // Create Scaled Timing areas for rendering.
            foreach (Layer layer in Layers)
            foreach (Note note in layer.Notes)
            {
                if (note is not IPlayable playable) continue;
                
                playable.JudgeArea.ScaledMarvelousEarly = Timestamp.ScaledTimeFromTime(layer, playable.JudgeArea.MarvelousEarly);
                playable.JudgeArea.ScaledMarvelousLate  = Timestamp.ScaledTimeFromTime(layer, playable.JudgeArea.MarvelousLate);
                playable.JudgeArea.ScaledGreatEarly     = Timestamp.ScaledTimeFromTime(layer, playable.JudgeArea.GreatEarly);
                playable.JudgeArea.ScaledGreatLate      = Timestamp.ScaledTimeFromTime(layer, playable.JudgeArea.GreatLate);
                playable.JudgeArea.ScaledGoodEarly      = Timestamp.ScaledTimeFromTime(layer, playable.JudgeArea.GoodEarly);
                playable.JudgeArea.ScaledGoodLate       = Timestamp.ScaledTimeFromTime(layer, playable.JudgeArea.GoodLate);
            }
            
            // Update PreviewBegin and PreviewEnd
            if (entry.PreviewBeginTime != null && entry.PreviewLengthTime != null)
            {
                entry.PreviewBegin = Timestamp.TimestampFromTime(this, entry.PreviewBeginTime.Value);
                entry.PreviewEnd = Timestamp.TimestampFromTime(this, entry.PreviewBeginTime.Value + entry.PreviewLengthTime.Value);
                
                entry.PreviewBeginTime = null;
                entry.PreviewLengthTime = null;
            }
            else
            {
                entry.PreviewBegin.Time = Timestamp.TimeFromTimestamp(this, entry.PreviewBegin);
                entry.PreviewBegin.ScaledTime = entry.PreviewBegin.Time;
                
                entry.PreviewEnd.Time = Timestamp.TimeFromTimestamp(this, entry.PreviewEnd);
                entry.PreviewEnd.ScaledTime = entry.PreviewEnd.Time;
            }
        }
    }
}