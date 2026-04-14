using System;
using System.Globalization;
using System.Text.RegularExpressions;
using SaturnData.Content.Cosmetics;
using SaturnData.Content.Cosmetics.Items;
using SaturnData.Content.Items;
using SaturnData.Content.Music;
using SaturnData.Content.StageUp;
using SaturnData.Utilities;
using ConsoleColor = SaturnData.Content.Cosmetics.ConsoleColor;

namespace SaturnData.Content.Serialization.SatContentV1;

public static class SatContentV1Reader
{
    private const string NavigatorDictionaryRegexPattern = @"^\s*([a-z_]+)\s+(.+)";
    private const string NavigatorLanguageRegexPattern = @"^\s{4}([a-zA-Z-]+)";
    private const string NavigatorDialogueRegexPattern = @"^\s{8}([a-z0-9_]+)";
    
    /// <summary>
    /// Reads content data and converts it into a chart.
    /// </summary>
    /// <param name="lines">Content file data separated into individual lines.</param>
    /// <returns>If unsuccessful, <c>null</c>. If successful, an instance of a class that inherits from <see cref="ContentItem"/>, depending on the defined type.</returns>
    public static ContentItem? ToContentItem(string[] lines)
    {
        // Determine type.
        ContentItem? contentItem = null;
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (line.StartsWith('#')) continue;

            if (SerializationHelpers.ContainsKey(line, "@TYPE ", out string type))
            {
                contentItem = type switch
                {
                    "Emblem" => new Emblem(),
                    "Icon" => new Icon(),
                    "ConsoleColor" => new ConsoleColor(),
                    "Navigator" => new Navigator(),
                    "NoteSound" => new NoteSound(),
                    "Plate" => new Plate(),
                    "SystemMusic" => new SystemMusic(),
                    "SystemSound" => new SystemSound(),
                    "Title" => new Title(),
                    "Folder" => new Folder(),
                    "StageUpStage" => new StageUpStage(),
                    _ => null,
                };

                break;
            }
        }

        if (contentItem == null) return null;

        foreach (string line in lines)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith('#')) continue;

                string value;
                if (SerializationHelpers.ContainsKey(line, "@ID ",   out value)) { contentItem.Id = value; }
                if (SerializationHelpers.ContainsKey(line, "@NAME ", out value)) { contentItem.Name = value; }

                if (contentItem is CosmeticItem cosmeticItem)
                {
                    if (SerializationHelpers.ContainsKey(line, "@AUTHOR ",      out value)) { cosmeticItem.Author = value; }
                    if (SerializationHelpers.ContainsKey(line, "@DESCRIPTION ", out value)) { cosmeticItem.Description = value; }
                    if (SerializationHelpers.ContainsKey(line, "@RARITY ",      out value)) { cosmeticItem.Rarity = Convert.ToInt32(value, CultureInfo.InvariantCulture); }
                }

                if (contentItem is ConsoleColor consoleColor)
                {
                    if (SerializationHelpers.ContainsKey(line, "@COLOR_A ", out value)) { consoleColor.ColorA = Convert.ToUInt32(value, 16); }
                    if (SerializationHelpers.ContainsKey(line, "@COLOR_B ", out value)) { consoleColor.ColorB = Convert.ToUInt32(value, 16); }
                    if (SerializationHelpers.ContainsKey(line, "@COLOR_C ", out value)) { consoleColor.ColorC = Convert.ToUInt32(value, 16); }
                    if (SerializationHelpers.ContainsKey(line, "@LED_A ",   out value)) { consoleColor.LedA = Convert.ToUInt32(value, 16); }
                    if (SerializationHelpers.ContainsKey(line, "@LED_B ",   out value)) { consoleColor.LedB = Convert.ToUInt32(value, 16); }
                    if (SerializationHelpers.ContainsKey(line, "@LED_C ",   out value)) { consoleColor.LedC = Convert.ToUInt32(value, 16); }
                }

                if (contentItem is Emblem emblem)
                {
                    if (SerializationHelpers.ContainsKey(line, "@ARTIST ", out value)) { emblem.Artist = value; }
                    if (SerializationHelpers.ContainsKey(line, "@IMAGE ",  out value)) { emblem.ImagePath = value; }
                }
                
                if (contentItem is Icon icon)
                {
                    if (SerializationHelpers.ContainsKey(line, "@ARTIST ", out value)) { icon.Artist = value; }
                    if (SerializationHelpers.ContainsKey(line, "@IMAGE ",  out value)) { icon.ImagePath = value; }
                }

                if (contentItem is Navigator navigator)
                {
                    if (SerializationHelpers.ContainsKey(line, "@ARTIST ", out value)) { navigator.Artist = value; }
                    if (SerializationHelpers.ContainsKey(line, "@VOICE ", out value)) { navigator.Voice = value; }

                    if (SerializationHelpers.ContainsKey(line, "@SIZE ", out value))
                    {
                        string[] values = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        if (values.Length == 2)
                        {
                            navigator.Width = Convert.ToSingle(values[0], CultureInfo.InvariantCulture);
                            navigator.Height = Convert.ToSingle(values[1], CultureInfo.InvariantCulture);
                        }
                    }
                    
                    if (SerializationHelpers.ContainsKey(line, "@OFFSET ", out value))
                    {
                        string[] values = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        if (values.Length == 2)
                        {
                            navigator.OffsetX = Convert.ToSingle(values[0], CultureInfo.InvariantCulture);
                            navigator.OffsetY = Convert.ToSingle(values[1], CultureInfo.InvariantCulture);
                        }
                    }
                    
                    if (SerializationHelpers.ContainsKey(line, "@MARGIN ", out value))
                    {
                        string[] values = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        if (values.Length == 4)
                        {
                            navigator.FaceMarginTop = Convert.ToSingle(values[0], CultureInfo.InvariantCulture);
                            navigator.FaceMarginBottom = Convert.ToSingle(values[1], CultureInfo.InvariantCulture);
                            navigator.FaceMarginLeft = Convert.ToSingle(values[2], CultureInfo.InvariantCulture);
                            navigator.FaceMarginRight = Convert.ToSingle(values[3], CultureInfo.InvariantCulture);
                        }
                    }
                    
                    if (SerializationHelpers.ContainsKey(line, "@BLINK_INTERVAL ", out value)) { navigator.BlinkAnimationInterval = Convert.ToSingle(value, CultureInfo.InvariantCulture); }
                    if (SerializationHelpers.ContainsKey(line, "@BLINK_DURATION ", out value)) { navigator.BlinkAnimationDuration = Convert.ToSingle(value, CultureInfo.InvariantCulture); }
                }

                if (contentItem is NoteSound noteSound)
                {
                    if (SerializationHelpers.ContainsKey(line, "@ARTIST ",             out value)) { noteSound.Artist = value; }
                    if (SerializationHelpers.ContainsKey(line, "@HOLD_LOOP_START ",    out value)) { noteSound.HoldLoopStartTime = Convert.ToSingle(value, CultureInfo.InvariantCulture); }
                    if (SerializationHelpers.ContainsKey(line, "@HOLD_LOOP_END ",      out value)) { noteSound.HoldLoopEndTime = Convert.ToSingle(value, CultureInfo.InvariantCulture); }
                    if (SerializationHelpers.ContainsKey(line, "@RE_HOLD_LOOP_START ", out value)) { noteSound.ReHoldLoopStartTime = Convert.ToSingle(value, CultureInfo.InvariantCulture); }
                    if (SerializationHelpers.ContainsKey(line, "@RE_HOLD_LOOP_END ",   out value)) { noteSound.ReHoldLoopEndTime = Convert.ToSingle(value, CultureInfo.InvariantCulture); }
                    if (SerializationHelpers.ContainsKey(line, "@CLICK ",              out value)) { noteSound.AudioClickPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@GUIDE ",              out value)) { noteSound.AudioGuidePath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@TOUCH_MARVELOUS ",    out value)) { noteSound.AudioTouchMarvelousPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@TOUCH_GOOD ",         out value)) { noteSound.AudioTouchGoodPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@SLIDE_MARVELOUS ",    out value)) { noteSound.AudioSlideMarvelousPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@SLIDE_GOOD ",         out value)) { noteSound.AudioSlideGoodPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@SNAP_MARVELOUS ",     out value)) { noteSound.AudioSnapMarvelousPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@SNAP_GOOD ",          out value)) { noteSound.AudioSnapGoodPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@HOLD ",               out value)) { noteSound.AudioHoldPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@RE_HOLD ",            out value)) { noteSound.AudioReHoldPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@CHAIN ",              out value)) { noteSound.AudioChainPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@BONUS ",              out value)) { noteSound.AudioBonusPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@R ",                  out value)) { noteSound.AudioRPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@DAMAGE ",             out value)) { noteSound.AudioDamagePath = value; }
                }
                
                if (contentItem is Plate plate)
                {
                    if (SerializationHelpers.ContainsKey(line, "@ARTIST ", out value)) { plate.Artist = value; }
                    if (SerializationHelpers.ContainsKey(line, "@IMAGE ",  out value)) { plate.ImagePath = value; }
                }
                
                if (contentItem is SystemMusic systemMusic)
                {
                    if (SerializationHelpers.ContainsKey(line, "@ARTIST ",          out value)) { systemMusic.Artist = value; }
                    if (SerializationHelpers.ContainsKey(line, "@ATTRACT ",         out value)) { systemMusic.AudioAttractPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@SELECT ",          out value)) { systemMusic.AudioSelectPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@RESULT ",          out value)) { systemMusic.AudioResultPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@STAGE_UP_SELECT ", out value)) { systemMusic.AudioStageUpSelectPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@STAGE_UP_SECRET ", out value)) { systemMusic.AudioStageUpSecretPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@SEE_YOU ",         out value)) { systemMusic.AudioSeeYouPath = value; }
                }

                if (contentItem is SystemSound systemSound)
                {
                    if (SerializationHelpers.ContainsKey(line, "@ARTIST",                    out value)) { systemSound.Artist = value; }
                    if (SerializationHelpers.ContainsKey(line, "@LOGIN",                     out value)) { systemSound.AudioLoginPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@CYCLE_MODE",                out value)) { systemSound.AudioCycleModePath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@CYCLE_FOLDER",              out value)) { systemSound.AudioCycleFolderPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@CYCLE_SONG",                out value)) { systemSound.AudioCycleSongPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@CYCLE_OPTION",              out value)) { systemSound.AudioCycleOptionPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@SELECT_OK",                 out value)) { systemSound.AudioSelectOkPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@SELECT_BACK",               out value)) { systemSound.AudioSelectBackPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@SELECT_DENIED",             out value)) { systemSound.AudioSelectDeniedPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@SELECT_DECIDE",             out value)) { systemSound.AudioSelectDecidePath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@SELECT_PREVIEW_SONG",       out value)) { systemSound.AudioSelectPreviewSongPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@SELECT_START_SONG",         out value)) { systemSound.AudioSelectStartSongPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@SELECT_START_SONG_ALT",     out value)) { systemSound.AudioSelectStartSongAltPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@FAVORITE_ADD",              out value)) { systemSound.AudioFavoriteAddPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@FAVORITE_REMOVE",           out value)) { systemSound.AudioFavoriteRemovePath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@RESULT_SCORE_COUNT",        out value)) { systemSound.AudioResultScoreCountPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@RESULT_SCORE_FINISHED",     out value)) { systemSound.AudioResultScoreFinishedPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@RESULT_RATE_BAD",           out value)) { systemSound.AudioResultRateBadPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@RESULT_RATE_GOOD",          out value)) { systemSound.AudioResultRateGoodPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@RHYTHM_GAME_READY",         out value)) { systemSound.AudioRhythmGameReadyPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@RHYTHM_GAME_FAIL",          out value)) { systemSound.AudioRhythmGameFailPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@RHYTHM_GAME_CLEAR",         out value)) { systemSound.AudioRhythmGameClearPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@RHYTHM_GAME_SPECIAL_CLEAR", out value)) { systemSound.AudioRhythmGameSpecialClearPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@TIMER_WARNING",             out value)) { systemSound.AudioTimerWarningPath = value; }
                    if (SerializationHelpers.ContainsKey(line, "@TEXTBOX_APPEAR",            out value)) { systemSound.AudioTextboxAppearPath = value; }
                }

                if (contentItem is Title title)
                {
                    if (SerializationHelpers.ContainsKey(line, "@MESSAGE", out value)) { title.Message = value; }
                }
            }
            catch (Exception ex)
            {
                // Don't throw.
                Console.WriteLine(ex);
            }
        }

        if (contentItem is Navigator navigator1)
        {
            bool readingTextures = false;
            bool readingDialogues = false;

            NavigatorDialogueLanguage? currentLanguage = null;
            NavigatorDialogueVariantCollection? currentDialogue = null;
            NavigatorDialogue? currentVariant = null;
            
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith('#')) continue;
                
                if (line.StartsWith("@TEXTURES"))
                {
                    readingTextures = true;
                    readingDialogues = false;
                    continue;
                }

                if (line.StartsWith("@DIALOGUES"))
                {
                    readingTextures = false;
                    readingDialogues = true;
                    continue;
                }
                
                if (readingTextures)
                {
                    try
                    {
                        Match match = Regex.Match(line, NavigatorDictionaryRegexPattern);
                        if (!match.Success) continue;
                    
                        navigator1.TexturePaths[match.Groups[1].Value] = match.Groups[2].Value;
                    }
                    catch (Exception ex)
                    {
                        // Don't throw.
                        Console.WriteLine(ex);
                    }
                }
                else if (readingDialogues)
                {
                    // Defining new language.
                    Match languageMatch = Regex.Match(line, NavigatorLanguageRegexPattern);
                    if (languageMatch.Success)
                    {
                        string localeKey = languageMatch.Groups[1].Value;
                        
                        currentLanguage = new();
                        currentDialogue = null;
                        currentVariant = null;
                        
                        navigator1.DialogueLanguages[localeKey] = currentLanguage;
                        continue;
                    }
                    
                    // Defining new dialogue.
                    Match dialogueMatch = Regex.Match(line, NavigatorDialogueRegexPattern);
                    if (dialogueMatch.Success)
                    {
                        string dialogueKey = dialogueMatch.Groups[1].Value;

                        currentDialogue = new();
                        currentVariant = null;

                        if (currentLanguage != null)
                        {
                            currentLanguage.Dialogues[dialogueKey] = currentDialogue;
                        }
                        continue;
                    }
                    
                    // Defining new variant.
                    if (currentDialogue == null) continue;
                    
                    Match variantMatch = Regex.Match(line, NavigatorDictionaryRegexPattern);
                    if (!variantMatch.Success) continue;

                    string key = variantMatch.Groups[1].Value;
                    string value = variantMatch.Groups[2].Value;

                    if (key == "message")
                    {
                        currentVariant = new() { Message = value, };
                        currentDialogue.Variants.Add(currentVariant);
                    }
                    else if (currentVariant != null && key == "audio_path")
                    {
                        currentVariant.AudioPath = value;
                    }
                    else if (currentVariant != null && key == "duration")
                    {
                        try
                        {
                            currentVariant.Duration = Convert.ToSingle(value, CultureInfo.InvariantCulture);
                        }
                        catch (Exception ex)
                        {
                            // Don't throw.
                            Console.WriteLine(ex);
                        }
                    }
                    else if (currentVariant != null && key == "show_skip")
                    {
                        currentVariant.ShowSkipButton = value == "TRUE";
                    }
                    else if (currentVariant != null && key == "expression")
                    {
                        currentVariant.Expression = value switch
                        {
                            "Neutral" => NavigatorExpression.Neutral,
                            "Amazed" => NavigatorExpression.Amazed,
                            "Troubled" => NavigatorExpression.Troubled,
                            "Surprised" => NavigatorExpression.Surprised,
                            "Startled" => NavigatorExpression.Startled,
                            "Angry" => NavigatorExpression.Angry,
                            "Laughing" => NavigatorExpression.Laughing,
                            "Smiling" => NavigatorExpression.Smiling,
                            "Grinning" => NavigatorExpression.Grinning,
                            _ => NavigatorExpression.Neutral,
                        };
                    }
                }
            }
        }
        
        return contentItem;
    }
}