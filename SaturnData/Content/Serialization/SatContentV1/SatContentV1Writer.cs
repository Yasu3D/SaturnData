using System;
using System.Collections.Generic;
using System.Text;
using SaturnData.Content.Cosmetics;
using SaturnData.Content.Cosmetics.Items;
using SaturnData.Content.Items;
using SaturnData.Content.Music;
using SaturnData.Content.StageUp;
using ConsoleColor = SaturnData.Content.Cosmetics.ConsoleColor;

namespace SaturnData.Content.Serialization.SatContentV1;

public static class SatContentV1Writer
{
    public static string ToString(ContentItem contentItem)
    {
        StringBuilder sb = new();

        sb.Append($"{"@SAT_C_VERSION",-16}1\n");

        string type = contentItem switch
        {
            Emblem => "Emblem",
            Icon => "Icon",
            ConsoleColor => "ConsoleColor",
            Navigator => "Navigator",
            NoteSound => "NoteSound",
            Plate => "Plate",
            SystemMusic => "SystemMusic",
            SystemSound => "SystemSound",
            Title => "Title",
            Folder => "Folder",
            StageUpStage => "StageUpStage",
            _ => throw new ArgumentOutOfRangeException(nameof(contentItem)),
        };

        sb.Append($"{"@TYPE",-16}{type}");
        sb.Append("\n");

        sb.Append($"{"@ID",-16}{contentItem.Id}\n");
        sb.Append($"{"@NAME",-16}{contentItem.Name}");
        sb.Append("\n");

        if (contentItem is CosmeticItem cosmeticItem)
        {
            sb.Append($"{"@AUTHOR",-16}{cosmeticItem.Author}\n");
            sb.Append($"{"@DESCRIPTION",-16}{cosmeticItem.Description}\n");
            sb.Append($"{"@RARITY",-16}{cosmeticItem.Rarity}\n");
        }

        if (contentItem is ConsoleColor consoleColor)
        {
            sb.Append($"{"@COLOR_A",-16}{consoleColor.ColorA:X8}\n");
            sb.Append($"{"@COLOR_B",-16}{consoleColor.ColorB:X8}\n");
            sb.Append($"{"@COLOR_C",-16}{consoleColor.ColorC:X8}\n");
            sb.Append($"{"@LED_A",-16}{consoleColor.LedA:X8}\n");
            sb.Append($"{"@LED_B",-16}{consoleColor.LedB:X8}\n");
            sb.Append($"{"@LED_C",-16}{consoleColor.LedC:X8}\n");
        }
        
        if (contentItem is Emblem emblem)
        {
            sb.Append($"{"@ARTIST",-16}{emblem.Artist}\n");
            sb.Append($"{"@IMAGE",-16}{emblem.ImagePath}\n");
        }
        
        if (contentItem is Icon icon)
        {
            sb.Append($"{"@ARTIST",-16}{icon.Artist}\n");
            sb.Append($"{"@IMAGE",-16}{icon.ImagePath}\n");
        }
        
        if (contentItem is Navigator navigator)
        {
            sb.Append($"{"@ARTIST",-16}{navigator.Artist}\n");
            sb.Append($"{"@VOICE",-16}{navigator.Voice}\n");
            sb.Append($"{"@SIZE",-16}{navigator.Width,-4} {navigator.Height,-4}\n");
            sb.Append($"{"@OFFSET",-16}{navigator.OffsetX,-4} {navigator.OffsetY,-4}\n");
            sb.Append($"{"@MARGIN",-16}{navigator.FaceMarginTop,-4} {navigator.FaceMarginBottom,-4} {navigator.FaceMarginLeft,-4} {navigator.FaceMarginRight,-4}\n");
            sb.Append($"{"@BLINK_INTERVAL",-16}{navigator.BlinkAnimationInterval}\n");
            sb.Append($"{"@BLINK_DURATION",-16}{navigator.BlinkAnimationDuration}\n");
            sb.Append("@TEXTURES\n");
            foreach (KeyValuePair<string, string> pair in navigator.TexturePaths)
            {
                sb.Append($"    {pair.Key,-17}{pair.Value}");
            }

            sb.Append("\n");

            sb.Append("@LANGUAGES\n");
            foreach (KeyValuePair<string, NavigatorDialogueLanguage> language in navigator.DialogueLanguages)
            {
                sb.Append($"    {language.Key}\n");
                
                foreach (KeyValuePair<string, NavigatorDialogueVariantCollection> dialogue in language.Value.Dialogues)
                {
                    sb.Append($"        {dialogue.Key}\n");
                    
                    foreach (NavigatorDialogue variant in dialogue.Value.Variants)
                    {
                        sb.Append($"            {"message",-11}{variant.Message}");
                        sb.Append($"            {"audio_path",-11}{variant.AudioPath}");
                        sb.Append($"            {"duration",-11}{variant.Duration}");
                        sb.Append($"            {"show_skip",-11}{(variant.ShowSkipButton ? "TRUE" : "FALSE")}");
                        sb.Append($"            {"expression",-11}{variant.Expression}");
                        sb.Append("\n");
                    }
                }

                sb.Append("\n");
            }
        }
        
        if (contentItem is NoteSound noteSound)
        {
            sb.Append($"{"@ARTIST",-19}{noteSound.Artist}\n");
            sb.Append($"{"@HOLD_LOOP_START",-19}{noteSound.HoldLoopStartTime}\n");
            sb.Append($"{"@HOLD_LOOP_END",-19}{noteSound.HoldLoopEndTime}\n");
            sb.Append($"{"@RE_HOLD_LOOP_START",-19}{noteSound.ReHoldLoopStartTime}\n");
            sb.Append($"{"@RE_HOLD_LOOP_END",-19}{noteSound.ReHoldLoopEndTime}\n");
            sb.Append($"{"@CLICK",-19}{noteSound.AudioClickPath}\n");
            sb.Append($"{"@GUIDE",-19}{noteSound.AudioGuidePath}\n");
            sb.Append($"{"@TOUCH_MARVELOUS",-19}{noteSound.AudioTouchMarvelousPath}\n");
            sb.Append($"{"@TOUCH_GOOD",-19}{noteSound.AudioTouchGoodPath}\n");
            sb.Append($"{"@SLIDE_MARVELOUS",-19}{noteSound.AudioSlideMarvelousPath}\n");
            sb.Append($"{"@SLIDE_GOOD",-19}{noteSound.AudioSlideGoodPath}\n");
            sb.Append($"{"@SNAP_MARVELOUS",-19}{noteSound.AudioSnapMarvelousPath}\n");
            sb.Append($"{"@SNAP_GOOD",-19}{noteSound.AudioSnapGoodPath}\n");
            sb.Append($"{"@HOLD",-19}{noteSound.AudioHoldPath}\n");
            sb.Append($"{"@RE_HOLD",-19}{noteSound.AudioReHoldPath}\n");
            sb.Append($"{"@CHAIN",-19}{noteSound.AudioChainPath}\n");
            sb.Append($"{"@BONUS",-19}{noteSound.AudioBonusPath}\n");
            sb.Append($"{"@R",-19}{noteSound.AudioRPath}\n");
            sb.Append($"{"@DAMAGE",-19}{noteSound.AudioDamagePath}\n");
        }
        
        if (contentItem is Plate plate)
        {
            sb.Append($"{"@ARTIST",-16}{plate.Artist}\n");
            sb.Append($"{"@IMAGE",-16}{plate.ImagePath}\n");
        }
        
        if (contentItem is SystemMusic systemMusic)
        {
            sb.Append($"{"@ARTIST",-17}{systemMusic.Artist}\n");
            sb.Append($"{"@ATTRACT",-17}{systemMusic.AudioAttractPath}\n");
            sb.Append($"{"@SELECT",-17}{systemMusic.AudioSelectPath}\n");
            sb.Append($"{"@RESULT",-17}{systemMusic.AudioResultPath}\n");
            sb.Append($"{"@STAGE_UP_SELECT",-17}{systemMusic.AudioStageUpSelectPath}\n");
            sb.Append($"{"@STAGE_UP_SECRET",-17}{systemMusic.AudioStageUpSecretPath}\n");
            sb.Append($"{"@SEE_YOU",-17}{systemMusic.AudioSeeYouPath}\n");
        }

        if (contentItem is SystemSound systemSound)
        {
            sb.Append($"{"@ARTIST",-27}{systemSound.Artist}\n");
            sb.Append($"{"@LOGIN",-27}{systemSound.AudioLoginPath}\n");
            sb.Append($"{"@CYCLE_MODE",-27}{systemSound.AudioCycleModePath}\n");
            sb.Append($"{"@CYCLE_FOLDER",-27}{systemSound.AudioCycleFolderPath}\n");
            sb.Append($"{"@CYCLE_SONG",-27}{systemSound.AudioCycleSongPath}\n");
            sb.Append($"{"@CYCLE_OPTION",-27}{systemSound.AudioCycleOptionPath}\n");
            sb.Append($"{"@SELECT_OK",-27}{systemSound.AudioSelectOkPath}\n");
            sb.Append($"{"@SELECT_BACK",-27}{systemSound.AudioSelectBackPath}\n");
            sb.Append($"{"@SELECT_DENIED",-27}{systemSound.AudioSelectDeniedPath}\n");
            sb.Append($"{"@SELECT_DECIDE",-27}{systemSound.AudioSelectDecidePath}\n");
            sb.Append($"{"@SELECT_PREVIEW_SONG",-27}{systemSound.AudioSelectPreviewSongPath}\n");
            sb.Append($"{"@SELECT_START_SONG",-27}{systemSound.AudioSelectStartSongPath}\n");
            sb.Append($"{"@SELECT_START_SONG_ALT",-27}{systemSound.AudioSelectStartSongAltPath}\n");
            sb.Append($"{"@FAVORITE_ADD",-27}{systemSound.AudioFavoriteAddPath}\n");
            sb.Append($"{"@FAVORITE_REMOVE",-27}{systemSound.AudioFavoriteRemovePath}\n");
            sb.Append($"{"@RESULT_SCORE_COUNT",-27}{systemSound.AudioResultScoreCountPath}\n");
            sb.Append($"{"@RESULT_SCORE_FINISHED",-27}{systemSound.AudioResultScoreFinishedPath}\n");
            sb.Append($"{"@RESULT_RATE_BAD",-27}{systemSound.AudioResultRateBadPath}\n");
            sb.Append($"{"@RESULT_RATE_GOOD",-27}{systemSound.AudioResultRateGoodPath}\n");
            sb.Append($"{"@RHYTHM_GAME_READY",-27}{systemSound.AudioRhythmGameReadyPath}\n");
            sb.Append($"{"@RHYTHM_GAME_FAIL",-27}{systemSound.AudioRhythmGameFailPath}\n");
            sb.Append($"{"@RHYTHM_GAME_CLEAR",-27}{systemSound.AudioRhythmGameClearPath}\n");
            sb.Append($"{"@RHYTHM_GAME_SPECIAL_CLEAR",-27}{systemSound.AudioRhythmGameSpecialClearPath}\n");
            sb.Append($"{"@TIMER_WARNING",-27}{systemSound.AudioTimerWarningPath}\n");
            sb.Append($"{"@TEXTBOX_APPEAR",-27}{systemSound.AudioTextboxAppearPath}\n");
        }

        if (contentItem is Title title)
        {
            sb.Append($"{"@MESSAGE",-16}{title.Message}\n");
        }

        if (contentItem is Folder folder)
        {
            sb.Append($"{"@COLOR",-16}{folder.Color:X8}\n");
            sb.Append($"{"@BACKGROUND",-16}{folder.Background}\n");
            sb.Append($"{"@IMAGE",-16}{folder.ImagePath}\n");
        }

        if (contentItem is StageUpStage stageUpStage)
        {
            sb.Append($"{"@IMAGE",-17}{stageUpStage.ImagePath}\n");
            sb.Append($"{"@ERROR_THRESHOLD",-17}{stageUpStage.ErrorThreshold}\n");
            sb.Append($"{"@HEALTH",-17}{stageUpStage.Health}\n");
            sb.Append($"{"@HEALTH_RECOVERY",-17}{stageUpStage.HealthRecovery}\n");
            sb.Append("@SONG_1\n");
            sb.Append($"    {"id",-7 }{stageUpStage.Song1.EntryId}\n");
            sb.Append($"    {"secret",-7}{(stageUpStage.Song1.Secret ? "TRUE" : "FALSE")}\n");
            sb.Append("@SONG_2\n");
            sb.Append($"    {"id",-7 }{stageUpStage.Song2.EntryId}\n");
            sb.Append($"    {"secret",-7}{(stageUpStage.Song2.Secret ? "TRUE" : "FALSE")}\n");
            sb.Append("@SONG_3\n");
            sb.Append($"    {"id",-7 }{stageUpStage.Song3.EntryId}\n");
            sb.Append($"    {"secret",-7}{(stageUpStage.Song3.Secret ? "TRUE" : "FALSE")}\n");
        }

        return sb.ToString();
    }
}