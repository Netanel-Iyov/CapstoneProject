using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.RemoteConfig;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HandleMenuClick : MonoBehaviour
{
    public GameObject ExtendedMenuPanel;
    public GameObject MinimizedMenuPanel;
    public GameObject MapScreenUI;
    public GameObject BasgesScreenUI;
    public GameObject SettingsScreenUI;
    public GameObject GameScreen;
    public GameObject ARStage;
    public GameObject ModelTargetObject;
    public GameObject environmentScreen;
    public GameObject endGameScreen;
    public GameObject explainPageScreen;
    public GameObject ScanExplainPageScreen;
    public TextMeshProUGUI playerNameInputText;
    public GameObject helpButton;
    public GameObject BadgesContainer;

    private MainController mainController;

    [SerializeField]
    private List<GameObject> stages;

    public void Start()
    {
        ModelTargetObject.SetActive(false);
        mainController = FindAnyObjectByType<MainController>();
        MinimizedMenuPanel.SetActive(false);
        MinimizedMenuPanel.GetComponentInChildren<Button>().interactable = false;
    }




    public HandleMenuClick()
    {
    }
    private void DisableHelpPopup()
    {
        try
        {
            PopupManager.Instance.DisableHelpPopup();
        }
        catch
        {
            // nothing to od
        }
    }
    /// <summary>
    /// Hamburger control
    /// </summary>
    public void ToggleMenuDisplayLevel()
    {
        if (ExtendedMenuPanel.activeSelf)
        {
            HideMenuLevel();
        }
        else if (MinimizedMenuPanel.activeSelf)
        {
            ShowMenuLevel();
        }
    }

    private void HideMenuLevel()
    {
        DisableHelpPopup();
        ExtendedMenuPanel.SetActive(false);
        MinimizedMenuPanel.SetActive(true);
    }

    private void ShowMenuLevel()
    {
        DisableHelpPopup();
        MinimizedMenuPanel.SetActive(false);
        ExtendedMenuPanel.SetActive(true);
        //try
        //{
        //    BadgesContainer.SetActive(RemoteConfigService.Instance.appConfig.GetBool("BadgesActive"));

        //}
        //catch
        //{
        //    BadgesContainer.SetActive(true); // default
        //}

    }

    public void Btn_StartGame()
    {
        DisableHelpPopup();
        GameManager.Instance.InsertStagesScreens(stages, ARStage, environmentScreen, endGameScreen);
        EnableDetection();
        GameManager.Instance.AskDetect();
        StartCoroutine(StartTimer());
    }

    private IEnumerator StartTimer()
    {
        // Wait for 5 seconds while the game remains playable
        yield return new WaitForSeconds(20f);

        // After 5 seconds, enable the pop-up
        if (!DisableEnableModelTarget.IsDetected) ScanExplainPageScreen.SetActive(true);
    }

    public static bool AllowDetectShip = false; // local variable to prevent a race from model target trigger action
    public void Btn_ShipDetected()
    {
        DisableHelpPopup();

        //if (DisableEnableModelTarget.TargetStatus != Vuforia.Status.NO_POSE && !AllowDetectShip) return;
        //AllowDetectShip = false;
        GameManager.Instance.ShipDetected();
    }


    public void ContinueFromWelcome()
    {
        DisableHelpPopup();

        // set player name
        SettingsModel.Instance.PlayerName = playerNameInputText.text;
        AnalyticsManager.Instance.StartAnalyticsDataCollection(); // mark as consent - start collecting data
        explainPageScreen.SetActive(true);
    }

    public void ContinueFromExplain()
    {
        MinimizedMenuPanel.SetActive(true);
        MinimizedMenuPanel.GetComponentInChildren<Button>().interactable = true;

        // start game

        DisableHelpPopup();

        HideMenuLevel();
        endGameScreen.SetActive(false); // in case the called made by the end of the game

        ModelTargetObject.SetActive(true);
        Btn_StartGame();
        helpButton.SetActive(true);
    }

    // user click on "play" button
    public void Btn_Home()
    {

        // reset all singletons 
        StageModel.ResetUserCommodities();
        DisableEnableModelTarget.IsDetected = false;
        AllowDetectShip = false;
        GameManager.Instance.ResetGame(); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);


    }

    public void EnableDetection()
    {
        AllowDetectShip = true; // Allow to detect the ship from now
        DisableEnableModelTarget.IsDetected = false;  // enable detection again
    }

    public void Btn_Map()
    {
        helpButton.SetActive(false);
        DisableHelpPopup();
        HideMenuLevel();
        endGameScreen.SetActive(false); // in case the called made by the end of the game

        AnalyticsManager.Instance.OnStageStart();
        MapScreenUI.SetActive(true);
    }

    public void Btn_Badges()
    {
        DisableHelpPopup();

        HideMenuLevel();
        endGameScreen.SetActive(false); // in case the called made by the end of the game

        BasgesScreenUI.SetActive(true);
    }

    public void Btn_Settings()
    {
        DisableHelpPopup();

        helpButton.SetActive(false);
        HideMenuLevel();
        endGameScreen.SetActive(false); // in case the called made by the end of the game

        SettingsScreenUI.SetActive(true);
    }
    
    public void Btn_Leaderboard()
    {
        DisableHelpPopup();
        helpButton.SetActive(false);
        HideMenuLevel();

        endGameScreen.SetActive(true);
    }

    public void Btn_Share()
    {
        DisableHelpPopup();

        helpButton.SetActive(false);
        string subject = LocalizationManager.Instance.GetLocalizedValue("share_subject");
        string body = LocalizationManager.Instance.GetLocalizedValue("share_body");

#if DEBUG
        return;
#endif
        //execute the below lines if being run on a Android device
#if UNITY_ANDROID
        //Reference of AndroidJavaClass class for intent
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        //Reference of AndroidJavaObject class for intent
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
        //call setAction method of the Intent object created
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        //set the type of sharing that is happening
        intentObject.Call<AndroidJavaObject>("setType", "text/plain");
        //add data to be passed to the other activity i.e., the data to be sent
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), subject);
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), body);
        //get the current activity
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        //start the activity by sending the intent data
        AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share Via");
        currentActivity.Call("startActivity", jChooser);

        // GiveBadge
        // TODO: Add popup?
        Leaderboard.Instance.GiveBadge("SocialPlayerBadge");

#endif
    }

    public void SkipScene()
    {
        PopupManager.Instance.SkipScene();

    }

    public void MuteAndUnmuteScene()
    {
        GameObject button = GameObject.Find("MuteButton");
        Sprite soundOffOnSprite;

        bool isSoundMuted = SettingsModel.Instance.AudioLevel == 0;

        if (!isSoundMuted)
        {
            SettingsModel.Instance.ChangeAudioLevel(0f);
            soundOffOnSprite = Resources.Load<Sprite>("images/soundOff");
        }
        else
        {
            SettingsModel.Instance.ChangeAudioLevel(100f);
            soundOffOnSprite = Resources.Load<Sprite>("images/soundOn");
        }
        button.GetComponent<Image>().sprite = soundOffOnSprite;
        SoundManager.Instance.UpdateCurrentPlayingAudioVolume();

    }
}
