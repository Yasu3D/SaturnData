using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using SaturnData.Content.Serialization;

namespace SaturnData.Content.Cosmetics.NavigatorData;

public class NavigatorWriting : ISctSerializable
{
    public Dictionary<string, NavigatorLanguage> Languages { get; set; } = [];
    
    public string Serialize(ContentFormatVersion formatVersion)
    {
        if (formatVersion != ContentFormatVersion.SatContentV2) return "";
        
        StringBuilder sb = new();

        sb.AppendLine();
        
        foreach (KeyValuePair<string, NavigatorLanguage> language in Languages)
        {
            sb.AppendLine($"{language.Key.Indent()}");

            foreach (KeyValuePair<string, NavigatorDialogue> dialogue in language.Value.Dialogues)
            {
                sb.AppendLine($"{dialogue.Key.Indent(2)}");
                
                foreach (NavigatorLine line in dialogue.Value.Lines)
                {
                    string message = $"\"{line.Message.Escape()}\"";
                    string audio = $"\"{line.AudioPath.Escape()}\"";
                    string duration = line.Duration.ToString("0.0", CultureInfo.InvariantCulture);
                    string skip = line.ShowSkipButton ? "TRUE " : "FALSE";
                    string expression = line.Expression.ToString();
                    
                    sb.AppendLine($"{message} {audio} {duration} {skip} {expression}".Indent(3));
                }
            }
        }

        return sb.ToString();
    }

    public void Deserialize(string[] data, ContentFormatVersion formatVersion)
    {
        if (formatVersion != ContentFormatVersion.SatContentV2) return;
        
        int languageIndent = -1;
        int dialogueIndent = -1;
        int lineIndent = -1;
        
        NavigatorLanguage? currentLanguage = null;
        NavigatorDialogue? currentDialogue = null;

        foreach (string line in data)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line.StartsWith('#')) continue;
            
            int indent = line.GetSpacesAtStart();
            
            if (languageIndent == -1)
            {
                languageIndent = indent;
            }
            else if (dialogueIndent == -1 && indent > languageIndent)
            {
                dialogueIndent = indent;
            }
            else if (lineIndent == -1 && indent > dialogueIndent)
            {
                lineIndent = indent;
            }

            // Language
            if (indent <= languageIndent)
            {
                string key = line.Trim();
                
                Languages.TryGetValue(key, out currentLanguage);

                if (currentLanguage == null)
                {
                    currentLanguage = new();
                    Languages[key] = currentLanguage;
                }
                
                continue;
            }

            // Dialogue
            if (indent <= dialogueIndent)
            {
                if (currentLanguage == null) continue;
                string key = line.Trim();

                currentLanguage.Dialogues.TryGetValue(key, out currentDialogue);
                
                if (currentDialogue == null)
                {
                    currentDialogue = new();
                    currentLanguage.Dialogues[key] = currentDialogue;
                }
                
                continue;
            }
            
            // Line
            if (indent <= lineIndent)
            {
                if (currentDialogue == null) continue;
                
                Match match = NavigatorLineRegex.Match(line);

                if (!match.Success) continue;

                try
                {
                    string message = match.Groups[1].Value.Unescape();
                    string audioPath = match.Groups[2].Value.Unescape();
                    float duration = Convert.ToSingle(match.Groups[3].Value, CultureInfo.InvariantCulture);
                    bool showSkipButton = match.Groups[4].Value == "TRUE";
                    NavigatorExpression expression = Enum.Parse<NavigatorExpression>(match.Groups[5].Value);

                    NavigatorLine navigatorLine = new()
                    {
                        Message = message,
                        AudioPath = audioPath,
                        Duration = duration,
                        ShowSkipButton = showSkipButton,
                        Expression = expression,
                    };

                    currentDialogue.Lines.Add(navigatorLine);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
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
    
    private static readonly Regex NavigatorLineRegex = new(@"""(.*)""\s+""(.*)""\s+([0-9.]+)\s+(TRUE|FALSE)\s+([a-zA-Z]+)");
}