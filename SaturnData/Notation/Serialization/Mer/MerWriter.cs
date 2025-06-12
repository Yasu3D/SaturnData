using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SaturnData.Notation.Core;
using SaturnData.Notation.Events;
using SaturnData.Notation.Interfaces;
using SaturnData.Notation.Notes;

namespace SaturnData.Notation.Serialization.Mer;

public static class MerWriter
{
    private class MerWriterNote
    {
        public int Measure { get; set; }

        public int Tick { get; set; }

        public int NoteType { get; set; }

        public int Position { get; set; }

        public int Size { get; set; }

        public int Render { get; set; }

        public int? Direction { get; set; }

        public MerWriterNote? Reference { get; set; }
    }

    private class MerWriterEvent
    {
        public int Measure { get; set; }
        
        public int Tick { get; set; }

        public int ObjectType { get; set; }
        
        public float? FloatValue { get; set; }
        
        public int? IntValue1 { get; set; }
        
        public int? IntValue2 { get; set; }
    }
    
    /// <summary>
    /// Converts a chart into a string.
    /// </summary>
    /// <param name="entry">The entry to serialize.</param>
    /// <param name="chart">The chart to serialize.</param>
    /// <returns></returns>
    public static string ToString(Entry entry, Chart chart, NotationSerializerOptions options)
    {
        StringBuilder sb = new();

        WriteMetadata(sb, entry, options);
        WriteEvents(sb, chart, options);
        WriteNotes(sb, chart, options);

        return sb.ToString();
    }
    
    /// <summary>
    /// Writes the metadata header of a <c>.mer</c> file.
    /// </summary>
    /// <param name="sb">The StringBuilder to use.</param>
    /// <param name="entry">The entry that holds the metadata to write.</param>
    /// <param name="options">Options to adjust serialization behaviour.</param>
    internal static void WriteMetadata(StringBuilder sb, Entry entry, NotationSerializerOptions options)
    {
        sb.Append("#MUSIC_SCORE_ID 0\n");
        sb.Append("#MUSIC_SCORE_VERSION 0\n");
        sb.Append("#GAME_VERSION\n");

        switch (options.WriteMerMusicFilePath)
        {
            case NotationSerializerOptions.WriteMerMusicFilePathOption.None:
                sb.Append("#MUSIC_FILE_PATH\n");
                break;

            case NotationSerializerOptions.WriteMerMusicFilePathOption.NoExtension:
                sb.Append($"#MUSIC_FILE_PATH {Path.GetFileNameWithoutExtension(entry.AudioPath)}\n");
                break;

            case NotationSerializerOptions.WriteMerMusicFilePathOption.WithExtension:
                sb.Append($"#MUSIC_FILE_PATH {Path.GetFileName(entry.AudioPath)}\n");
                break;
        }

        sb.Append($"#OFFSET {entry.AudioOffset / 1000}\n");
        sb.Append($"#MOVIEOFFSET {entry.VideoOffset / 1000}\n");
        sb.Append("#BODY\n");
    }

    /// <summary>
    /// Writes the event list of a <c>.mer</c> file.
    /// </summary>
    /// <param name="sb">The StringBuilder to use.</param>
    /// <param name="chart">The chart that holds the events to write.</param>
    /// <param name="options">Options to adjust serialization behaviour.</param>
    internal static void WriteEvents(StringBuilder sb, Chart chart, NotationSerializerOptions options)
    {
        List<MerWriterEvent> events = [];
        
        // Add all global events
        foreach (Event globalEvent in chart.GlobalEvents)
        {
            if (globalEvent is BpmChangeEvent bpmChangeEvent)
            {
                events.Add(new ()
                {
                    Measure = bpmChangeEvent.Timestamp.Measure,
                    Tick = bpmChangeEvent.Timestamp.Tick,
                    ObjectType = 2,
                    FloatValue = bpmChangeEvent.Bpm,
                });
                
                continue;
            }

            if (globalEvent is TimeSignatureChangeEvent timeSignatureChangeEvent)
            {
                events.Add(new ()
                {
                    Measure = timeSignatureChangeEvent.Timestamp.Measure,
                    Tick = timeSignatureChangeEvent.Timestamp.Tick,
                    ObjectType = 3,
                    IntValue1 = timeSignatureChangeEvent.Upper,
                    IntValue2 = timeSignatureChangeEvent.Lower,
                });
                
                continue;
            }
        }

        // Add all layer-specific events from the first layer
        Layer<Event>? firstLayer = chart.EventLayers.Values.FirstOrDefault();
        if (firstLayer != null)
        {
            foreach (Event @event in firstLayer.Items)
            {
                if (@event is InvisibleEffectEvent) continue;

                if (@event is ReverseEffectEvent reverseEffectEvent)
                {
                    if (reverseEffectEvent.SubEvents.Length != 3) continue;

                    events.Add(new()
                    {
                        Measure = reverseEffectEvent.SubEvents[0].Timestamp.Measure,
                        Tick = reverseEffectEvent.SubEvents[0].Timestamp.Tick,
                        ObjectType = 6,
                    });

                    events.Add(new()
                    {
                        Measure = reverseEffectEvent.SubEvents[1].Timestamp.Measure,
                        Tick = reverseEffectEvent.SubEvents[1].Timestamp.Tick,
                        ObjectType = 7,
                    });

                    events.Add(new()
                    {
                        Measure = reverseEffectEvent.SubEvents[2].Timestamp.Measure,
                        Tick = reverseEffectEvent.SubEvents[2].Timestamp.Tick,
                        ObjectType = 8,
                    });

                    continue;
                }

                if (@event is StopEffectEvent stopEffectEvent)
                {
                    if (stopEffectEvent.SubEvents.Length != 2) continue;

                    events.Add(new()
                    {
                        Measure = stopEffectEvent.SubEvents[0].Timestamp.Measure,
                        Tick = stopEffectEvent.SubEvents[0].Timestamp.Tick,
                        ObjectType = 9,
                    });

                    events.Add(new()
                    {
                        Measure = stopEffectEvent.SubEvents[1].Timestamp.Measure,
                        Tick = stopEffectEvent.SubEvents[1].Timestamp.Tick,
                        ObjectType = 10,
                    });

                    continue;
                }

                if (@event is HiSpeedChangeEvent hiSpeedChangeEvent)
                {
                    events.Add(new()
                    {
                        Measure = hiSpeedChangeEvent.Timestamp.Measure,
                        Tick = hiSpeedChangeEvent.Timestamp.Tick,
                        ObjectType = 5,
                        FloatValue = hiSpeedChangeEvent.HiSpeed,
                    });
                    
                    continue;
                }
            }
        }

        // Order events by timestamp.
        events = events
            .OrderBy(x => x.Measure)
            .ThenBy(x => x.Tick)
            .ToList();

        // Create string with StringBuilder
        foreach (MerWriterEvent @event in events)
        {
            sb.Append($"{@event.Measure,4} {@event.Tick,4} {@event.ObjectType,4}");

            if (@event.FloatValue != null)
            {
                sb.Append($" {@event.FloatValue:F6}");
            }

            if (@event.IntValue1 != null && @event.IntValue2 != null)
            {
                sb.Append($" {@event.IntValue1,4} {@event.IntValue2,4}");
            }

            sb.Append("\n");
        }
    }

    /// <summary>
    /// Writes the note list of a <c>.mer</c> file.
    /// </summary>
    /// <param name="sb">The StringBuilder to use.</param>
    /// <param name="chart">The chart that holds the notes to write.</param>
    /// <param name="options">Options to adjust serialization behaviour.</param>
    internal static void WriteNotes(StringBuilder sb, Chart chart, NotationSerializerOptions options)
    {
        List<MerWriterNote> notes = [];
        
        // Add all note layers.
        foreach (Layer<Note> layer in chart.NoteLayers.Values)
        foreach (Note note in layer.Items)
        {
            if (note is TouchNote touchNote)
            {
                notes.Add(new()
                {
                    Measure = touchNote.Timestamp.Measure,
                    Tick = touchNote.Timestamp.Tick,
                    NoteType = touchNote.BonusType switch
                    {
                        BonusType.None => 1,
                        BonusType.Bonus => 2,
                        BonusType.R => 20,
                        _ => 1,
                    },
                    Position = touchNote.Position,
                    Size = touchNote.Size,
                    Render = 1,
                });

                continue;
            }
            
            if (note is SnapForwardNote snapForwardNote)
            {
                notes.Add(new()
                {
                    Measure = snapForwardNote.Timestamp.Measure,
                    Tick = snapForwardNote.Timestamp.Tick,
                    NoteType = snapForwardNote.BonusType switch
                    {
                        BonusType.None => 3,
                        BonusType.Bonus => 3,
                        BonusType.R => 21,
                        _ => 3,
                    },
                    Position = snapForwardNote.Position,
                    Size = snapForwardNote.Size,
                    Render = 1,
                });

                continue;
            }
            
            if (note is SnapBackwardNote snapBackwardNote)
            {
                notes.Add(new()
                {
                    Measure = snapBackwardNote.Timestamp.Measure,
                    Tick = snapBackwardNote.Timestamp.Tick,
                    NoteType = snapBackwardNote.BonusType switch
                    {
                        BonusType.None => 4,
                        BonusType.Bonus => 4,
                        BonusType.R => 22,
                        _ => 4,
                    },
                    Position = snapBackwardNote.Position,
                    Size = snapBackwardNote.Size,
                    Render = 1,
                });

                continue;
            }
            
            if (note is SlideClockwiseNote slideClockwiseNote)
            {
                notes.Add(new()
                {
                    Measure = slideClockwiseNote.Timestamp.Measure,
                    Tick = slideClockwiseNote.Timestamp.Tick,
                    NoteType = slideClockwiseNote.BonusType switch
                    {
                        BonusType.None => 5,
                        BonusType.Bonus => 6,
                        BonusType.R => 23,
                        _ => 5,
                    },
                    Position = slideClockwiseNote.Position,
                    Size = slideClockwiseNote.Size,
                    Render = 1,
                });

                continue;
            }
            
            if (note is SlideCounterclockwiseNote slideCounterclockwiseNote)
            {
                notes.Add(new()
                {
                    Measure = slideCounterclockwiseNote.Timestamp.Measure,
                    Tick = slideCounterclockwiseNote.Timestamp.Tick,
                    NoteType = slideCounterclockwiseNote.BonusType switch
                    {
                        BonusType.None => 7,
                        BonusType.Bonus => 8,
                        BonusType.R => 24,
                        _ => 7,
                    },
                    Position = slideCounterclockwiseNote.Position,
                    Size = slideCounterclockwiseNote.Size,
                    Render = 1,
                });

                continue;
            }
            
            if (note is ChainNote chainNote)
            {
                notes.Add(new()
                {
                    Measure = chainNote.Timestamp.Measure,
                    Tick = chainNote.Timestamp.Tick,
                    NoteType = chainNote.BonusType switch
                    {
                        BonusType.None => 16,
                        BonusType.Bonus => 16,
                        BonusType.R => 26,
                        _ => 16,
                    },
                    Position = chainNote.Position,
                    Size = chainNote.Size,
                    Render = 1,
                });

                continue;
            }

            if (note is HoldNote holdNote)
            {
                MerWriterNote? lastNoteToReference = null;

                // Loop from end to start to easily get "next" reference.
                for (int i = holdNote.Points.Count - 1; i >= 0; i--)
                {
                    HoldPointNote point = holdNote.Points[i];

                    MerWriterNote merWriterNote = new()
                    {
                        Measure = point.Timestamp.Measure,
                        Tick = point.Timestamp.Tick,
                        Position = point.Position,
                        Size = point.Size,
                    };

                    // Hold End
                    if (i == holdNote.Points.Count - 1)
                    {
                        merWriterNote.NoteType = 11;
                        merWriterNote.Render = 1;
                    }

                    // Hold Start
                    else if (i == 0)
                    {
                        merWriterNote.NoteType = holdNote.BonusType switch
                        {
                            BonusType.None => 9,
                            BonusType.Bonus => 9,
                            BonusType.R => 25,
                            _ => 9,
                        };
                        merWriterNote.Render = 1;
                        merWriterNote.Reference = lastNoteToReference;
                    }

                    // Hold Points
                    else
                    {
                        merWriterNote.NoteType = 10;
                        merWriterNote.Render = (int)point.RenderBehaviour;
                        merWriterNote.Reference = lastNoteToReference;
                    }

                    notes.Add(merWriterNote);
                    lastNoteToReference = merWriterNote;
                }

                continue;
            }
        }
        
        // Add all mask notes.
        foreach (Note mask in chart.Masks)
        {
            if (mask is MaskAddNote maskAddNote)
            {
                notes.Add(new()
                {
                    Measure = maskAddNote.Timestamp.Measure,
                    Tick = maskAddNote.Timestamp.Tick,
                    NoteType = 12,
                    Position = maskAddNote.Position,
                    Size = maskAddNote.Size,
                    Render = 1,
                    Direction = (int)maskAddNote.Direction,
                });

                continue;
            }

            if (mask is MaskSubtractNote maskSubtractNote)
            {
                notes.Add(new()
                {
                    Measure = maskSubtractNote.Timestamp.Measure,
                    Tick = maskSubtractNote.Timestamp.Tick,
                    NoteType = 13,
                    Position = maskSubtractNote.Position,
                    Size = maskSubtractNote.Size,
                    Render = 1,
                    Direction = (int)maskSubtractNote.Direction,
                });

                continue;
            }
        }
        
        // Add chart end.
        if (options.GenerateChartEnd && chart.ChartEnd == null)
        {
            throw new NotImplementedException();
        }
        else if (chart.ChartEnd != null)
        {
            notes.Add(new()
            {
                Measure = chart.ChartEnd.Value.Measure,
                Tick = chart.ChartEnd.Value.Tick,
                NoteType = 14,
                Position = 0,
                Size = 60,
                Render = 1,
            });
        }
        
        notes = notes
            .OrderBy(x => x.Measure)
            .ThenBy(x => x.Tick)
            .ToList();

        for (int i = 0; i < notes.Count; i++)
        {
            MerWriterNote writerNote = notes[i];
            sb.Append($"{writerNote.Measure,4} {writerNote.Tick,4}    1 {writerNote.NoteType,4} {i,4} {writerNote.Position,4} {writerNote.Size,4} {writerNote.Render,4}");

            if (writerNote.Direction != null)
            {
                sb.Append($" {writerNote.Direction,4}");
            }

            if (writerNote.Reference != null)
            {
                int index = notes.IndexOf(writerNote.Reference);
                sb.Append($" {index,4}");
            }

            sb.Append("\n");
        }
    }
}