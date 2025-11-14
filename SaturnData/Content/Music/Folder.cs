using System;
using System.Collections.Generic;
using SaturnData.Notation.Core;

namespace SaturnData.Content.Music;

/// <summary>
/// A list of styles for the scrolling background of folders.
/// </summary>
public enum FolderBackgroundStyle
{
    /// <summary>
    /// Black and gray checkerboard pattern.
    /// </summary>
    Checkers = 0,
    
    /// <summary>
    /// Light pink triangles on a hot pink background.
    /// </summary>
    Triangles = 1,
    
    /// <summary>
    /// Cyan circles on a yellow-green background.
    /// </summary>
    Circles = 2,
    
    /// <summary>
    /// Black sparkles on an orange background.
    /// </summary>
    Sparkles = 3,
    
    /// <summary>
    /// Light red arrows on a dark red background.
    /// </summary>
    Arrows = 4,
    
    /// <summary>
    /// Gray square mesh on a sky blue background.
    /// </summary>
    SquareMesh = 5,
    
    /// <summary>
    /// White triangle mesh on a deep blue background.
    /// </summary>
    TrianglesMesh = 6,
    
    /// <summary>
    /// Purple stripes on a hot pink background.
    /// </summary>
    Stripes = 7,
    
    /// <summary>
    /// White dots on a yellow-orange background.
    /// </summary>
    Dots = 8,
    
    /// <summary>
    /// White stars on a blue-cyan background.
    /// </summary>
    Stars = 9,
    
    /// <summary>
    /// Purple stripes on a hot pink background.
    /// </summary>
    Level = 10,
    
    /// <summary>
    /// Green triangles on a white-green background
    /// </summary>
    Name = 11,
}

/// <summary>
/// A list of character groups for sorting folders by text.
/// </summary>
public enum FolderCharacterGroup
{
    /// <summary>
    /// Hiragana, Katakana | Vowels | あ い う え お
    /// </summary>
    RowA = 0,
    
    /// <summary>
    /// Hiragana, Katakana | Ka-row | か き く け こ
    /// </summary>
    RowKa = 1,
    
    /// <summary>
    /// Hiragana, Katakana | Sa-row | さ し す せ そ
    /// </summary>
    RowSa = 2,
    
    /// <summary>
    /// Hiragana, Katakana | Ta-row | た ち つ て と
    /// </summary>
    RowTa = 3,
    
    /// <summary>
    /// Hiragana, Katakana | Na-row | な に ぬ ね の
    /// </summary>
    RowNa = 4,
    
    /// <summary>
    /// Hiragana, Katakana | Ha-row | は ひ ふ へ ほ
    /// </summary>
    RowHa = 5,
    
    /// <summary>
    /// Hiragana, Katakana | Ma-row | ま み む め も
    /// </summary>
    RowMa = 6,
    
    /// <summary>
    /// Hiragana, Katakana | Ya-row | や ゆ よ
    /// </summary>
    RowYa = 7,
    
    /// <summary>
    /// Hiragana, Katakana | Ra-row | ら り る れ ろ
    /// </summary>
    RowRa = 8,
    
    /// <summary>
    /// Hiragana, Katakana | Wa-row | わ を
    /// </summary>
    RowWa = 9,
    
    /// <summary>
    /// Numbers and special characters that aren't sorted into any other group.
    /// </summary>
    Symbol = 10,
    
    /// <summary>
    /// Latin alphabet | A B C D
    /// </summary>
    RangeAxD = 11,
    
    /// <summary>
    /// Latin alphabet | E F G H
    /// </summary>
    RangeExH = 12,
    
    /// <summary>
    /// Latin alphabet | I J K L
    /// </summary>
    RangeIxL = 13,
    
    /// <summary>
    /// Latin alphabet | M N O P
    /// </summary>
    RangeMxP = 14,
    
    /// <summary>
    /// Latin alphabet | Q R S T
    /// </summary>
    RangeQxT = 15,
    
    /// <summary>
    /// Latin alphabet | U V X Y Z
    /// </summary>
    RangeUxZ = 16,
}

/// <summary>
/// A collection of songs.
/// </summary>
[Serializable]
public class Folder : ContentItem
{
    /// <summary>
    /// The color of the folder.
    /// </summary>
    public uint Color { get; set; } = 0xFFFFFFFF;

    /// <summary>
    /// The background theme of the folder.
    /// </summary>
    public FolderBackgroundStyle Background { get; set; } = FolderBackgroundStyle.Checkers;

    /// <summary>
    /// The local filepath of the folder image, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public string ImagePath { get; set; } = "";

    /// <summary>
    /// The string to display on a small additional folder label.
    /// </summary>
    public string Label = "";

    /// <summary>
    /// Entries sorted by individual metadata.
    /// </summary>
    /// <remarks>
    /// Used for difficulty-independent grouping.
    /// </remarks>
    public List<Entry> Entries = [];
    
    /// <summary>
    /// Songs sorted by Normal difficulty metadata
    /// </summary>
    /// <remarks>
    /// Used for standard grouping, within each difficulty.
    /// </remarks>
    public List<Song> NormalSongs = [];
    
    /// <summary>
    /// Songs sorted by Hard difficulty metadata
    /// </summary>
    /// <remarks>
    /// Used for standard grouping, within each difficulty.
    /// </remarks>
    public List<Song> HardSongs = [];
    
    /// <summary>
    /// Songs sorted by Expert difficulty metadata
    /// </summary>
    /// <remarks>
    /// Used for standard grouping, within each difficulty.
    /// </remarks>
    public List<Song> ExpertSongs = [];
    
    /// <summary>
    /// Songs sorted by Inferno difficulty metadata
    /// </summary>
    /// <remarks>
    /// Used for standard grouping, within each difficulty.
    /// </remarks>
    public List<Song> InfernoSongs = [];
    
    /// <summary>
    /// Songs sorted by World's End difficulty metadata
    /// </summary>
    /// <remarks>
    /// Used for standard grouping, within each difficulty.
    /// </remarks>
    public List<Song> WorldsEndSongs = [];
    
    /// <summary>
    /// The absolute filepath of the folder image.
    /// </summary>
    public string AbsoluteImagePath => AbsolutePath(ImagePath);
    
    /// <summary>
    /// Returns a list of songs ordered by the metadata of the specified difficulty.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when difficulty equals <see cref="Difficulty.None"/></exception>
    public List<Song> SongsByDifficulty(Difficulty difficulty)
    {
        return difficulty switch
        {
            Difficulty.Normal => NormalSongs,
            Difficulty.Hard => HardSongs,
            Difficulty.Expert => ExpertSongs,
            Difficulty.Inferno => InfernoSongs,
            Difficulty.WorldsEnd => WorldsEndSongs,
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null),
        };
    }
}