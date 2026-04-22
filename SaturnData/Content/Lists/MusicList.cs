using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SaturnData.Content.Items;
using SaturnData.Content.Music;
using SaturnData.Content.Serialization;
using SaturnData.Notation.Core;
using SaturnData.Notation.Serialization;
using SaturnData.Utilities;

namespace SaturnData.Content.Lists;

/// <summary>
/// The criteria to group content into folders by.
/// </summary>
public enum GroupType
{
    /// <summary>
    /// Song Grouping | Groups content by the song's parent directory.
    /// </summary>
    Directory = 0,
    
    /// <summary>
    /// Entry Grouping | Groups content by level.
    /// </summary>
    Level = 1,
    
    /// <summary>
    /// Song Grouping | Groups content by title.
    /// </summary>
    Title = 2,
    
    /// <summary>
    /// Song Grouping | Groups content by artist.
    /// </summary>
    Artist = 3,
    
    /// <summary>
    /// Entry Grouping | Groups content by notes designer.
    /// </summary>
    NotesDesigner = 4,
}

/// <summary>
/// The criteria to sort folder contents by.
/// </summary>
public enum SortType
{
    /// <summary>
    /// Sorts folder contents by their original order in the file system.
    /// </summary>
    Directory = 0,
    
    /// <summary>
    /// Sorts folder contents by title.
    /// </summary>
    Title = 1,
    
    /// <summary>
    /// Sorts folder contents by artist
    /// </summary>
    Artist = 2,
    
    /// <summary>
    /// Sorts folder contents by notes designer.
    /// </summary>
    NotesDesigner = 3,
    
    /// <summary>
    /// Sorts folder contents by tempo.
    /// </summary>
    Tempo = 4,
    
    /// <summary>
    /// Sorts folder contents by level.
    /// </summary>
    Level = 5,
}

/// <summary>
/// Contains all music content.
/// </summary>
public class MusicList
{
    public MusicList()
    {
        Mesh = new();
        Mesh.SelectionChanged += Mesh_OnSelectionChanged;
    }

    public event EventHandler? FolderSelectStateChanged;
    
    /// <summary>
    /// All folders, generated based on the ActiveFolderSortType and ActiveSongSortType.
    /// </summary>
    public readonly List<Folder> Folders = [];

    /// <summary>
    /// All songs marked as favorite.
    /// </summary>
    public List<Song> Favorites = [];
    
    /// <summary>
    /// The currently selected Folder.
    /// </summary>
    /// <remarks>
    /// This is used in SATURN to keep track of the selected Folder in a Folder-Select Scene without changing the selected MusicMesh Node.<br/>
    /// This field is updated every time the MusicMesh selection changes, but changing it directly has no effect on the MusicMesh selection. The update only goes one way.
    /// </remarks>
    public Folder? SelectedFolder
    {
        get => selectedFolder;
        set
        {
            if (selectedFolder == value) return;
            
            selectedFolder = value;
            FolderSelectStateChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private Folder? selectedFolder;

    /// <summary>
    /// The currently selected Difficulty.
    /// </summary>
    /// <remarks>
    /// This is used in SATURN to keep track of the selected Difficulty in a Folder-Select Scene without changing the selected MusicMesh Node.<br/>
    /// This field is updated every time the MusicMesh selection changes, but changing it directly has no effect on the MusicMesh selection. The update only goes one way.
    /// </remarks>
    public Difficulty? SelectedDifficulty
    {
        get => selectedDifficulty;
        set
        {
            if (selectedDifficulty == value) return;
            
            selectedDifficulty = value;
            FolderSelectStateChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private Difficulty? selectedDifficulty;
    
    /// <summary>
    /// All entries, listed by their Id.
    /// </summary>
    public readonly Dictionary<string, Entry> Entries = [];

    /// <summary>
    /// All entries, laid out in a cylinder-like mesh.
    /// </summary>
    public readonly MusicMesh Mesh;
    
    /// <summary>
    /// The criteria content is currently grouped into folders by.
    /// </summary>
    public GroupType ActiveGroupType 
    {
        get => activeGroupType; 
        set
        {
            if (activeGroupType == value) return;
            
            activeGroupType = value;
            Refresh();
        }
    }
    private GroupType activeGroupType = GroupType.Directory;
    
    /// <summary>
    /// The criteria folder contents are currently sorted by.
    /// </summary>
    public SortType ActiveSortType 
    {
        get => activeSortType; 
        set
        {
            if (activeSortType == value) return;
            
            activeSortType = value;
            Refresh();
        }
    }
    private SortType activeSortType = SortType.Directory;

    /// <summary>
    /// Should entries be grouped individually, or as songs?
    /// </summary>
    public bool UseEntryGrouping => ActiveGroupType is GroupType.Level or GroupType.NotesDesigner;

    /// <summary>
    /// Scans a directory and all of its subdirectories for music data, then loads it into memory.
    /// </summary>
    /// <param name="musicDirectoryPath">The absolute path of the directory to scan.</param>
    public void Load(string musicDirectoryPath)
    {
        try
        {
            Entries.Clear();

            // Nothing to load if the specified directory doesn't exist.
            if (!Directory.Exists(musicDirectoryPath)) return;
            
            // Go through all subdirectories.
            string[] subDirectories = Directory.GetDirectories(musicDirectoryPath, "*", SearchOption.AllDirectories);
            foreach (string directory in subDirectories)
            {
                // Go through all files in the directory.
                string[] files = Directory.EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly).ToArray();
                if (files.Length == 0) continue;
                
                // Try to load entries.
                List<Entry> entries = [];
                NotationReadArgs args = new();
                foreach (string file in files)
                {
                    ChartFormatVersion chartFormatVersion = NotationSerializer.DetectFormatVersion(file);
                    if (chartFormatVersion == ChartFormatVersion.Unknown) continue;
                    if (chartFormatVersion == ChartFormatVersion.Mer) continue;

                    try
                    {
                        Entry entry = NotationSerializer.ToEntry(file, args, out _);
                        
                        // Filter out entries without an Id.
                        if (string.IsNullOrWhiteSpace(entry.Id)) continue;
                        
                        entries.Add(entry);
                    }
                    catch
                    {
                        // ignored
                    }
                }
                
                if (entries.Count == 0) continue;
                
                // Build song from entries.
                Song song = new() { ParentDirectory = Directory.GetParent(directory)?.FullName ?? "" };
                foreach (Entry entry in entries)
                {
                    entry.Song = song;
                    
                    switch (entry.Difficulty)
                    {
                        case Difficulty.Normal:    { song.Normal    = entry; break; }
                        case Difficulty.Hard:      { song.Hard      = entry; break; }
                        case Difficulty.Expert:    { song.Expert    = entry; break; }
                        case Difficulty.Inferno:   { song.Inferno   = entry; break; }
                        case Difficulty.WorldsEnd: { song.WorldsEnd = entry; break; }
                        case Difficulty.None: break;
                    }
                }
                
                // Fill empty entries with content from existing entries.
                Entry? sourceEntry = null;
                if      (song.Normal    != null) { sourceEntry = song.Normal; }
                else if (song.Hard      != null) { sourceEntry = song.Hard; }
                else if (song.Expert    != null) { sourceEntry = song.Expert; }
                else if (song.Inferno   != null) { sourceEntry = song.Inferno; }
                else if (song.WorldsEnd != null) { sourceEntry = song.WorldsEnd; }

                if (sourceEntry != null && sourceEntry.Exists)
                {
                    song.Normal    ??= copyEntry(sourceEntry, Difficulty.Normal);
                    song.Hard      ??= copyEntry(sourceEntry, Difficulty.Hard);
                    song.Expert    ??= copyEntry(sourceEntry, Difficulty.Expert);
                    song.Inferno   ??= copyEntry(sourceEntry, Difficulty.Inferno);
                    song.WorldsEnd ??= copyEntry(sourceEntry, Difficulty.WorldsEnd);
                    
                    Entry copyEntry(Entry source, Difficulty difficulty)
                    {
                        return new()
                        {
                            Title = source.Title,
                            Reading = source.Reading,
                            Artist = source.Artist,
                            BpmMessage = source.BpmMessage,
                            Difficulty = difficulty,
                            RootDirectory = source.RootDirectory,
                            JacketFile = source.JacketFile,
                            AudioFile = source.AudioFile,
                            VideoFile = source.VideoFile,
                            Song = source.Song,
                        };
                    }
                }
                
                // Add final valid entries to entry list.
                if (song.Normal    != null) { Entries[song.Normal.Id]    = song.Normal;    }
                if (song.Hard      != null) { Entries[song.Hard.Id]      = song.Hard;      }
                if (song.Expert    != null) { Entries[song.Expert.Id]    = song.Expert;    }
                if (song.Inferno   != null) { Entries[song.Inferno.Id]   = song.Inferno;   }
                if (song.WorldsEnd != null) { Entries[song.WorldsEnd.Id] = song.WorldsEnd; }
            }

            Refresh();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    /// <summary>
    /// Re-does grouping, sorting, and remeshing without loading new content.
    /// </summary>
    public void Refresh()
    {
        Group();
        Sort();
        Remesh();
    }
    
    /// <summary>
    /// Groups all content based on the <see cref="ActiveGroupType"/>.
    /// </summary>
    protected virtual void Group()
    {
        Folders.Clear();

        switch (ActiveGroupType)
        {
            case GroupType.Directory:     { groupByDirectory();     break; }
            case GroupType.Level:         { groupByLevel();         break; }
            case GroupType.Title:         { groupByTitle();         break; }
            case GroupType.Artist:        { groupByArtist();        break; }
            case GroupType.NotesDesigner: { groupByNotesDesigner(); break; }
        }

        return;
        
        void groupByDirectory()
        {
            IEnumerable<IGrouping<string, Entry>> songsGroupedByDirectory = Entries.Values.GroupBy(entry => entry.Song?.ParentDirectory ?? "").OrderBy(grouping => grouping.Key);
            foreach (IGrouping<string, Entry> grouping in songsGroupedByDirectory)
            {
                if (grouping.Key == "") continue;
                
                try
                {
                    string folderDataPath = Path.Combine(grouping.Key, $"folder{SaturnFileExtensionList.SaturnContentFile}");
                    Folder folder;

                    try
                    {
                        ContentItem? contentItem = ContentSerializer.ToContentItem(folderDataPath);
                        if (contentItem is Folder f)
                        {
                            folder = f;
                            folder.AbsoluteSourcePath = folderDataPath;
                        }
                        else
                        {
                            folder = new()
                            {
                                Name = new DirectoryInfo(grouping.Key).Name,
                                Color = 0xFF808080,
                                ImagePath = "",
                                Label = "",
                            };
                        }
                    }
                    catch
                    {
                        folder = new()
                        {
                            Name = new DirectoryInfo(grouping.Key).Name,
                            Color = 0xFF808080,
                            ImagePath = "",
                            Label = "",
                        };
                    }
                    
                    HashSet<Song> addedSongs = [];
                    foreach (Entry entry in grouping.ToList())
                    {
                        if (entry.Song == null) continue;
                        
                        folder.Entries.Add(entry);
                        
                        if (!addedSongs.Add(entry.Song)) continue;
                        folder.NormalSongs.Add(entry.Song);
                        folder.HardSongs.Add(entry.Song);
                        folder.ExpertSongs.Add(entry.Song);
                        folder.InfernoSongs.Add(entry.Song);
                        folder.WorldsEndSongs.Add(entry.Song);
                    }

                    Folders.Add(folder);
                }
                catch
                {
                    // ignored
                }
            }

            Folder favorites = new()
            {
                Name = "select_folder_title_favorites",
                Color = 0xFF03FEDD,
                Background = FolderBackgroundStyle.Stars,
            };
            
            foreach (Song song in Favorites)
            {
                favorites.WorldsEndSongs.Add(song);
                favorites.InfernoSongs.Add(song);
                favorites.ExpertSongs.Add(song);
                favorites.HardSongs.Add(song);
                favorites.NormalSongs.Add(song);

                foreach (Entry? entry in song.Entries)
                {
                    if (entry == null) continue;
                    favorites.Entries.Add(entry);
                }
            }

            Folders.Add(favorites);
        }

        void groupByLevel()
        {
            IEnumerable<IGrouping<string, Entry>> entriesGroupedByLevel = Entries.Values.OrderBy(entry => entry.Level).GroupBy(entry => entry.LevelString);
            foreach (IGrouping<string, Entry> grouping in entriesGroupedByLevel)
            {
                try
                {
                    Folder folder = new()
                    {
                        Name = grouping.Key,
                        Label = grouping.Key,
                        ImagePath = "",
                        Color = 0xFF808080,
                        Background = FolderBackgroundStyle.Level,
                    };

                    if (grouping.Key == "X")
                    {
                        folder.Name = "select_folder_nochart";
                        folder.Background = FolderBackgroundStyle.Checkers;
                    }

                    foreach (Entry entry in grouping.ToList())
                    {
                        folder.Entries.Add(entry);
                    }

                    Folders.Add(folder);
                }
                catch
                {
                    // ignored
                }
            }
        }

        void groupByTitle()
        {
            IEnumerable<IGrouping<FolderCharacterGroup, Entry>> entriesGroupedByTitle = Entries.Values.GroupBy(entry => getCharacterGroup(string.IsNullOrEmpty(entry.Reading) ? entry.Title : entry.Reading));
            foreach (IGrouping<FolderCharacterGroup, Entry> grouping in entriesGroupedByTitle)
            {
                Folder folder = new()
                {
                    Name = getFolderNameFromGroup(grouping.Key),
                    Label = getFolderLabelFromGroup(grouping.Key),
                    ImagePath = "",
                    Color = 0xFF808080,
                    Background = FolderBackgroundStyle.Name,
                };
                
                if (grouping.Key == FolderCharacterGroup.Blank)
                {
                    folder.Name = "select_folder_title_blank";
                    folder.Label = "select_folder_label_blank";
                    folder.Background = FolderBackgroundStyle.Checkers;
                }

                foreach (Entry entry in grouping)
                {
                    if (entry.Song == null) continue;
                    
                    folder.Entries.Add(entry);

                    switch (entry.Difficulty)
                    {
                        case Difficulty.Normal:    { folder.NormalSongs.Add(entry.Song);    break; }
                        case Difficulty.Hard:      { folder.HardSongs.Add(entry.Song);      break; }
                        case Difficulty.Expert:    { folder.ExpertSongs.Add(entry.Song);    break; }
                        case Difficulty.Inferno:   { folder.InfernoSongs.Add(entry.Song);   break; }
                        case Difficulty.WorldsEnd: { folder.WorldsEndSongs.Add(entry.Song); break; }
                        default: continue;
                    }
                }

                Folders.Add(folder);
            }
        }
        
        void groupByArtist()
        {
            IEnumerable<IGrouping<FolderCharacterGroup, Entry>> entriesGroupedByTitle = Entries.Values.GroupBy(entry => getCharacterGroup(entry.Artist));
            foreach (IGrouping<FolderCharacterGroup, Entry> grouping in entriesGroupedByTitle)
            {
                Folder folder = new()
                {
                    Name = getFolderNameFromGroup(grouping.Key),
                    Label = getFolderLabelFromGroup(grouping.Key),
                    ImagePath = "",
                    Color = 0xFF808080,
                    Background = FolderBackgroundStyle.Name,
                };
                
                if (grouping.Key == FolderCharacterGroup.Blank)
                {
                    folder.Name = "select_folder_title_blank";
                    folder.Label = "select_folder_label_blank";
                    folder.Background = FolderBackgroundStyle.Checkers;
                }

                foreach (Entry entry in grouping)
                {
                    if (entry.Song == null) continue;
                    
                    folder.Entries.Add(entry);

                    switch (entry.Difficulty)
                    {
                        case Difficulty.Normal:    { folder.NormalSongs.Add(entry.Song);    break; }
                        case Difficulty.Hard:      { folder.HardSongs.Add(entry.Song);      break; }
                        case Difficulty.Expert:    { folder.ExpertSongs.Add(entry.Song);    break; }
                        case Difficulty.Inferno:   { folder.InfernoSongs.Add(entry.Song);   break; }
                        case Difficulty.WorldsEnd: { folder.WorldsEndSongs.Add(entry.Song); break; }
                        default: continue;
                    }
                }
                
                Folders.Add(folder);
            }
        }

        void groupByNotesDesigner()
        {
            IEnumerable<IGrouping<FolderCharacterGroup, Entry>> entriesGroupedByTitle = Entries.Values.GroupBy(entry => getCharacterGroup(entry.NotesDesigner));
            foreach (IGrouping<FolderCharacterGroup, Entry> grouping in entriesGroupedByTitle)
            {
                Folder folder = new()
                {
                    Name = getFolderNameFromGroup(grouping.Key),
                    Label = getFolderLabelFromGroup(grouping.Key),
                    ImagePath = "",
                    Color = 0xFF808080,
                    Background = FolderBackgroundStyle.Name,
                };

                if (grouping.Key == FolderCharacterGroup.Blank)
                {
                    folder.Name = "select_folder_title_blank";
                    folder.Label = "select_folder_label_blank";
                    folder.Background = FolderBackgroundStyle.Checkers;
                }

                foreach (Entry entry in grouping)
                {
                    if (entry.Song == null) continue;
                    
                    folder.Entries.Add(entry);

                    switch (entry.Difficulty)
                    {
                        case Difficulty.Normal:    { folder.NormalSongs.Add(entry.Song);    break; }
                        case Difficulty.Hard:      { folder.HardSongs.Add(entry.Song);      break; }
                        case Difficulty.Expert:    { folder.ExpertSongs.Add(entry.Song);    break; }
                        case Difficulty.Inferno:   { folder.InfernoSongs.Add(entry.Song);   break; }
                        case Difficulty.WorldsEnd: { folder.WorldsEndSongs.Add(entry.Song); break; }
                        default: continue;
                    }
                }
                
                Folders.Add(folder);
            }
        }
        
        
        FolderCharacterGroup getCharacterGroup(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return FolderCharacterGroup.Blank;

            // Normalize Input
            input = input.Normalize(NormalizationForm.FormKC);

            char c = input[0];

            // Convert Katakana to Hiragana
            if (c >= 'ァ' && c <= 'ン')
            {
                c = (char)(c - 'ァ' + 'ぁ');
            }

            // Get Hiragana Rows
            if (c >= 'あ' && c <= 'ん')
            {
                if ("あいうえお".Contains(c)) return FolderCharacterGroup.RowA;
                if ("かきくけこがぎぐげご".Contains(c)) return FolderCharacterGroup.RowKa;
                if ("さしすせそざじずぜぞ".Contains(c)) return FolderCharacterGroup.RowSa;
                if ("たちつてとだぢづでど".Contains(c)) return FolderCharacterGroup.RowTa;
                if ("なにぬねの".Contains(c)) return FolderCharacterGroup.RowNa;
                if ("はひふへほばびぶべぼぱぴぷぺぽ".Contains(c)) return FolderCharacterGroup.RowHa;
                if ("まみむめも".Contains(c)) return FolderCharacterGroup.RowMa;
                if ("やゆよ".Contains(c)) return FolderCharacterGroup.RowYa;
                if ("らりるれろ".Contains(c)) return FolderCharacterGroup.RowRa;
                if ("わをん".Contains(c)) return FolderCharacterGroup.RowWa;
            }

            // Get Latin Ranges
            c = char.ToLowerInvariant(c);

            if (c >= 'a' && c <= 'd') return FolderCharacterGroup.RangeAxD;
            if (c >= 'e' && c <= 'h') return FolderCharacterGroup.RangeExH;
            if (c >= 'i' && c <= 'l') return FolderCharacterGroup.RangeIxL;
            if (c >= 'm' && c <= 'p') return FolderCharacterGroup.RangeMxP;
            if (c >= 'q' && c <= 't') return FolderCharacterGroup.RangeQxT;
            if (c >= 'u' && c <= 'z') return FolderCharacterGroup.RangeUxZ;

            return FolderCharacterGroup.Symbol;
        }

        string getFolderNameFromGroup(FolderCharacterGroup group)
        {
            return group switch
            {
                FolderCharacterGroup.RowA => "select_folder_title_row_a",
                FolderCharacterGroup.RowKa => "select_folder_title_row_ka",
                FolderCharacterGroup.RowSa => "select_folder_title_row_sa",
                FolderCharacterGroup.RowTa => "select_folder_title_row_ta",
                FolderCharacterGroup.RowNa => "select_folder_title_row_na",
                FolderCharacterGroup.RowHa => "select_folder_title_row_ha",
                FolderCharacterGroup.RowMa => "select_folder_title_row_ma",
                FolderCharacterGroup.RowYa => "select_folder_title_row_ya",
                FolderCharacterGroup.RowRa => "select_folder_title_row_ra",
                FolderCharacterGroup.RowWa => "select_folder_title_row_wa",
                FolderCharacterGroup.RangeAxD => "select_folder_title_range_a-d",
                FolderCharacterGroup.RangeExH => "select_folder_title_range_e-h",
                FolderCharacterGroup.RangeIxL => "select_folder_title_range_i-l",
                FolderCharacterGroup.RangeMxP => "select_folder_title_range_m-p",
                FolderCharacterGroup.RangeQxT => "select_folder_title_range_q-t",
                FolderCharacterGroup.RangeUxZ => "select_folder_title_range_u-z",
                FolderCharacterGroup.Symbol => "select_folder_title_symbol",
                _ => "select_folder_title_symbol",
            };
        }
        
        string getFolderLabelFromGroup(FolderCharacterGroup group)
        {
            return group switch
            {
                FolderCharacterGroup.RowA => "select_folder_label_row_a",
                FolderCharacterGroup.RowKa => "select_folder_label_row_ka",
                FolderCharacterGroup.RowSa => "select_folder_label_row_sa",
                FolderCharacterGroup.RowTa => "select_folder_label_row_ta",
                FolderCharacterGroup.RowNa => "select_folder_label_row_na",
                FolderCharacterGroup.RowHa => "select_folder_label_row_ha",
                FolderCharacterGroup.RowMa => "select_folder_label_row_ma",
                FolderCharacterGroup.RowYa => "select_folder_label_row_ya",
                FolderCharacterGroup.RowRa => "select_folder_label_row_ra",
                FolderCharacterGroup.RowWa => "select_folder_label_row_wa",
                FolderCharacterGroup.RangeAxD => "select_folder_label_range_a-d",
                FolderCharacterGroup.RangeExH => "select_folder_label_range_e-h",
                FolderCharacterGroup.RangeIxL => "select_folder_label_range_i-l",
                FolderCharacterGroup.RangeMxP => "select_folder_label_range_m-p",
                FolderCharacterGroup.RangeQxT => "select_folder_label_range_q-t",
                FolderCharacterGroup.RangeUxZ => "select_folder_label_range_u-z",
                FolderCharacterGroup.Symbol => "select_folder_label_symbol",
                _ => "select_folder_label_symbol",
            };
        }
    }

    /// <summary>
    /// Sorts all content based on the <see cref="ActiveSortType"/>.
    /// </summary>
    private void Sort()
    {
        switch (ActiveSortType)
        {
            case SortType.Directory:     { sortByDirectory();     break; }
            case SortType.Title:         { sortByTitle();         break; }
            case SortType.Artist:        { sortByArtist();        break; }
            case SortType.NotesDesigner: { sortByNotesDesigner(); break; }
            case SortType.Tempo:         { sortByTempo();         break; }
            case SortType.Level:         { sortByLevel();         break; }
            default: return;
        }

        return;

        void sortByDirectory()
        {
            foreach (Folder folder in Folders)
            {
                folder.Entries = folder.Entries.OrderBy(entry => entry.RootDirectory).ToList();
                
                folder.NormalSongs    = folder.NormalSongs.OrderBy(song    => song.Normal?.RootDirectory).ToList();
                folder.HardSongs      = folder.HardSongs.OrderBy(song      => song.Hard?.RootDirectory).ToList();
                folder.ExpertSongs    = folder.ExpertSongs.OrderBy(song    => song.Expert?.RootDirectory).ToList();
                folder.InfernoSongs   = folder.InfernoSongs.OrderBy(song   => song.Inferno?.RootDirectory).ToList();
                folder.WorldsEndSongs = folder.WorldsEndSongs.OrderBy(song => song.WorldsEnd?.RootDirectory).ToList();
            }
        }

        void sortByTitle()
        {
            foreach (Folder folder in Folders)
            {
                folder.Entries = folder.Entries.OrderBy(entry => readingOrDefault(entry.Reading, entry.Title)).ToList();
                
                folder.NormalSongs    = folder.NormalSongs.OrderBy(song    => readingOrDefault(song.Normal?.Reading,    song.Normal?.Title)).ToList();
                folder.HardSongs      = folder.HardSongs.OrderBy(song      => readingOrDefault(song.Hard?.Reading,      song.Hard?.Title)).ToList();
                folder.ExpertSongs    = folder.ExpertSongs.OrderBy(song    => readingOrDefault(song.Expert?.Reading,    song.Expert?.Title)).ToList();
                folder.InfernoSongs   = folder.InfernoSongs.OrderBy(song   => readingOrDefault(song.Inferno?.Reading,   song.Inferno?.Title)).ToList();
                folder.WorldsEndSongs = folder.WorldsEndSongs.OrderBy(song => readingOrDefault(song.WorldsEnd?.Reading, song.WorldsEnd?.Title)).ToList();
                continue;
                
                string readingOrDefault(string? reading, string? title) => (string.IsNullOrEmpty(reading) ? title : reading) ?? "";
            }
        }

        void sortByArtist()
        {
            foreach (Folder folder in Folders)
            {
                folder.Entries = folder.Entries.OrderBy(entry => entry.Artist).ToList();
                
                folder.NormalSongs    = folder.NormalSongs.OrderBy(song    => song.Normal?.Artist).ToList();
                folder.HardSongs      = folder.HardSongs.OrderBy(song      => song.Hard?.Artist).ToList();
                folder.ExpertSongs    = folder.ExpertSongs.OrderBy(song    => song.Expert?.Artist).ToList();
                folder.InfernoSongs   = folder.InfernoSongs.OrderBy(song   => song.Inferno?.Artist).ToList();
                folder.WorldsEndSongs = folder.WorldsEndSongs.OrderBy(song => song.WorldsEnd?.Artist).ToList();
            }
        }

        void sortByNotesDesigner()
        {
            foreach (Folder folder in Folders)
            {
                folder.Entries = folder.Entries.OrderBy(entry => entry.NotesDesigner).ToList();
                
                folder.NormalSongs    = folder.NormalSongs.OrderBy(song    => song.Normal?.NotesDesigner).ToList();
                folder.HardSongs      = folder.HardSongs.OrderBy(song      => song.Hard?.NotesDesigner).ToList();
                folder.ExpertSongs    = folder.ExpertSongs.OrderBy(song    => song.Expert?.NotesDesigner).ToList();
                folder.InfernoSongs   = folder.InfernoSongs.OrderBy(song   => song.Inferno?.NotesDesigner).ToList();
                folder.WorldsEndSongs = folder.WorldsEndSongs.OrderBy(song => song.WorldsEnd?.NotesDesigner).ToList();
            }
        }

        void sortByTempo()
        {
            foreach (Folder folder in Folders)
            {
                folder.Entries = folder.Entries.OrderBy(entry => tempo(entry.BpmMessage)).ToList();
                
                folder.NormalSongs    = folder.NormalSongs.OrderBy(song    => tempo(song.Normal?.BpmMessage)).ToList();
                folder.HardSongs      = folder.HardSongs.OrderBy(song      => tempo(song.Hard?.BpmMessage)).ToList();
                folder.ExpertSongs    = folder.ExpertSongs.OrderBy(song    => tempo(song.Expert?.BpmMessage)).ToList();
                folder.InfernoSongs   = folder.InfernoSongs.OrderBy(song   => tempo(song.Inferno?.BpmMessage)).ToList();
                folder.WorldsEndSongs = folder.WorldsEndSongs.OrderBy(song => tempo(song.WorldsEnd?.BpmMessage)).ToList();
                continue;

                float tempo(string? bpmMessage)
                {
                    if (bpmMessage == null) return -1;
                    
                    try
                    {
                        MatchCollection matches = Regex.Matches(bpmMessage, @"\d+[,\.]?\d*");

                        float value = -1;
                        foreach (Match match in matches)
                        {
                            try
                            {
                                value = Math.Max(value, Convert.ToSingle(match.Value, CultureInfo.InvariantCulture));
                            }
                            catch
                            {
                                // Don't throw.
                            }
                        }
                        
                        return value;
                    }
                    catch
                    {
                        return -1;
                    }
                }
            }
        }

        void sortByLevel()
        {
            foreach (Folder folder in Folders)
            {
                folder.Entries = folder.Entries.OrderBy(entry => entry.Level).ToList();
                
                folder.NormalSongs    = folder.NormalSongs.OrderBy(song    => song.Normal?.Level).ToList();
                folder.HardSongs      = folder.HardSongs.OrderBy(song      => song.Hard?.Level).ToList();
                folder.ExpertSongs    = folder.ExpertSongs.OrderBy(song    => song.Expert?.Level).ToList();
                folder.InfernoSongs   = folder.InfernoSongs.OrderBy(song   => song.Inferno?.Level).ToList();
                folder.WorldsEndSongs = folder.WorldsEndSongs.OrderBy(song => song.WorldsEnd?.Level).ToList();
            }
        }
    }   

    /// <summary>
    /// Rebuilds the <see cref="MusicMesh"/> for UI navigation.
    /// </summary>
    private void Remesh()
    {
        // Save current selection
        Folder? savedFolder = Mesh.SelectedNode?.Folder;
        Entry? savedEntry = Mesh.SelectedNode?.Entry;
        
        if (UseEntryGrouping)
        {
            remeshEntryGrouping();
        }
        else
        {
            if (ActiveGroupType == GroupType.Directory)
            {
                remeshSongGroupingWithFavorites();
            }
            else
            {
                remeshSongGrouping();
            }
        }
        
        // Reapply current selection
        Mesh.Select(savedFolder, savedEntry);
        
        return;

        void remeshEntryGrouping()
        {
            Dictionary<Entry, MusicMeshNode> nodes = [];
            
            foreach (Folder folder in Folders)
            foreach (Entry entry in folder.Entries)
            {
                nodes.Add(entry, new() { Folder = folder, Entry = entry });
            }
            
            // Link together nodes within a row. [Left | Right]
            linkNodesInRow(nodes.Values.ToList());

            // Create "jumps" from one node to another. [Up | Down]
            foreach (MusicMeshNode node in nodes.Values)
            {
                linkNodesAcrossRow(nodes, node);
            }

            Mesh.Nodes = nodes.Values.ToList();
        }

        void remeshSongGrouping()
        {
            Dictionary<Entry, MusicMeshNode> nodes = [];
            foreach (Folder folder in Folders)
            foreach (Entry entry in folder.Entries)
            {
                nodes[entry] = new() { Folder = folder, Entry = entry };
            }
            
            List<MusicMeshNode> normalNodes = [];
            List<MusicMeshNode> hardNodes = [];
            List<MusicMeshNode> expertNodes = [];
            List<MusicMeshNode> infernoNodes = [];
            List<MusicMeshNode> worldsEndNodes = [];

            foreach (Folder folder in Folders)
            {
                createNodeRow(folder.NormalSongs, Difficulty.Normal, normalNodes);
                createNodeRow(folder.HardSongs, Difficulty.Hard, hardNodes);
                createNodeRow(folder.ExpertSongs, Difficulty.Expert, expertNodes);
                createNodeRow(folder.InfernoSongs, Difficulty.Inferno, infernoNodes);
                createNodeRow(folder.WorldsEndSongs, Difficulty.WorldsEnd, worldsEndNodes);

                continue;

                void createNodeRow(IEnumerable<Song> songs, Difficulty difficulty, List<MusicMeshNode> row)
                {
                    foreach (Song song in songs)
                    {
                        Entry? entry = song.EntryByDifficulty(difficulty);
                        if (entry == null) continue;

                        row.Add(nodes[entry]);
                    }
                }
            }

            // Link together nodes within a row [Left | Right]
            linkNodesInRow(normalNodes);
            linkNodesInRow(hardNodes);
            linkNodesInRow(expertNodes);
            linkNodesInRow(infernoNodes);
            linkNodesInRow(worldsEndNodes);

            // Link together nodes across rows [Up | Down]
            foreach (MusicMeshNode node in normalNodes)
            {
                linkNodesAcrossRow(nodes, node);
            }
            
            Mesh.Nodes = nodes.Values.ToList();
        }
        
        void remeshSongGroupingWithFavorites()
        {
            Dictionary<(Folder, Song), MusicMeshNodeGroup> groups = [];
            
            foreach (Folder folder in Folders)
            foreach (Song song in folder.NormalSongs)
            {
                MusicMeshNodeGroup group = new()
                {
                    Normal = new() { Folder = folder, Entry = song.Normal },
                    Hard = new() { Folder = folder, Entry = song.Hard },
                    Expert = new() { Folder = folder, Entry = song.Expert },
                    Inferno = new() { Folder = folder, Entry = song.Inferno },
                    WorldsEnd = new() { Folder = folder, Entry = song.WorldsEnd },
                };

                group.WorldsEnd.Bottom = group.Inferno;

                group.Inferno.Top = group.WorldsEnd;
                group.Inferno.Bottom = group.Expert;
                
                group.Expert.Top = group.Inferno;
                group.Expert.Bottom = group.Hard;
                
                group.Hard.Top = group.Expert;
                group.Hard.Bottom = group.Normal;
                
                group.Normal.Top = group.Hard;

                groups[(folder, song)] = group;
            }
            
            List<MusicMeshNode> nodes = [];
            List<MusicMeshNode> normalRow = [];
            List<MusicMeshNode> hardRow = [];
            List<MusicMeshNode> expertRow = [];
            List<MusicMeshNode> infernoRow = [];
            List<MusicMeshNode> worldsEndRow = [];

            foreach (Folder folder in Folders)
            {
                createNodeRow(folder.NormalSongs, Difficulty.Normal, normalRow);
                createNodeRow(folder.HardSongs, Difficulty.Hard, hardRow);
                createNodeRow(folder.ExpertSongs, Difficulty.Expert, expertRow);
                createNodeRow(folder.InfernoSongs, Difficulty.Inferno, infernoRow);
                createNodeRow(folder.WorldsEndSongs, Difficulty.WorldsEnd, worldsEndRow);
                continue;

                void createNodeRow(IEnumerable<Song> songs, Difficulty difficulty, List<MusicMeshNode> row)
                {
                    foreach (Song song in songs)
                    {
                        MusicMeshNodeGroup group = groups[(folder, song)];
                        MusicMeshNode node = difficulty switch
                        {
                            Difficulty.Normal => group.Normal,
                            Difficulty.Hard => group.Hard,
                            Difficulty.Expert => group.Expert,
                            Difficulty.Inferno => group.Inferno,
                            Difficulty.WorldsEnd => group.WorldsEnd,
                            _ => throw new ArgumentOutOfRangeException(),
                        };

                        row.Add(node);
                        nodes.Add(node);
                    }
                }
            }
            
            linkNodesInRow(normalRow);
            linkNodesInRow(hardRow);
            linkNodesInRow(expertRow);
            linkNodesInRow(infernoRow);
            linkNodesInRow(worldsEndRow);
            
            Mesh.Nodes = nodes;
        }
        
        void linkNodesInRow(List<MusicMeshNode> nodesToLink)
        {
            for (int i = 0; i < nodesToLink.Count; i++)
            {
                MusicMeshNode node = nodesToLink[i];
                MusicMeshNode left = i == 0 ? nodesToLink[^1] : nodesToLink[i - 1];
                
                node.Left = left;
                left.Right = node;
            }
        }
        
        void linkNodesAcrossRow(Dictionary<Entry, MusicMeshNode> nodes, MusicMeshNode node)
        {
            try
            {
                // There *shouldn't* be anything in here that's null at this point.
                // Still, try-catch just in case.

                Song song = node.Entry!.Song!;

                MusicMeshNode normalNode = nodes[song.Normal!];
                MusicMeshNode hardNode = nodes[song.Hard!];
                MusicMeshNode expertNode = nodes[song.Expert!];
                MusicMeshNode infernoNode = nodes[song.Inferno!];
                MusicMeshNode worldsEndNode = nodes[song.WorldsEnd!];

                normalNode.Top = hardNode;

                hardNode.Bottom = normalNode;
                hardNode.Top = expertNode;

                expertNode.Bottom = hardNode;
                expertNode.Top = infernoNode;

                infernoNode.Bottom = expertNode;
                infernoNode.Top = worldsEndNode;

                worldsEndNode.Bottom = infernoNode;
            }
            catch
            {
                // ignored
            }
        }
    }

    private void Mesh_OnSelectionChanged(object? sender, EventArgs e)
    {
        selectedFolder = Mesh.SelectedNode?.Folder;
        selectedDifficulty = Mesh.SelectedNode?.Entry?.Difficulty ?? Difficulty.None;
        
        FolderSelectStateChanged?.Invoke(null, EventArgs.Empty);
    }
}