using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour, ILanguage
{
    [SerializeField] private TextMeshProUGUI lblHome;
    [SerializeField] private TextMeshProUGUI lblMap;
    [SerializeField] private TextMeshProUGUI lblSettings;
    [SerializeField] private TextMeshProUGUI lblShare;
    [SerializeField] private TextMeshProUGUI lblBadges;
    [SerializeField] private TextMeshProUGUI lblLeaderboard;
    [SerializeField] private TextAsset enJSONFile;
    [SerializeField] private TextAsset heJSONFile;
    public static MainController Instance;
    public ScenesManager sceneManager;

    void Start()
    {
        InitLanguage();
        InitAnalytics();
        InitRemoteConfig();
        InitGeneral();
    }

    private void InitGeneral()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        sceneManager = ScenesManager.Instance;
        SoundManager soundManager = SoundManager.Instance;
        Instance = this;
    }
    private void InitLanguage()
    {
        // Set localization manager json files at application start up
        LocalizationManager.Instance.enJSONFile = enJSONFile;
        LocalizationManager.Instance.heJSONFile = heJSONFile;
        SettingsModel.Instance.SwitchLanguage(LanguageCode.HE);

    }
    private async void InitAnalytics()
    {
        await AnalyticsManager.Instance.InitializeUnityServices();
    }

    private void InitRemoteConfig()
    {
        var temp = RemoteConfigManager.Instance;
    }

    #region GUI

    public void FetchText()
    {
        string basicString = "menu_";

        lblHome.text = LocalizationManager.Instance.GetLocalizedValue(basicString + "home");
        lblMap.text = LocalizationManager.Instance.GetLocalizedValue(basicString + "map");
        lblSettings.text = LocalizationManager.Instance.GetLocalizedValue(basicString + "settings");
        lblShare.text = LocalizationManager.Instance.GetLocalizedValue(basicString + "share");
        lblBadges.text = LocalizationManager.Instance.GetLocalizedValue(basicString + "badges");
        lblLeaderboard.text = LocalizationManager.Instance.GetLocalizedValue(basicString + "leaderboard");
    }


    #endregion




}
