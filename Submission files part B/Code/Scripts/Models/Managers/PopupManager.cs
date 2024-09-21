using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

/// <summary>
/// The PopupController class manages the creation, display, and removal of popup UI elements.
/// It maintains a list of popup models and their instances, provides methods to show and hide popups,
/// and allows setting text on popups. The class uses a singleton pattern to ensure a single instance.
/// </summary>
public class PopupManager : MonoBehaviour, ILanguage
{
    [SerializeField] List<GameObject> popupModelList;
    private static PopupManager _instance;
    private static GameObject mainCanvas;
    private static List<GameObject> popupInstancesList;
    public GameObject SkipButton;
    public GameObject MuteButton;
    private static Dictionary<PopupTypes, GameObject> PrefabByType { get; set; }
    private GameObject currentSubtitlePopup;
    private GameObject helpPopUp;
    [SerializeField] GameObject ScanExplanationPopup;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// Initializes the popup instances list, sets the singleton instance, and finds the main canvas.
    /// </summary>
    public void Awake()
    {
        popupInstancesList = new List<GameObject>();
        _instance = this;
        mainCanvas = GameObject.Find("MainCanvas");
        InitDictOfPopups();
    }


    /// <summary>
    /// Gets the singleton instance of the PopupManager.
    /// </summary>
    public static PopupManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void InitDictOfPopups()
    {
        if (popupModelList.Count < 3) return;
        PrefabByType = new();
        PrefabByType.Add(PopupTypes.STAGE_TITLE, popupModelList[0]);
        PrefabByType.Add(PopupTypes.CENTERED, popupModelList[1]);
        PrefabByType.Add(PopupTypes.SUBTITLES, popupModelList[2]);
    }

    /// <summary>
    /// Wrapper 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="text"></param>
    public void Popup(PopupTypes type, string text)
    {
        if (type == PopupTypes.SUBTITLES)
        {

            if (currentSubtitlePopup != null)
            {
                Destroy(currentSubtitlePopup);
                currentSubtitlePopup = null;
            }
            // show skip button
            SkipButton.SetActive(true);
            MuteButton.SetActive(true);
        }
        GameObject go = ShowPanel(type);
        SetText(go, text);
    }

    /// <summary>
    /// Shows a popup panel based on the popupId. Optionally, adjusts the position relative to the entire screen.
    /// </summary>
    /// <param name="popupId">The ID of the popup to show.</param>
    /// <param name="relativeToEntireScreen">If true, moves the popup 75 units to the left.</param>
    /// <returns>The instantiated popup GameObject, or null if the popupId is not found.</returns>
    public GameObject ShowPanel(PopupTypes type)
    {
        GameObject newPopupInstance;
        GameObject popupModel = PrefabByType[type];

        if (popupModel == null)
        {
            Debug.LogWarning($"{type.ToString()} was not found in model list, could not instantiate it.");
            newPopupInstance = null;
        }
        else
        {
            newPopupInstance = Instantiate(popupModel, mainCanvas.transform);

            if (type == PopupTypes.SUBTITLES)
                currentSubtitlePopup = newPopupInstance;
            else
                popupInstancesList.Add(newPopupInstance);
        }

        return newPopupInstance;
    }

    /// <summary>
    /// Sets the text of the first TextMeshProUGUI component found in the popup.
    /// If there is no TextMeshProUGUI component in the popup, do nothing
    /// </summary>
    /// <param name="popup">The popup GameObject.</param>
    /// <param name="text">The text to set.</param>
    public void SetText(GameObject popup, string text)
    {
        TextMeshProUGUI textComponent = null;

        foreach (Transform child in popup.transform)
        {
            textComponent = child.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
                break; // Exit loop after finding the first one

        }

        if (textComponent == null)
        {
            Debug.Log($"No text component found in {popup.name}.");
            return;
        }

        textComponent.text = text;
    }

    /// <summary>
    /// Hides a specific popup or the most recently shown popup if no popup is specified.
    /// </summary>
    /// <param name="popup">The popup GameObject to hide. If null, the most recently shown popup will be hidden.</param>
    public void HidePopUp(GameObject popup = null, bool toHideSubtitles = true)
    {
        if (popup != null)
        {
            Destroy(popup);
            return;
        }

        if (toHideSubtitles && currentSubtitlePopup != null)
        {
            Destroy(currentSubtitlePopup);
            currentSubtitlePopup = null;
            SkipButton.SetActive(false);
            MuteButton.SetActive(false);
        }
        else
        {
            if (popupInstancesList.Count == 0) return;

            GameObject toDestroy = popupInstancesList[popupInstancesList.Count - 1];
            popupInstancesList.RemoveAt(popupInstancesList.Count - 1);
            Destroy(toDestroy);
        }

    }
    public void SkipScene()
    {
        SoundManager.Instance.StopAudio();
        SkipButton.SetActive(false);
        MuteButton.SetActive(false);

        // this MUST be the last line here
        ScenesManager.Instance.cts.Cancel(); // will call "hide popup"

    }

    public void FetchText()
    {
        SkipButton.GetComponentInChildren<TextMeshProUGUI>().text = LocalizationManager.Instance.GetLocalizedValue("skip");
        setHelpPopupText();
    }

    public void setHelpPopupText()
    {
        if (!helpPopUp) return;

        int currentStage = GameManager.Instance.GetCurrentStage();
        string headKey = $"help_head";
        string bodyKey = $"stage_{currentStage}_help";
        TextMeshProUGUI[] textMeshProComponents = helpPopUp.GetComponentsInChildren<TextMeshProUGUI>();

        textMeshProComponents[0].text = LocalizationManager.Instance.GetLocalizedValue(headKey);
        textMeshProComponents[1].text = LocalizationManager.Instance.GetLocalizedValue(bodyKey);
    }

    public void BtnHelpOnClick()
    {
        int currentStage = GameManager.Instance.GetCurrentStage();
        if (currentStage == 0)
        {
            ScanExplanationPopup.SetActive(true);
            return;
        }

        helpPopUp = PrefabByType[PopupTypes.CENTERED];

        if (helpPopUp.activeSelf) return;
        helpPopUp.SetActive(true);

        setHelpPopupText();
        // Text(helpPopUp, $"Stage {currentStage} - Help");
    }

    public void DisableHelpPopup()
    {
        helpPopUp = PrefabByType[PopupTypes.CENTERED];

        if (helpPopUp.activeSelf)
            helpPopUp.SetActive(false);
    }
}

public enum PopupTypes
{
    STAGE_TITLE, CENTERED, SUBTITLES
}
