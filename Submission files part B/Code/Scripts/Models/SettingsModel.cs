using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class SettingsModel
{

    public string PlayerName
    {
        get
        {
            return PlayerPrefs.GetString("PlayerName", "שחקן1");
        }
        set
        {
            PlayerPrefs.SetString("PlayerName", value);
            PlayerPrefs.Save();
        }
    }

    public float AudioLevel
    {
        get
        {
            return PlayerPrefs.GetFloat("AudioLevel", 100f); // Default to 100 if not set
        }
        private set
        {
            PlayerPrefs.SetFloat("AudioLevel", value);
            PlayerPrefs.Save();
        }
    }

    public LanguageCode Language
    {
        get
        {
            return (LanguageCode)PlayerPrefs.GetInt("Language", (int)LanguageCode.HE); // Default to EN if not set
        }
        private set
        {
            PlayerPrefs.SetInt("Language", (int)value);
            PlayerPrefs.Save();
        }
    }

    public bool IsSoundMuted
    {
        get
        {
            return AudioLevel < 10f;
        }
    }

    
    public Dictionary<GamificationMechanisms, bool> ActiveGamifications;

    private static SettingsModel _instance;
    public static SettingsModel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SettingsModel();
            }
            return _instance;
        }
    }

    private SettingsModel()
    {
        AudioLevel = 100f;
        Language = LanguageCode.HE;
        InitGamificationMechanisms();
    }

    /// <summary>
    /// later changed values from Remote-Config
    /// </summary>
    private void InitGamificationMechanisms()
    {
        ActiveGamifications = new();

        ActiveGamifications.Add(GamificationMechanisms.Points, true);
        ActiveGamifications.Add(GamificationMechanisms.Leaderboard, true);
        ActiveGamifications.Add(GamificationMechanisms.Badges, true);
        ActiveGamifications.Add(GamificationMechanisms.ProgressBar, true);

    }
    /// <summary>
    ///  For access
    /// </summary>
    /// <param name="mechanism"></param>
    /// <returns></returns>
    public bool IsGamificationActive(GamificationMechanisms mechanism) => ActiveGamifications.ContainsKey(mechanism) && ActiveGamifications[mechanism];

    public void SwitchLanguage(LanguageCode languageCode)
    {
        Language = languageCode;
        LocalizationManager.Instance.LoadLocalizedText(languageCode);
        FetchTextForAllLanguageControllers();
        LocalizationManager.Instance.UpdateDirectionForAllTMP(languageCode == LanguageCode.HE);
    }

    private void FetchTextForAllLanguageControllers()
    {
        var languageControllers = Resources.FindObjectsOfTypeAll<MonoBehaviour>().OfType<ILanguage>();
        HashSet<ILanguage> uniqueInstances = new HashSet<ILanguage>(languageControllers);
        
        uniqueInstances.Add(ScenesManager.Instance); // not mono behaviour

        foreach (ILanguage controller in uniqueInstances)
        {
            try
            {
                controller.FetchText();
            }
            catch
            {
                continue;
            }
        }
    }


    public void ChangeAudioLevel(float level)
    {
        AudioLevel = level;
    }
}
