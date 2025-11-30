using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kawazu;
using SaturnData.Content.Music;
using SaturnData.Notation.Serialization;

namespace SaturnData.Notation.Core;

/// <summary>
/// The "difficulty category" of a chart.
/// </summary>
/// <remarks>
/// <c>None</c> difficulty is not a valid difficulty for a chart. This enum value is only used as a failsafe in Saturn.
/// </remarks>
public enum Difficulty
{
    None = -1,
    Normal = 0,
    Hard = 1,
    Expert = 2,
    Inferno = 3,
    WorldsEnd = 4,
}

/// <summary>
/// The default background to display in Saturn when playing the chart.
/// </summary>
public enum BackgroundOption
{
    Auto = 0,
    Saturn = 1,
    Version3 = 2,
    Version2 = 3,
    Version1 = 4,
    Boss = 5,
    StageUp = 6,
    WorldsEnd = 7,
    Jacket = 8,
}

/// <summary>
/// Holds all metadata for a chart.
/// </summary>
public class Entry
{
    public event EventHandler? EntryChanged;
    public event EventHandler? JacketChanged;
    public event EventHandler? AudioChanged;
    public event EventHandler? VideoChanged;

    /// <summary>
    /// Does a chart file exist for this entry?
    /// </summary>
    public bool Exists => File.Exists(ChartPath);
    
    /// <summary>
    /// The folder containing this entry.
    /// </summary>
    public Folder? Folder = null;
    
    /// <summary>
    /// The song containing this entry.
    /// </summary>
    public Song? Song = null;



    /// <summary>
    /// The unique identifier of the chart.
    /// </summary>
    public string Id = NewId;

    /// <summary>
    /// The format version of the file that defined this entry.
    /// </summary>
    /// <remarks>
    /// If the entry was not created by deserializing a file,
    /// the default value should be kept instead.
    /// </remarks>
    public FormatVersion FormatVersion { get; set; } = FormatVersion.SatV3;

    /// <summary>
    /// The title of the chart's song.
    /// </summary>
    public string Title
    {
        get => title;
        set
        {
            if (title == value) return;
            
            title = value;
            EntryChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private string title = "";
    
    /// <summary>
    /// The kana reading of the title.
    /// </summary>
    public string Reading
    {
        get => reading;
        set
        {
            if (reading == value) return;
            
            reading = value;
            EntryChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private string reading = "";
    
    /// <summary>
    /// The artist of the chart's song.
    /// </summary>
    public string Artist
    {
        get => artist;
        set
        {
            if (artist == value) return;
            
            artist = value;
            EntryChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private string artist = "";
    
    /// <summary>
    /// The bpm text to display on the song select screen.
    /// </summary>
    public string BpmMessage
    {
        get => bpmMessage;
        set
        {
            if (bpmMessage == value) return;
            
            bpmMessage = value;
            EntryChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private string bpmMessage = "";

    
    
    /// <summary>
    /// The current revision of the chart. Optional and only used for users to keep track of different chart revisions.
    /// </summary>
    public string Revision
    {
        get => revision;
        set
        {
            if (revision == value) return;
            
            revision = value;
            EntryChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private string revision = "";
    
    /// <summary>
    /// The notes designer of the chart.
    /// </summary>
    public string NotesDesigner
    {
        get => notesDesigner;
        set
        {
            if (notesDesigner == value) return;
            
            notesDesigner = value;
            EntryChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private string notesDesigner = "";
    
    /// <summary>
    /// The difficulty category of the chart.
    /// </summary>
    public Difficulty Difficulty
    {
        get => difficulty;
        set
        {
            if (difficulty == value) return;
            
            difficulty = value;
            EntryChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private Difficulty difficulty = Difficulty.Normal;
    
    /// <summary>
    /// The level (or constant) of the chart.
    /// </summary>
    public double Level
    {
        get => level;
        set
        {
            if (level == value) return;
            
            level = value;
            EntryChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private double level = 0;

    /// <summary>
    /// The percentage of the clear bar that needs to be reached to clear a song.
    /// </summary>
    public float ClearThreshold
    {
        get => clearThreshold;
        set
        {
            if (clearThreshold == value) return;
            
            clearThreshold = value;
            EntryChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private float clearThreshold = 0.45f;
    
    
    
    /// <summary>
    /// The timestamp the chart preview starts at. Must be less than <see cref="PreviewEnd"/>.
    /// </summary>
    public Timestamp PreviewBegin
    {
        get => previewBegin;
        set
        {
            if (previewBegin == value) return;
            
            previewBegin = value;

            if (previewEnd.FullTick <= previewBegin.FullTick)
            {
                previewEnd = new(previewBegin.FullTick + 7680);
            }
            
            EntryChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private Timestamp previewBegin = new(Timestamp.Zero);
    
    /// <summary>
    /// The timestamp the chart preview ends at. Must be greater than <see cref="PreviewBegin"/>.
    /// </summary>
    public Timestamp PreviewEnd
    {
        get => previewEnd;
        set
        {
            if (previewEnd == value) return;
            
            previewEnd = value;

            if (previewBegin.FullTick >= previewEnd.FullTick)
            {
                previewBegin = new(previewEnd.FullTick - 7680);
            }
            
            EntryChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private Timestamp previewEnd = new(7680);
    
    internal float? PreviewBeginTime = null;
    internal float? PreviewLengthTime = null;

    /// <summary>
    /// The default background for the chart.
    /// </summary>
    public BackgroundOption Background
    {
        get => background;
        set
        {
            if (background == value) return;
            
            background = value;
            EntryChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private BackgroundOption background = BackgroundOption.Auto;



    /// <summary>
    /// The directory that all chart-related files are in.
    /// </summary>
    public string RootDirectory
    {
        get => rootDirectory;
        set
        {
            if (rootDirectory == value) return;

            rootDirectory = value;
            EntryChanged?.Invoke(null, EventArgs.Empty);
            
            // Invoke all media change events since all filepaths changed.
            JacketChanged?.Invoke(null, EventArgs.Empty);
            AudioChanged?.Invoke(null, EventArgs.Empty);
            VideoChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private string rootDirectory = "";
    
    /// <summary>
    /// File name of the chart file that defined this entry.
    /// </summary>
    public string ChartFile
    {
        get => chartFile;
        set
        {
            if (chartFile == value) return;
            
            chartFile = value;
            EntryChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private string chartFile = "";
    
    /// <summary>
    /// File name of the background audio file.
    /// </summary>
    public string AudioFile
    {
        get => audioFile;
        set
        {
            if (audioFile != value)
            {
                audioFile = value;
                EntryChanged?.Invoke(null, EventArgs.Empty);
            }
            
            // Always invoke AudioChanged when a filepath changes to allow for reloading the same file.
            AudioChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private string audioFile = "";

    /// <summary>
    /// File name of the background video file.
    /// </summary>
    public string VideoFile
    {
        get => videoFile;
        set
        {
            if (videoFile != value)
            {
                videoFile = value;
                EntryChanged?.Invoke(null, EventArgs.Empty);
            }

            // Always invoke VideoChanged when a filepath changes to allow for reloading the same file.
            VideoChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private string videoFile = "";

    /// <summary>
    /// File name of the jacket image file.
    /// </summary>
    public string JacketFile
    {
        get => jacketFile;
        set
        {
            if (jacketFile != value)
            {
                jacketFile = value;
                EntryChanged?.Invoke(null, EventArgs.Empty);
            }
            
            // Always invoke JacketChanged when a filepath changes to allow for reloading the same file.
            JacketChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private string jacketFile = "";
    
    /// <summary>
    /// Absolute file path to the chart file that defined this entry.
    /// </summary>
    public string ChartPath => Path.Combine(RootDirectory, ChartFile);

    /// <summary>
    /// Absolute file path to the background audio file.
    /// </summary>
    public string AudioPath => Path.Combine(RootDirectory, AudioFile);

    /// <summary>
    /// Absolute file path to the background video file.
    /// </summary>
    public string VideoPath => Path.Combine(RootDirectory, VideoFile);

    /// <summary>
    /// Absolute file path to the jacket image file.
    /// </summary>
    public string JacketPath => Path.Combine(RootDirectory, JacketFile);

    /// <summary>
    /// Audio offset <b>in milliseconds</b>
    /// </summary>
    public float AudioOffset
    {
        get => audioOffset;
        set
        {
            if (audioOffset == value) return;
            
            audioOffset = value;
            EntryChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private float audioOffset = 0;

    /// <summary>
    /// Video offset <b>in milliseconds</b>
    /// </summary>
    public float VideoOffset
    {
        get => videoOffset;
        set
        {
            if (videoOffset == value) return;
            
            videoOffset = value;
            EntryChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private float videoOffset = 0;

    /// <summary>
    /// The Chart End timestamp, where playback stops and the gameplay result is shown.
    /// </summary>
    public Timestamp ChartEnd
    {
        get => chartEnd;
        set
        {
            if (chartEnd == value) return;
            
            chartEnd = value;
            EntryChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private Timestamp chartEnd = Timestamp.Zero;
    
    
    
    /// <summary>
    /// Should this chart be shown in tutorial mode?
    /// </summary>
    public bool TutorialMode
    {
        get => tutorialMode;
        set
        {
            if (tutorialMode == value) return;
            
            tutorialMode = value;
            EntryChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private bool tutorialMode = false;
    
    /// <summary>
    /// Should the <see cref="Reading"/> be determined automatically?
    /// </summary>
    public bool AutoReading
    {
        get => autoReading;
        set
        {
            if (autoReading == value) return;
            
            autoReading = value;
            EntryChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private bool autoReading = false;
    
    /// <summary>
    /// Should the <see cref="BpmMessage"/> be determined automatically?
    /// </summary>
    public bool AutoBpmMessage
    {
        get => autoBpmMessage;
        set
        {
            if (autoBpmMessage == value) return;
            
            autoBpmMessage = value;
            EntryChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private bool autoBpmMessage = false;
    
    /// <summary>
    /// Should the <see cref="ClearThreshold"/> be determined automatically?
    /// </summary>
    public bool AutoClearThreshold
    {
        get => autoClearThreshold;
        set
        {
            if (autoClearThreshold == value) return;
            
            autoClearThreshold = value;
            EntryChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private bool autoClearThreshold = true;

    /// <summary>
    /// Should the <see cref="ChartEnd"/> be determined automatically?
    /// </summary>
    public bool AutoChartEnd
    {
        get => autoChartEnd;
        set
        {
            if (autoChartEnd == value) return;
            
            autoChartEnd = value;
            EntryChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private bool autoChartEnd = true;
    
    
    
    /// <summary>
    /// Does a file exist at <c>VideoPath</c>?
    /// </summary>
    public bool VideoExists => File.Exists(VideoFile);
    
    /// <summary>
    /// Returns the integer part of the <see cref="Level"/>, and adds a + if the decimal part is >= 0.7.<br/>
    /// Returns '?' if <see cref="Level"/> is below 0.
    /// Returns 'X' if chart file does not exist.
    /// </summary>
    /// <code>
    ///  12.6    -> "12"
    ///  12.7    -> "12+"
    ///  13.2    -> "13"
    /// -1.0     -> "?"
    ///  No File -> "X"
    /// </code>
    public string LevelString
    {
        get
        {
            if (!Exists) return "X";
            return RawLevelString;
        }
    }

    /// <summary>
    /// Returns the integer part of the <see cref="Level"/>, and adds a + if the decimal part is >= 0.7.<br/>
    /// Returns '?' if <see cref="Level"/> is below 0.
    /// </summary>
    /// <code>
    ///  12.6    -> "12"
    ///  12.7    -> "12+"
    ///  13.2    -> "13"
    /// -1.0     -> "?"
    /// </code>
    public string RawLevelString
    {
        get
        {
            if (Level < 0) return "?";
            
            return Math.Round(Level * 10) - (int)Level * 10 >= 7
                ? $"{(int)Level}+"
                : $"{(int)Level}";
        }
    }

    /// <summary>
    /// Returns the standard clear threshold for the specified difficulty.
    /// </summary>
    public static float GetAutoClearThreshold(Difficulty difficulty)
    {
        return difficulty switch
        {
            Difficulty.None => 0.45f,
            Difficulty.Normal => 0.45f,
            Difficulty.Hard => 0.55f,
            Difficulty.Expert => 0.8f,
            Difficulty.Inferno => 0.8f,
            Difficulty.WorldsEnd => 0.55f,
            _ => 0.8f,
        };
    }

    /// <summary>
    /// Returns the "reading" string for a specified title.
    /// </summary>
    public static async Task<string> GetAutoReading(string title)
    {
        string result = title;

        foreach (KeyValuePair<char, char> pair in FullWidthDict)
        {
            result = result.Replace(pair.Key, pair.Value);
        }

        KawazuConverter converter = new();
        result = await converter.Convert(result);

        result = new(result.Where(c => ValidCharacters.Contains(c)).ToArray());

        return result;
    }
    
    public static string NewId => $"SAT{Guid.NewGuid()}";
    
    private static readonly HashSet<char> ValidCharacters =
    [
        'あ', 'い', 'う', 'え', 'お',
        'か', 'き', 'く', 'け', 'こ',
        'さ', 'し', 'す', 'せ', 'そ',
        'た', 'ち', 'つ', 'て', 'と',
        'な', 'に', 'ぬ', 'ね', 'の',
        'は', 'ひ', 'ふ', 'へ', 'ほ',
        'ま', 'み', 'む', 'め', 'も',
        'や', 'ゆ', 'よ',
        'ら', 'り', 'る', 'れ', 'ろ',
        'わ', 'ゐ', 'ゑ', 'を',
        'が', 'ぎ', 'ぐ', 'げ', 'ご',
        'ざ', 'じ', 'ず', 'ぜ', 'ぞ',
        'だ', 'ぢ', 'づ', 'で', 'ど',
        'ば', 'び', 'ぶ', 'べ', 'ぼ',
        'ぱ', 'ぴ', 'ぷ', 'ぺ', 'ぽ',
        'ん','っ',
        
        'ぁ','ぃ','ぅ','ぇ','ぉ',
        'ゃ','ゅ','ょ',

        '１', '２', '３', '４', '５', '６', '７', '８', '９', '０',
        'Ａ', 'Ｂ', 'Ｃ', 'Ｄ', 'Ｅ', 'Ｆ', 'Ｇ', 'Ｈ', 'Ｉ', 'Ｊ', 'Ｋ', 'Ｌ', 'Ｍ', 'Ｎ', 'Ｏ', 'Ｐ', 'Ｑ', 'Ｒ', 'Ｓ', 'Ｔ', 'Ｕ', 'Ｖ', 'Ｗ', 'Ｘ', 'Ｙ', 'Ｚ',
    ];

    private static readonly Dictionary<char, char> FullWidthDict = new()
    {
        ['A'] = 'Ａ', ['a'] = 'Ａ',
        ['B'] = 'Ｂ', ['b'] = 'Ｂ',
        ['C'] = 'Ｃ', ['c'] = 'Ｃ',
        ['D'] = 'Ｄ', ['d'] = 'Ｄ',
        ['E'] = 'Ｅ', ['e'] = 'Ｅ',
        ['F'] = 'Ｆ', ['f'] = 'Ｆ',
        ['G'] = 'Ｇ', ['g'] = 'Ｇ',
        ['H'] = 'Ｈ', ['h'] = 'Ｈ',
        ['I'] = 'Ｉ', ['i'] = 'Ｉ',
        ['J'] = 'Ｊ', ['j'] = 'Ｊ',
        ['K'] = 'Ｋ', ['k'] = 'Ｋ',
        ['L'] = 'Ｌ', ['l'] = 'Ｌ',
        ['M'] = 'Ｍ', ['m'] = 'Ｍ',
        ['N'] = 'Ｎ', ['n'] = 'Ｎ',
        ['O'] = 'Ｏ', ['o'] = 'Ｏ',
        ['P'] = 'Ｐ', ['p'] = 'Ｐ',
        ['Q'] = 'Ｑ', ['q'] = 'Ｑ',
        ['R'] = 'Ｒ', ['r'] = 'Ｒ',
        ['S'] = 'Ｓ', ['s'] = 'Ｓ',
        ['T'] = 'Ｔ', ['t'] = 'Ｔ',
        ['U'] = 'Ｕ', ['u'] = 'Ｕ',
        ['V'] = 'Ｖ', ['v'] = 'Ｖ',
        ['W'] = 'Ｗ', ['w'] = 'Ｗ',
        ['X'] = 'Ｘ', ['x'] = 'Ｘ',
        ['Y'] = 'Ｙ', ['y'] = 'Ｙ',
        ['Z'] = 'Ｚ', ['z'] = 'Ｚ',
        
        ['1'] = '１', 
        ['2'] = '２', 
        ['3'] = '３', 
        ['4'] = '４', 
        ['5'] = '５', 
        ['6'] = '６', 
        ['7'] = '７', 
        ['8'] = '８', 
        ['9'] = '９', 
        ['0'] = '０',
    };
}