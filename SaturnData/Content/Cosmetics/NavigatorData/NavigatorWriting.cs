using System.Collections.Generic;
using SaturnData.Content.Serialization;

namespace SaturnData.Content.Cosmetics.NavigatorData;

public class NavigatorWriting : ISctSerializable
{
    public Dictionary<string, NavigatorLanguage> Languages { get; set; } = [];
    
    public string Serialize()
    {
        return "";
    }

    public static NavigatorWriting Deserialize(string[] data)
    {
        return null;
    }
    
    /// <summary>
    /// Returns a random variant of a dialogue in the specified language.
    /// If the language (or dialogue in the language) aren't found, it falls back to the locale <c>en-US</c>, then the first locale that contains the specified dialogue.
    /// </summary>
    /// <param name="locale">The locale code of the desired language.</param>
    /// <param name="key">The key of the dialogue to get.</param>
    /// <returns></returns>
    public NavigatorLine GetLine(string locale, string key)
    {
        if (Languages.Count == 0) return NavigatorLine.NoMessageFound(key);

        // Try to get desired language.
        if (Languages.TryGetValue(locale, out NavigatorLanguage? desiredLanguage))
        {
            if (desiredLanguage.Dialogues.TryGetValue(key, out NavigatorDialogue? collection))
            {
                return collection.RandomLine;
            }
        }
        
        // Try to get en-US
        if (Languages.TryGetValue("en-US", out NavigatorLanguage? fallbackLanguage))
        {
            if (fallbackLanguage.Dialogues.TryGetValue(key, out NavigatorDialogue? collection))
            {
                return collection.RandomLine;
            }
        }
        
        // Search through all languages and return dialogue in the first language it exists in.
        foreach (NavigatorLanguage language in Languages.Values)
        {
            if (!language.Dialogues.TryGetValue(key, out NavigatorDialogue? collection)) continue;

            return collection.RandomLine;
        }
        
        // Return no message found.
        return NavigatorLine.NoMessageFound(key);

        
    }
}