using System.IO;

namespace SaturnData.Content.Items;

/// <summary>
/// Defines the base data for a piece of user-defined content.
/// </summary>
public abstract class ContentItem
{
    /// <summary>
    /// The string used to identify a <see cref="ContentItem"/>.<br/>
    /// </summary>
    /// <remarks>
    /// It's recommended to use a universally unique identifier (UUID) for the Id,
    /// but the value doesn't have to conform to any standard.
    /// </remarks>
    public string Id { get; set; } = "";
    
    /// <summary>
    /// The name for a <see cref="ContentItem"/> to display to users.
    /// </summary>
    public string Name { get; set; }  = "";
    
    /// <summary>
    /// The absolute path to the file that defined the <see cref="ContentItem"/>.
    /// </summary>
    public string AbsoluteSourcePath = "";

    /// <summary>
    /// Returns the absolute filepath of a file at the specified local path.
    /// </summary>
    /// <param name="localPath"></param>
    protected string AbsolutePath(string localPath)
    {
        if (localPath == "") return "";
        
        string directory = Path.GetDirectoryName(AbsoluteSourcePath) ?? "";
        return Path.Combine(directory, localPath);
    }
}