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
        public Timestamp Timestamp { get; set; }

        public int NoteType { get; set; }

        public int Position { get; set; }

        public int Size { get; set; }

        public int Render { get; set; }

        public int? Direction { get; set; }

        public MerWriterNote? Reference { get; set; }
    }

    private class MerWriterEvent
    {
        public Timestamp Timestamp { get; set; }

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
    public static string ToString(Entry entry, Chart chart, NotationWriteArgs args)
    {
        StringBuilder sb = new();

        WriteMetadata(sb, entry, args);
        WriteEvents(sb, chart, args);
        WriteNotes(sb, chart, entry, args);

        return sb.ToString();
    }
    
    /// <summary>
    /// Writes the metadata header of a <c>.mer</c> file.
    /// </summary>
    /// <param name="sb">The StringBuilder to use.</param>
    /// <param name="entry">The entry that holds the metadata to write.</param>
    /// <param name="args">Options to adjust serialization behaviour.</param>
    public static void WriteMetadata(StringBuilder sb, Entry entry, NotationWriteArgs args)
    {
        sb.Append("#MUSIC_SCORE_ID 0\n");
        sb.Append("#MUSIC_SCORE_VERSION 0\n");
        sb.Append("#GAME_VERSION\n");

        switch (args.WriteMusicFilePath)
        {
            case WriteMusicFilePathOption.None:
                sb.Append("#MUSIC_FILE_PATH\n");
                break;

            case WriteMusicFilePathOption.NoExtension:
                sb.Append($"#MUSIC_FILE_PATH {Path.GetFileNameWithoutExtension(entry.AudioFile)}\n");
                break;

            case WriteMusicFilePathOption.WithExtension:
                sb.Append($"#MUSIC_FILE_PATH {Path.GetFileName(entry.AudioFile)}\n");
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
    /// <param name="args">Options to adjust serialization behaviour.</param>
    public static void WriteEvents(StringBuilder sb, Chart chart, NotationWriteArgs args)
    {
        List<MerWriterEvent> events = [];
        
        // Add all global events
        foreach (Event globalEvent in chart.Events)
        {
            if (globalEvent is TempoChangeEvent bpmChangeEvent)
            {
                events.Add(new ()
                {
                    Timestamp = bpmChangeEvent.Timestamp,
                    ObjectType = 2,
                    FloatValue = bpmChangeEvent.Tempo,
                });
                
                continue;
            }

            if (globalEvent is MetreChangeEvent timeSignatureChangeEvent)
            {
                events.Add(new ()
                {
                    Timestamp = timeSignatureChangeEvent.Timestamp,
                    ObjectType = 3,
                    IntValue1 = timeSignatureChangeEvent.Upper,
                    IntValue2 = timeSignatureChangeEvent.Lower,
                });
            }
        }

        // Add all layer-specific events
        for (int i = 0; i < chart.Layers.Count; i++)
        {
            if (i > 0 && args.MergeExtraLayers is MergeExtraLayersOption.ExcludeFromExport or MergeExtraLayersOption.MergeIntoMainLayerWithoutEvents) break;
            
            foreach (Event @event in chart.Layers[i].Events)
            {
                if (@event is VisibilityChangeEvent) continue;

                if (@event is ReverseEffectEvent reverseEffectEvent)
                {
                    if (reverseEffectEvent.SubEvents.Length != 3) continue;

                    events.Add(new()
                    {
                        Timestamp = reverseEffectEvent.SubEvents[0].Timestamp,
                        ObjectType = 6,
                    });

                    events.Add(new()
                    {
                        Timestamp = reverseEffectEvent.SubEvents[1].Timestamp,
                        ObjectType = 7,
                    });

                    events.Add(new()
                    {
                        Timestamp = reverseEffectEvent.SubEvents[2].Timestamp,
                        ObjectType = 8,
                    });

                    continue;
                }

                if (@event is StopEffectEvent stopEffectEvent)
                {
                    if (stopEffectEvent.SubEvents.Length != 2) continue;

                    events.Add(new()
                    {
                        Timestamp = stopEffectEvent.SubEvents[0].Timestamp,
                        ObjectType = 9,
                    });

                    events.Add(new()
                    {
                        Timestamp = stopEffectEvent.SubEvents[1].Timestamp,
                        ObjectType = 10,
                    });

                    continue;
                }

                if (@event is SpeedChangeEvent hiSpeedChangeEvent)
                {
                    events.Add(new()
                    {
                        Timestamp = hiSpeedChangeEvent.Timestamp,
                        ObjectType = 5,
                        FloatValue = hiSpeedChangeEvent.HiSpeed,
                    });
                }
            }
        }

        // Order events by timestamp.
        events = events
            .OrderBy(x => x.Timestamp)
            .ToList();

        // Create string with StringBuilder
        foreach (MerWriterEvent @event in events)
        {
            sb.Append($"{@event.Timestamp.Measure,4} {@event.Timestamp.Tick,4} {@event.ObjectType,4}");

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
    /// <param name="args">Options to adjust serialization behaviour.</param>
    public static void WriteNotes(StringBuilder sb, Chart chart, Entry entry, NotationWriteArgs args)
    {
        List<MerWriterNote> notes = [];
        
        // Add all note layers.
        for (int i = 0; i < chart.Layers.Count; i++)
        {
            if (i > 0 && args.MergeExtraLayers is MergeExtraLayersOption.ExcludeFromExport) break;
            
            foreach (Note note in chart.Layers[i].Notes)
            {
                if (note is IPlayable playable)
                {
                    if (args.ConvertFakeNotes == ConvertFakeNotesOption.ExcludeFromExport && playable.JudgementType == JudgementType.Fake) continue;
                    if (args.ConvertAutoplayNotes == ConvertAutoplayNotesOption.ExcludeFromExport && playable.JudgementType == JudgementType.Autoplay) continue;
                }
                
                if (note is TouchNote touchNote)
                {
                    notes.Add(new()
                    {
                        Timestamp = touchNote.Timestamp,
                        NoteType = touchNote.BonusType switch
                        {
                            BonusType.Normal => 1,
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
                        Timestamp = snapForwardNote.Timestamp,
                        NoteType = snapForwardNote.BonusType switch
                        {
                            BonusType.Normal => 3,
                            BonusType.Bonus => args.ConvertExtendedBonusTypes == ConvertExtendedBonusTypesOption.ConvertToNormal ? 3 : 21,
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
                        Timestamp = snapBackwardNote.Timestamp,
                        NoteType = snapBackwardNote.BonusType switch
                        {
                            BonusType.Normal => 4,
                            BonusType.Bonus => args.ConvertExtendedBonusTypes == ConvertExtendedBonusTypesOption.ConvertToNormal ? 4 : 22,
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
                        Timestamp = slideClockwiseNote.Timestamp,
                        NoteType = slideClockwiseNote.BonusType switch
                        {
                            BonusType.Normal => 5,
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
                        Timestamp = slideCounterclockwiseNote.Timestamp,
                        NoteType = slideCounterclockwiseNote.BonusType switch
                        {
                            BonusType.Normal => 7,
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
                        Timestamp = chainNote.Timestamp,
                        NoteType = chainNote.BonusType switch
                        {
                            BonusType.Normal => 16,
                            BonusType.Bonus => args.ConvertExtendedBonusTypes == ConvertExtendedBonusTypesOption.ConvertToNormal ? 16 : 26,
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
                    // TODO: Bake Holds here.

                    MerWriterNote? lastNoteToReference = null;

                    // Loop from end to start to easily get "next" reference.
                    for (int j = holdNote.Points.Count - 1; j >= 0; j--)
                    {
                        HoldPointNote point = holdNote.Points[j];

                        MerWriterNote merWriterNote = new()
                        {
                            Timestamp = point.Timestamp,
                            Position = point.Position,
                            Size = point.Size,
                        };

                        // Hold End
                        if (j == holdNote.Points.Count - 1)
                        {
                            merWriterNote.NoteType = 11;
                            merWriterNote.Render = 1;
                        }

                        // Hold Start
                        else if (j == 0)
                        {
                            merWriterNote.NoteType = holdNote.BonusType switch
                            {
                                BonusType.Normal => 9,
                                BonusType.Bonus => args.ConvertExtendedBonusTypes == ConvertExtendedBonusTypesOption.ConvertToNormal ? 9 : 25,
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
                            merWriterNote.Render = (int)point.RenderType;
                            merWriterNote.Reference = lastNoteToReference;
                        }

                        notes.Add(merWriterNote);
                        lastNoteToReference = merWriterNote;
                    }
                }
            }
        }

        // Add all lane toggle notes.
        foreach (Note note in chart.LaneToggles)
        {
            if (note is LaneShowNote laneShowNote)
            {
                notes.Add(new()
                {
                    Timestamp = laneShowNote.Timestamp,
                    NoteType = 12,
                    Position = laneShowNote.Position,
                    Size = laneShowNote.Size,
                    Render = 1,
                    Direction = (int)laneShowNote.Direction,
                });

                continue;
            }

            if (note is LaneHideNote laneHideNote)
            {
                notes.Add(new()
                {
                    Timestamp = laneHideNote.Timestamp,
                    NoteType = 13,
                    Position = laneHideNote.Position,
                    Size = laneHideNote.Size,
                    Render = 1,
                    Direction = (int)laneHideNote.Direction,
                }); 
            }
        }
        
        // Add chart end.
        notes.Add(new()
        {
            Timestamp = entry.ChartEnd,
            NoteType = 14,
            Position = 0,
            Size = 60,
            Render = 1,
        });
        
        notes = notes
            .OrderBy(x => x.Timestamp)
            .ToList();

        for (int i = 0; i < notes.Count; i++)
        {
            MerWriterNote writerNote = notes[i];
            sb.Append($"{writerNote.Timestamp.Measure,4} {writerNote.Timestamp.Tick,4}    1 {writerNote.NoteType,4} {i,4} {writerNote.Position,4} {writerNote.Size,4} {writerNote.Render,4}");

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