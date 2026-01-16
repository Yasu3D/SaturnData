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
    /// A list of all items.
    /// </summary>
    public List<T> Items { get; private set; } = [];

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
            List<string> subDirectories = Directory.GetDirectories(contentDirectoryPath, "*", SearchOption.AllDirectories).ToList();
            subDirectories.Add(contentDirectoryPath);
            
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

    /// <summary>
    /// Returns the first 
    /// </summary>
    /// <param name="id">The Id of the <see cref="ContentItem"/> to find.</param>
    public T? TryGetItem(string id) => Items.FirstOrDefault(x => x.Id == id);
}