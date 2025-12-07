using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SaturnData.Content.Items;
using Tomlyn;

namespace SaturnData.Content.Lists;

public class ContentList<T> where T : ContentItem, new()
{
    /// <summary>
    /// Invoked whenever the selected item changes.
    /// </summary>
    public event EventHandler? SelectionChanged;
    
    /// <summary>
    /// A list of all items.
    /// </summary>
    public List<T> Items { get; private set; } = [];

    /// <summary>
    /// The currently selected item.
    /// </summary>
    public T? SelectedItem
    {
        get => selectedItem;
        set
        {
            if (selectedItem == value) return;
            selectedItem = value;
            
            SelectionChanged?.Invoke(null, EventArgs.Empty);
        }
    }
    private T? selectedItem = null;

    /// <summary>
    /// Scans a directory and all of its subdirectories for content data, then loads it into memory.
    /// </summary>
    /// <param name="contentDirectoryPath">The absolute path of the directory to scan.</param>
    public void Load(string contentDirectoryPath)
    {
        try
        {
            Items.Clear();

            // Nothing to load if the specified directory doesn't exist.
            if (!Directory.Exists(contentDirectoryPath)) return;
            
            // Go through all subdirectories.
            string[] subDirectories = Directory.GetDirectories(contentDirectoryPath, "*", SearchOption.AllDirectories);
            foreach (string directory in subDirectories)
            {
                // Go through all files in the directory.
                string[] files = Directory.EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly).ToArray();
                if (files.Length == 0) continue;
                
                // Try to load items.
                foreach (string file in files)
                {
                    try
                    {
                        string data = File.ReadAllText(file);
                        
                        T item = Toml.ToModel<T>(data);
                        item.AbsoluteSourcePath = file;
                        
                        Items.Add(item);
                    }
                    catch
                    {
                        // ignored.
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}