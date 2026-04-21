using System;
using System.Globalization;
using System.Text.RegularExpressions;
using SaturnData.Content.Cosmetics;
using SaturnData.Content.Cosmetics.Items;
using SaturnData.Content.Items;
using SaturnData.Content.Localization;
using SaturnData.Content.Music;
using SaturnData.Content.StageUp;
using SaturnData.Utilities;
using ConsoleColor = SaturnData.Content.Cosmetics.ConsoleColor;

namespace SaturnData.Content.Serialization.SatContentV1;

public static class SatContentV1Reader
{
    private const string DictionaryRegexPattern = @"^\s*([a-z_0-9-]+)\s+(.+)";
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
                    "Locale" => new Locale(),
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
                else if (SerializationHelpers.ContainsKey(line, "@NAME ", out value)) { contentItem.Name = value; }

                if (contentItem is CosmeticItem cosmeticItem)
                {
                    if (SerializationHelpers.ContainsKey(line, "@AUTHOR ",      out value)) { cosmeticItem.Author = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@DESCRIPTION ", out value)) { cosmeticItem.Description = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@RARITY ",      out value)) { cosmeticItem.Rarity = Convert.ToInt32(value, CultureInfo.InvariantCulture); }
                }

                if (contentItem is ConsoleColor consoleColor)
                {
                    if (SerializationHelpers.ContainsKey(line, "@COLOR_A ", out value)) { consoleColor.ColorA = Convert.ToUInt32(value, 16); }
                    else if (SerializationHelpers.ContainsKey(line, "@COLOR_B ", out value)) { consoleColor.ColorB = Convert.ToUInt32(value, 16); }
                    else if (SerializationHelpers.ContainsKey(line, "@COLOR_C ", out value)) { consoleColor.ColorC = Convert.ToUInt32(value, 16); }
                    else if (SerializationHelpers.ContainsKey(line, "@LED_A ",   out value)) { consoleColor.LedA = Convert.ToUInt32(value, 16); }
                    else if (SerializationHelpers.ContainsKey(line, "@LED_B ",   out value)) { consoleColor.LedB = Convert.ToUInt32(value, 16); }
                    else if (SerializationHelpers.ContainsKey(line, "@LED_C ",   out value)) { consoleColor.LedC = Convert.ToUInt32(value, 16); }
                }

                if (contentItem is Emblem emblem)
                {
                    if (SerializationHelpers.ContainsKey(line, "@ARTIST ", out value)) { emblem.Artist = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@IMAGE ",  out value)) { emblem.ImagePath = value; }
                }
                
                if (contentItem is Icon icon)
                {
                    if (SerializationHelpers.ContainsKey(line, "@ARTIST ", out value)) { icon.Artist = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@IMAGE ",  out value)) { icon.ImagePath = value; }
                }

                if (contentItem is Navigator navigator)
                {
                    if (SerializationHelpers.ContainsKey(line, "@ARTIST ", out value)) { navigator.Artist = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@VOICE ", out value)) { navigator.Voice = value; }

                    else if (SerializationHelpers.ContainsKey(line, "@SIZE ", out value))
                    {
                        string[] values = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        if (values.Length == 2)
                        {
                            navigator.Width = Convert.ToSingle(values[0], CultureInfo.InvariantCulture);
                            navigator.Height = Convert.ToSingle(values[1], CultureInfo.InvariantCulture);
                        }
                    }
                    
                    else if (SerializationHelpers.ContainsKey(line, "@OFFSET ", out value))
                    {
                        string[] values = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        if (values.Length == 2)
                        {
                            navigator.OffsetX = Convert.ToSingle(values[0], CultureInfo.InvariantCulture);
                            navigator.OffsetY = Convert.ToSingle(values[1], CultureInfo.InvariantCulture);
                        }
                    }
                    
                    else if (SerializationHelpers.ContainsKey(line, "@MARGIN ", out value))
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
                    
                    else if (SerializationHelpers.ContainsKey(line, "@BLINK_INTERVAL ", out value)) { navigator.BlinkAnimationInterval = Convert.ToSingle(value, CultureInfo.InvariantCulture); }
                    else if (SerializationHelpers.ContainsKey(line, "@BLINK_DURATION ", out value)) { navigator.BlinkAnimationDuration = Convert.ToSingle(value, CultureInfo.InvariantCulture); }
                }

                if (contentItem is NoteSound noteSound)
                {
                    if (SerializationHelpers.ContainsKey(line, "@ARTIST ",             out value)) { noteSound.Artist = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@HOLD_LOOP_START ",    out value)) { noteSound.HoldLoopStartTime = Convert.ToSingle(value, CultureInfo.InvariantCulture); }
                    else if (SerializationHelpers.ContainsKey(line, "@HOLD_LOOP_END ",      out value)) { noteSound.HoldLoopEndTime = Convert.ToSingle(value, CultureInfo.InvariantCulture); }
                    else if (SerializationHelpers.ContainsKey(line, "@RE_HOLD_LOOP_START ", out value)) { noteSound.ReHoldLoopStartTime = Convert.ToSingle(value, CultureInfo.InvariantCulture); }
                    else if (SerializationHelpers.ContainsKey(line, "@RE_HOLD_LOOP_END ",   out value)) { noteSound.ReHoldLoopEndTime = Convert.ToSingle(value, CultureInfo.InvariantCulture); }
                    else if (SerializationHelpers.ContainsKey(line, "@CLICK ",              out value)) { noteSound.AudioClickPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@GUIDE ",              out value)) { noteSound.AudioGuidePath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@TOUCH_MARVELOUS ",    out value)) { noteSound.AudioTouchMarvelousPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@TOUCH_GOOD ",         out value)) { noteSound.AudioTouchGoodPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@SLIDE_MARVELOUS ",    out value)) { noteSound.AudioSlideMarvelousPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@SLIDE_GOOD ",         out value)) { noteSound.AudioSlideGoodPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@SNAP_MARVELOUS ",     out value)) { noteSound.AudioSnapMarvelousPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@SNAP_GOOD ",          out value)) { noteSound.AudioSnapGoodPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@HOLD ",               out value)) { noteSound.AudioHoldPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@RE_HOLD ",            out value)) { noteSound.AudioReHoldPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@CHAIN ",              out value)) { noteSound.AudioChainPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@BONUS ",              out value)) { noteSound.AudioBonusPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@R ",                  out value)) { noteSound.AudioRPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@DAMAGE ",             out value)) { noteSound.AudioDamagePath = value; }
                }
                
                if (contentItem is Plate plate)
                {
                    if (SerializationHelpers.ContainsKey(line, "@ARTIST ", out value)) { plate.Artist = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@IMAGE ",  out value)) { plate.ImagePath = value; }
                }
                
                if (contentItem is SystemMusic systemMusic)
                {
                    if (SerializationHelpers.ContainsKey(line, "@ARTIST ",          out value)) { systemMusic.Artist = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@ATTRACT ",         out value)) { systemMusic.AudioAttractPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@SELECT ",          out value)) { systemMusic.AudioSelectPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@RESULT ",          out value)) { systemMusic.AudioResultPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@STAGE_UP_SELECT ", out value)) { systemMusic.AudioStageUpSelectPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@STAGE_UP_SECRET ", out value)) { systemMusic.AudioStageUpSecretPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@SEE_YOU ",         out value)) { systemMusic.AudioSeeYouPath = value; }
                }

                if (contentItem is SystemSound systemSound)
                {
                    if (SerializationHelpers.ContainsKey(line, "@ARTIST ",                    out value)) { systemSound.Artist = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@LOGIN ",                     out value)) { systemSound.AudioLoginPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@CYCLE_MODE ",                out value)) { systemSound.AudioCycleModePath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@CYCLE_FOLDER ",              out value)) { systemSound.AudioCycleFolderPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@CYCLE_SONG ",                out value)) { systemSound.AudioCycleSongPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@CYCLE_OPTION ",              out value)) { systemSound.AudioCycleOptionPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@SELECT_OK ",                 out value)) { systemSound.AudioSelectOkPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@SELECT_BACK ",               out value)) { systemSound.AudioSelectBackPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@SELECT_DENIED ",             out value)) { systemSound.AudioSelectDeniedPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@SELECT_DECIDE ",             out value)) { systemSound.AudioSelectDecidePath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@SELECT_PREVIEW_SONG ",       out value)) { systemSound.AudioSelectPreviewSongPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@SELECT_START_SONG ",         out value)) { systemSound.AudioSelectStartSongPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@SELECT_START_SONG_ALT ",     out value)) { systemSound.AudioSelectStartSongAltPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@FAVORITE_ADD ",              out value)) { systemSound.AudioFavoriteAddPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@FAVORITE_REMOVE ",           out value)) { systemSound.AudioFavoriteRemovePath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@RESULT_SCORE_COUNT ",        out value)) { systemSound.AudioResultScoreCountPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@RESULT_SCORE_FINISHED ",     out value)) { systemSound.AudioResultScoreFinishedPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@RESULT_RATE_BAD ",           out value)) { systemSound.AudioResultRateBadPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@RESULT_RATE_GOOD ",          out value)) { systemSound.AudioResultRateGoodPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@RHYTHM_GAME_READY ",         out value)) { systemSound.AudioRhythmGameReadyPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@RHYTHM_GAME_FAIL ",          out value)) { systemSound.AudioRhythmGameFailPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@RHYTHM_GAME_CLEAR ",         out value)) { systemSound.AudioRhythmGameClearPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@RHYTHM_GAME_SPECIAL_CLEAR ", out value)) { systemSound.AudioRhythmGameSpecialClearPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@TIMER_WARNING ",             out value)) { systemSound.AudioTimerWarningPath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@TEXTBOX_APPEAR ",            out value)) { systemSound.AudioTextboxAppearPath = value; }
                }

                if (contentItem is Title title)
                {
                    if (SerializationHelpers.ContainsKey(line, "@MESSAGE", out value)) { title.Message = value; }
                }

                if (contentItem is Folder folder)
                {
                    if (SerializationHelpers.ContainsKey(line, "@COLOR ", out value)) { folder.Color = Convert.ToUInt32(value, 16); }
                    else if (SerializationHelpers.ContainsKey(line, "@IMAGE ", out value)) { folder.ImagePath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@BACKGROUND ", out value))
                    {
                        folder.Background = value switch
                        {
                            "Checkers" => FolderBackgroundStyle.Checkers,
                            "Triangles" => FolderBackgroundStyle.Triangles,
                            "Circles" => FolderBackgroundStyle.Circles,
                            "Sparkles" => FolderBackgroundStyle.Sparkles,
                            "Arrows" => FolderBackgroundStyle.Arrows,
                            "SquareMesh" => FolderBackgroundStyle.SquareMesh,
                            "TrianglesMesh" => FolderBackgroundStyle.TrianglesMesh,
                            "Stripes" => FolderBackgroundStyle.Stripes,
                            "Dots" => FolderBackgroundStyle.Dots,
                            "Stars" => FolderBackgroundStyle.Stars,
                            "Level" => FolderBackgroundStyle.Level,
                            "Name" => FolderBackgroundStyle.Name,
                            _ => FolderBackgroundStyle.Checkers,
                        };
                    }
                }
                
                if (contentItem is StageUpStage stageUpStage)
                {
                    if (SerializationHelpers.ContainsKey(line, "@IMAGE ",           out value)) { stageUpStage.ImagePath = value; }
                    else if (SerializationHelpers.ContainsKey(line, "@HEALTH ",          out value)) { stageUpStage.Health = Convert.ToInt32(value, CultureInfo.InvariantCulture); }
                    else if (SerializationHelpers.ContainsKey(line, "@HEALTH_RECOVERY ", out value)) { stageUpStage.HealthRecovery = Convert.ToInt32(value, CultureInfo.InvariantCulture); }
                    else if (SerializationHelpers.ContainsKey(line, "@ERROR_THRESHOLD ", out value))
                    {
                        stageUpStage.ErrorThreshold = value switch
                        {
                            "Miss" => ErrorThreshold.Miss,
                            "GoodOrBelow" => ErrorThreshold.GoodOrBelow,
                            "GreatOrBelow" => ErrorThreshold.GreatOrBelow,
                            _ => ErrorThreshold.Miss,
                        };
                    }
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
                        Match match = Regex.Match(line, DictionaryRegexPattern);
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
                    
                    Match variantMatch = Regex.Match(line, DictionaryRegexPattern);
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

        if (contentItem is StageUpStage stageUpStage1)
        {
            StageUpSong? currentSong = null;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith('#')) continue;

                if (line.StartsWith("@SONG_1"))
                {
                    currentSong = stageUpStage1.Song1;
                }
                else if (line.StartsWith("@SONG_2"))
                {
                    currentSong = stageUpStage1.Song2;
                }
                else if (line.StartsWith("@SONG_3"))
                {
                    currentSong = stageUpStage1.Song3;
                }

                if (currentSong != null)
                {
                    try
                    {
                        Match match = Regex.Match(line, DictionaryRegexPattern);
                        if (!match.Success) continue;

                        string key = match.Groups[1].Value;
                        string value = match.Groups[2].Value;

                        if (key == "id")
                        {
                            currentSong.EntryId = value;
                        }
                        else if (key == "secret")
                        {
                            currentSong.Secret = value == "TRUE";
                        }
                    }
                    catch (Exception ex)
                    {
                        // Don't throw.
                        Console.WriteLine(ex);
                    }
                }
            }
        }

        if (contentItem is Locale locale)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith('#')) continue;

                try
                {
                    Match match = Regex.Match(line, DictionaryRegexPattern);
                    if (!match.Success) continue;

                    string key = match.Groups[1].Value;
                    string value = match.Groups[2].Value.Replace(@"\n", "\n");

                    locale.Strings[key] = value;
                }
                catch (Exception ex)
                {
                    // Don't throw.
                    Console.WriteLine(ex);
                }
            }
        }
        
        return contentItem;
    }
}