using System;
using System.Collections.Generic;

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
[Serializable]
public class Navigator : ContentItem
{
    /// <summary>
    /// The creator of the <see cref="Navigator"/> artwork.
    /// </summary>
    public string Artist { get; set; } = "";

    /// <summary>
    /// The voice actor of the <see cref="Navigator"/> voice lines.
    /// </summary>
    public string Voice { get; set; } = "";
    
    /// <summary>
    /// The width of the in-game UI element in pixels.
    /// </summary>
    public float Width { get; set; } = 1024;
    
    /// <summary>
    /// The height of the in-game UI element in pixels.
    /// </summary>
    public float Height { get; set; } = 1024;
    
    /// <summary>
    /// The horizontal offset to apply to the in-game UI element.
    /// </summary>
    /// <remarks>
    /// - Negative values move UI element left.<br/>
    /// - Positive values move UI element right.
    /// </remarks>
    public float OffsetX { get; set; } = 0;
    
    /// <summary>
    /// The vertical offset to apply to the in-game UI element.
    /// </summary>
    /// <remarks>
    /// - Negative values move UI element down.<br/>
    /// - Positive values move UI element up.
    /// </remarks>
    public float OffsetY { get; set; } = 0;

    /// <summary>
    /// The distance from the top edge of the main UI element to the top edge of the face UI element.
    /// </summary>
    public float FaceMarginTop { get; set; } = 0;
    
    /// <summary>
    /// The distance from the bottom edge of the main UI element to the bottom edge of the face UI element.
    /// </summary>
    public float FaceMarginBottom { get; set; } = 0;
    
    /// <summary>
    /// The distance from the left edge of the main UI element to the left edge of the face UI element.
    /// </summary>
    public float FaceMarginLeft { get; set; } = 0;
    
    /// <summary>
    /// The distance from the right edge of the main UI element to the right edge of the face UI element.
    /// </summary>
    public float FaceMarginRight { get; set; } = 0;

    /// <summary>
    /// The time in milliseconds between blinks.
    /// </summary>
    public float BlinkAnimationInterval { get; set; } = 5000;
    
    /// <summary>
    /// The duration of a blink in milliseconds.
    /// </summary>
    public float BlinkAnimationDuration { get; set; } = 250;

    /// <summary>
    /// The local filepaths of the <see cref="Navigator"/> artwork images, relative to the <see cref="ContentItem.AbsoluteSourcePath"/>.
    /// </summary>
    public Dictionary<string, string> TexturePaths { get; set; } = [];

    /// <summary>
    /// The collection of textbox dialogues and accompanying voice lines, grouped by language.
    /// </summary>
    public Dictionary<string, NavigatorDialogueLanguage> DialogueLanguages { get; set; } = [];

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
    /// Returns the absolute filepath of a <see cref="NavigatorDialogue"/>'s audio path.
    /// </summary>
    /// <param name="variant">The <see cref="NavigatorDialogue"/> to take the audio path from.</param>
    /// <returns></returns>
    public string AbsoluteAudioPath(NavigatorDialogue variant) => AbsolutePath(variant.AudioPath);

    /// <summary>
    /// Returns a random variant of a dialogue in the specified language.
    /// If the language (or dialogue in the language) aren't found, it falls back to the locale <c>en-US</c>, then the first locale that contains the specified dialogue.
    /// </summary>
    /// <param name="locale">The locale code of the desired language.</param>
    /// <param name="key">The key of the dialogue to get.</param>
    /// <returns></returns>
    public NavigatorDialogue GetDialogue(string locale, string key)
    {
        if (DialogueLanguages.Count == 0) return NavigatorDialogue.ErrorDialogue;

        // Try to get desired language.
        if (DialogueLanguages.TryGetValue(locale, out NavigatorDialogueLanguage desiredLanguage))
        {
            if (desiredLanguage.Dialogues.TryGetValue(key, out NavigatorDialogueVariantCollection collection))
            {
                return collection.RandomVariant;
            }
        }
        
        // Try to get en-US
        if (DialogueLanguages.TryGetValue("en-US", out NavigatorDialogueLanguage fallbackLanguage))
        {
            if (fallbackLanguage.Dialogues.TryGetValue(key, out NavigatorDialogueVariantCollection collection))
            {
                return collection.RandomVariant;
            }
        }
        
        // Search through all languages and return dialogue in the first language it exists in.
        foreach (NavigatorDialogueLanguage language in DialogueLanguages.Values)
        {
            if (!language.Dialogues.TryGetValue(key, out NavigatorDialogueVariantCollection collection)) continue;

            return collection.RandomVariant;
        }
        
        // Give up.
        return NavigatorDialogue.ErrorDialogue;
    }
}

/// <summary>
/// A collection of <see cref="NavigatorDialogueVariantCollection"/>s in one language.
/// </summary>
[Serializable]
public class NavigatorDialogueLanguage
{
    /// <summary>
    /// The collection of dialogues, accessible by their key.
    /// </summary>
    public Dictionary<string, NavigatorDialogueVariantCollection> Dialogues { get; set; } = [];
}

/// <summary> 
/// A collection of <see cref="NavigatorDialogue"/>s to randomly choose from.
/// </summary>
[Serializable]
public class NavigatorDialogueVariantCollection
{
    /// <summary>
    /// The collection of dialogue variants.
    /// </summary>
    public List<NavigatorDialogue> Variants { get; set; } = [];

    /// <summary>
    /// A random dialogue variant chosen from <see cref="Variants"/>
    /// </summary>
    public NavigatorDialogue RandomVariant
    {
        get
        {
            if (Variants.Count == 1) return Variants[0];

            if (Variants.Count > 1)
            {
                Random random = new();
                int index = random.Next(0, Variants.Count);

                return Variants[index];
            }
            
            return NavigatorDialogue.ErrorDialogue;
        }
    }
}

/// <summary>
/// Describes a single dialogue line to display in a textbox.
/// </summary>
[Serializable]
public class NavigatorDialogue
{
    /// <summary>
    /// The dialogue message to display in a textbox.
    /// </summary>
    public string Message { get; set; } = "";
    
    /// <summary>
    /// The local filepath to a spoken version of the message.
    /// </summary>
    public string AudioPath { get; set; } = "";
    
    /// <summary>
    /// The duration of how long to show the textbox.
    /// </summary>
    /// <remarks>
    /// This value should be greater than or equal to the duration of the audio file defined by <see cref="AudioPath"/>.
    /// </remarks>
    public float Duration { get; set; } = 5000;
    
    /// <summary>
    /// Determines if a button to skip a textbox should pop up.
    /// </summary>
    public bool ShowSkipButton { get; set; } = false;
    
    /// <summary>
    /// Determines the expression the chosen navigator will show.
    /// </summary>
    public NavigatorExpression Expression { get; set; } = NavigatorExpression.Neutral;

    /// <summary>
    /// A placeholder <see cref="NavigatorDialogue"/> to display if no valid data was found.
    /// </summary>
    public static NavigatorDialogue ErrorDialogue => new()
    {
        Message = "ERROR: NO DATA FOUND.",
        AudioPath = "",
        Duration = 10000,
        ShowSkipButton = false,
        Expression = NavigatorExpression.Neutral,
    };
}