using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Collections;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

public class ScenesManager : ILanguage
{
    private Dictionary<SceneIdentifier, Scene> appScenes;
    private SettingsModel settingsModel;
    public CancellationTokenSource cts { get; set; }

    private static ScenesManager _Instance;
    public static ScenesManager Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = new ScenesManager();
            return _Instance;
        }
    }
    private ScenesManager()
    {
        settingsModel = SettingsModel.Instance;
        InitScenes();
    }

    /// <summary>
    /// Create the scenes for the game.
    /// Each scene has text and audio
    /// </summary>
    private void InitScenes()
    {
        appScenes = new Dictionary<SceneIdentifier, Scene>();
        LocalizationManager lm = LocalizationManager.Instance;
        LanguageCode lc = settingsModel.Language;
        string audioLocation = $"Audio/{lc}";

        // TODO: Change the audio clip to GetLocalizedValue also, for support multiple languages

        // Stage 0
        appScenes.Add(SceneIdentifier.STAGE_0_ASKDETECT, new Scene(lm.GetLocalizedValue("stage_0_askdetect"), Resources.Load<AudioClip>($"{audioLocation}/stage_0_askdetect")));
        appScenes.Add(SceneIdentifier.STAGE_0_SCAN_INSTRUCTIONS, new Scene(lm.GetLocalizedValue("stage_0_scan_instructions"), Resources.Load<AudioClip>($"{audioLocation}/stage_0_scan_instructions")));
        appScenes.Add(SceneIdentifier.STAGE_0_DETECTED, new Scene(lm.GetLocalizedValue("stage_0_detected"), Resources.Load<AudioClip>($"{audioLocation}/stage_0_detected")));

        appScenes.Add(SceneIdentifier.GAME_INTRO, new Scene(lm.GetLocalizedValue("game_intro"), Resources.Load<AudioClip>($"{audioLocation}/game_intro")));

        // Stage 1
        appScenes.Add(SceneIdentifier.STAGE_1_INTRO, new Scene(lm.GetLocalizedValue("stage_1_intro"), Resources.Load<AudioClip>($"{audioLocation}/stage_1_intro")));
        appScenes.Add(SceneIdentifier.STAGE_1_END, new Scene(lm.GetLocalizedValue("stage_1_end"), Resources.Load<AudioClip>($"{audioLocation}/stage_1_end")));

        // Stage 2
        appScenes.Add(SceneIdentifier.STAGE_2_INTRO, new Scene(lm.GetLocalizedValue("stage_2_intro"), Resources.Load<AudioClip>($"{audioLocation}/stage_2_intro")));
        appScenes.Add(SceneIdentifier.STAGE_2_SALES_1, new Scene(lm.GetLocalizedValue("stage_2_sales_1"), Resources.Load<AudioClip>($"{audioLocation}/stage_2_sales")));
        appScenes.Add(SceneIdentifier.STAGE_2_SALES_2, new Scene(lm.GetLocalizedValue("stage_2_sales_2"), Resources.Load<AudioClip>($"{audioLocation}/stage_2_sales")));
        appScenes.Add(SceneIdentifier.STAGE_2_SALES_3, new Scene(lm.GetLocalizedValue("stage_2_sales_3"), Resources.Load<AudioClip>($"{audioLocation}/stage_2_sales")));
        appScenes.Add(SceneIdentifier.STAGE_2_SAILOR_SUCCESS_1, new Scene(lm.GetLocalizedValue("stage_2_sailor_success_1"), null));
        appScenes.Add(SceneIdentifier.STAGE_2_SAILOR_SUCCESS_2, new Scene(lm.GetLocalizedValue("stage_2_sailor_success_2"), null));
        appScenes.Add(SceneIdentifier.STAGE_2_NO_MONEY, new Scene(lm.GetLocalizedValue("stage_2_no_money"), null));
        appScenes.Add(SceneIdentifier.STAGE_2_END, new Scene(lm.GetLocalizedValue("stage_2_end"), Resources.Load<AudioClip>($"{audioLocation}/stage_2_end")));

        // Stage 3
        appScenes.Add(SceneIdentifier.STAGE_3_INTRO, new Scene(lm.GetLocalizedValue("stage_3_intro"), Resources.Load<AudioClip>($"{audioLocation}/stage_3_intro")));
        appScenes.Add(SceneIdentifier.STAGE_3_SELL_GOOD_1, new Scene(lm.GetLocalizedValue("stage_3_sell_good_1"), null));
        appScenes.Add(SceneIdentifier.STAGE_3_SELL_GOOD_2, new Scene(lm.GetLocalizedValue("stage_3_sell_good_2"), null));
        appScenes.Add(SceneIdentifier.STAGE_3_SELL_GOOD_3, new Scene(lm.GetLocalizedValue("stage_3_sell_good_3"), null));
        appScenes.Add(SceneIdentifier.STAGE_3_SELL_BAD_1, new Scene(lm.GetLocalizedValue("stage_3_sell_bad_1"), null));
        appScenes.Add(SceneIdentifier.STAGE_3_SELL_BAD_2, new Scene(lm.GetLocalizedValue("stage_3_sell_bad_2"), null));
        appScenes.Add(SceneIdentifier.STAGE_3_SUGGEST, new Scene(lm.GetLocalizedValue("stage_3_suggest"), null));
        appScenes.Add(SceneIdentifier.STAGE_3_END, new Scene(lm.GetLocalizedValue("stage_3_end"), Resources.Load<AudioClip>($"{audioLocation}/stage_3_end")));


        appScenes.Add(SceneIdentifier.GAME_END, new Scene(lm.GetLocalizedValue("game_end"), null));
    }

    public Scene GetScene(SceneIdentifier sceneIdentifier)
    {
        if (appScenes != null && appScenes.ContainsKey(sceneIdentifier))
            return appScenes[sceneIdentifier];
        return null;
    }


    public string GetTextOfSceneWithParams(SceneIdentifier sceneIdentifier, string[] arrayOfParams = null)
    {
         return ReplaceParams(GetTextOfScene(sceneIdentifier), arrayOfParams);

    }
    public string GetTextOfScene(SceneIdentifier sceneIdentifier)
    {
        Scene scene = GetScene(sceneIdentifier);
        if (scene != null) return scene.text;
        return string.Empty;
    }

    public AudioClip GetAudioOfScene(SceneIdentifier sceneIdentifier)
    {
        Scene scene = GetScene(sceneIdentifier);
        if (scene != null) return scene.audio;
        return null;
    }

    /// <summary>
    /// Playing entire scene based on sceneIdentifier
    /// Show text and play sound (if not muted)
    /// </summary>
    /// <param name="sceneIdentifier"></param>
    public async Task PlayScene(SceneIdentifier sceneIdentifier, string[] arrayOfParams = null, bool overrideWaitMore = false)
    {
        // show text on screen (like popup, define locations)
        string txt = ReplaceParams(GetTextOfScene(sceneIdentifier), arrayOfParams);
        AudioClip ac = GetAudioOfScene(sceneIdentifier);
        int audioClipLength = 1000 * 3; // default

        // play audio if not muted
        if (ac != null)
        {
            SoundManager.Instance.PlayAudio(ac);
            audioClipLength = (overrideWaitMore ? 0 : audioClipLength) + Mathf.RoundToInt(ac.length * 1000);  // ac.length is given in sec
        }

        if (txt != LocalizationManager.Instance.MissingTextString)
        {
            if (cts != null) cts.Cancel();
            cts = new CancellationTokenSource();
            PopupManager.Instance.Popup(PopupTypes.SUBTITLES, txt);
            try { await Task.Delay(audioClipLength, cts.Token);     /* AudioTimeDuration + 3 sec before hiding the popup */}
            catch { } // every cancellation throw exception, but its ok

            PopupManager.Instance.HidePopUp();
        }
    }


    public void FetchText()
    {
        // get data again 
        InitScenes();
    }


    /// <summary>
    /// Helping method to detect and replace text that need params
    /// </summary>
    /// <param name="text"></param>
    /// <param name="textParams"></param>
    /// <returns></returns>
    private string ReplaceParams(string text, string[] textParams)
    {
        // nothing to change..
        if (textParams == null || textParams.Length == 0)
        {
            return text;
        }

        // regex pattern
        string pattern = @"\bX(\d+)\b";  // i was set params to X1 X2 etc... it cannot be inside another word like "lidorX1". it has to be "lidor X1"

        // replace
        string result = Regex.Replace(text, pattern, match =>
        {
            int index;
            if (int.TryParse(match.Groups[1].Value, out index) && index > 0 && index <= textParams.Length)
            {
                return textParams[index - 1]; 
            }
            return match.Value; 
        });

        return result;
    }
}

public class Scene
{
    public string text { get; set; }
    public AudioClip audio { get; set; }
    public Scene(string text, AudioClip audio)
    {
        this.text = text;
        this.audio = audio;
    }
}


public enum SceneIdentifier
{
    // Ask to detect and detection
    STAGE_0_ASKDETECT,
    STAGE_0_SCAN_INSTRUCTIONS,
    STAGE_0_DETECTED,

    GAME_INTRO,

    // Stage 1
    STAGE_1_INTRO,
    STAGE_1_END,

    // Stage 2
    STAGE_2_INTRO,
    STAGE_2_SALES_1,
    STAGE_2_SALES_2,
    STAGE_2_SALES_3, // for each stand we have.
    STAGE_2_SAILOR_SUCCESS_1,
    STAGE_2_SAILOR_SUCCESS_2, // different messages 
    STAGE_2_NO_MONEY,
    STAGE_2_END,

    // Stage 3
    STAGE_3_INTRO,
    STAGE_3_SELL_GOOD_1,
    STAGE_3_SELL_GOOD_2,
    STAGE_3_SELL_GOOD_3,
    STAGE_3_SELL_BAD_1,
    STAGE_3_SELL_BAD_2,  // different messages
    STAGE_3_SUGGEST, // System suggests to sell more commoditie
    STAGE_3_END,

    GAME_END

}
