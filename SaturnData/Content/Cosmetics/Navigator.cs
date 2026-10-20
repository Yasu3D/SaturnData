using System.Collections.Generic;
using SaturnData.Content.Cosmetics.NavigatorData;
using SaturnData.Content.Items;
using SaturnData.Content.Items.Attributes;

namespace SaturnData.Content.Cosmetics;

public enum NavigatorExpression
{
    Neutral = 0,
    Amazed = 1,
    Troubled = 2,
    Surprised = 3,
    Startled = 4,
    Angry = 5,
    Laughing = 6,
    Smiling = 7,
    Grinning = 8,
}

public enum NavigatorBlinkState
{
    Open = 0,
    HalfClose = 1,
    Closed = 2,
}

/// <summary>
/// A character that accompanies the user throughout gameplay.
/// </summary>
public class Navigator : CosmeticItem
{
    /// <summary>
    /// A string listing all contributors of a <see cref="Navigator"/>.
    /// </summary>
    public override string Copyright
    {
        get
        {
            if (Author == "" && Artist == "" && Voice == "") return "";

            string author = Author == "" ? "" : $"{Author}{(Artist != "" || Voice != "" ? ", " : "")}";
            string artist = Artist == "" ? "" : $"{Artist}{(Voice != "" ? ", " : "")}";

            return $"(c) {author}{artist}{Voice}";
        }
    }
    
    /// <summary>
    /// The creator of the <see cref="Navigator"/> artwork.
    /// </summary>
    [SctLineBreak]
    [SctKey("ARTIST", 14)]
    public string Artist { get; set; } = "";

    /// <summary>
    /// The voice actor of the <see cref="Navigator"/> voice lines.
    /// </summary>
    [SctKey("VOICE", 14)]
    public string Voice { get; set; } = "";

    /// <summary>
    /// The width and height of the in-game base UI element in pixels.
    /// </summary>
    [SctKey("SIZE", 14)]
    public NavigatorSize Size { get; set; } = new(1024, 1024);

    /// <summary>
    /// The position offset of the in-game base UI element in pixels.
    /// </summary>
    [SctKey("OFFSET", 14)]
    public NavigatorOffset Offset { get; set; } = new(0, 0);

    /// <summary>
    /// The margin to apply to the in-game face UI element in pixels.
    /// </summary>
    [SctKey("MARGIN", 14)]
    public NavigatorMargin FaceMargin { get; set; } = new(0, 0, 0, 0);

    /// <summary>
    /// The time between blinks in milliseconds.
    /// </summary>
    [SctKey("BLINK_INTERVAL", 14)]
    public float BlinkAnimationInterval { get; set; } = 5000;
    
    /// <summary>
    /// The duration of a blink in milliseconds.
    /// </summary>
    [SctKey("BLINK_DURATION", 14)]
    public float BlinkAnimationDuration { get; set; } = 250;

    /// <summary>
    /// The local filepaths of the <see cref="Navigator"/> artwork images, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    [SctLineBreak]
    [SctKey("TEXTURES", 0)]
    public Dictionary<string, string> TexturePaths { get; set; } = [];

    /// <summary>
    /// The collection of textbox dialogues and accompanying voice lines, grouped by language.
    /// </summary>
    [SctLineBreak]
    [SctKey(["WRITING", "DIALOGUES"], 0)]
    public NavigatorWriting Writing { get; set; } = new();
    
    /// <summary>
    /// Returns the absolute filepath of a <see cref="Navigator"/> artwork image.
    /// </summary>
    /// <param name="key">The key of the texture.</param>
    public string AbsoluteTexturePath(string key)
    {
        string localPath = TexturePaths.GetValueOrDefault(key, "");
        return AbsolutePath(localPath);
    }

    /// <summary>
    /// Returns the absolute filepath of a <see cref="NavigatorLine"/>'s audio path.
    /// </summary>
    /// <param name="line">The <see cref="NavigatorLine"/> to take the audio path from.</param>
    /// <returns></returns>
    public string AbsoluteAudioPath(NavigatorLine line) => AbsolutePath(line.AudioPath);
}