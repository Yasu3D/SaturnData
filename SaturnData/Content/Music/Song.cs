using System;
using System.Collections.Generic;
using SaturnData.Notation.Core;

namespace SaturnData.Content.Music;

/// <summary>
/// A set of five <see cref="Entry">Entries</see>
/// </summary>
public class Song
{
    /// <summary>
    /// The absolute path of the directory containing the files that defined this song.
    /// </summary>
    public string ParentDirectory = "";
    
    /// <summary>
    /// The Normal difficulty of a song.
    /// </summary>
    public Entry? Normal = null;

    /// <summary>
    /// The Hard difficulty of a song.
    /// </summary>
    public Entry? Hard = null;

    /// <summary>
    /// The Expert difficulty of a song.
    /// </summary>
    public Entry? Expert = null;

    /// <summary>
    /// The Inferno difficulty of a song.
    /// </summary>
    public Entry? Inferno = null;

    /// <summary>
    /// The World's End difficulty of a song.
    /// </summary>
    public Entry? WorldsEnd = null;

    /// <summary>
    /// Returns a song's entry by its difficulty.
    /// </summary>
    /// <param name="difficulty">The difficulty to look for.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when difficulty equals <see cref="Difficulty.None"/></exception>
    public Entry? EntryByDifficulty(Difficulty difficulty)
    {
        return difficulty switch
        {
            Difficulty.Normal => Normal,
            Difficulty.Hard => Hard,
            Difficulty.Expert => Expert,
            Difficulty.Inferno => Inferno,
            Difficulty.WorldsEnd => WorldsEnd,
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null),
        };
    }

    /// <summary>
    /// Returns a song's entry by its Id, or <c>null</c> if no entry has the Id.
    /// </summary>
    /// <param name="id">The Id to look for.</param>
    public Entry? EntryById(string id)
    {
        if (Normal?.Id == id) return Normal;
        if (Hard?.Id == id) return Hard;
        if (Expert?.Id == id) return Expert;
        if (Inferno?.Id == id) return Inferno;
        if (WorldsEnd?.Id == id) return WorldsEnd;

        return null;
    }
    
    /// <summary>
    /// Returns all difficulties as an IEnumerable.
    /// </summary>
    public IEnumerable<Entry?> Entries => [Normal, Hard, Expert, Inferno, WorldsEnd];
}