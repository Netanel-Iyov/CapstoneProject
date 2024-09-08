using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.Services.RemoteConfig;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public abstract class StageController : MonoBehaviour, ILanguage
{
    public static Dictionary<CommoditieType, string> typeToText;
    [SerializeField] protected GameObject progressBar;
    protected bool needHelp = true;
    protected void SetCommoditiesTypeToText()
    {
        typeToText = new();
        typeToText.Add(CommoditieType.WINE, LocalizationManager.Instance.GetLocalizedValue("commo_wine"));
        typeToText.Add(CommoditieType.COTTON, LocalizationManager.Instance.GetLocalizedValue("commo_cotton"));
        typeToText.Add(CommoditieType.OIL, LocalizationManager.Instance.GetLocalizedValue("commo_oil"));
        typeToText.Add(CommoditieType.ALMONDS, LocalizationManager.Instance.GetLocalizedValue("commo_almonds"));
        typeToText.Add(CommoditieType.MILK, LocalizationManager.Instance.GetLocalizedValue("commo_milk"));
    }

    protected IEnumerator StartTimerCheckForHelp(float timeInSec)
    {
        needHelp = true;
        yield return new WaitForSeconds(timeInSec);

        if (needHelp)
            PopupManager.Instance.BtnHelpOnClick();
    }
    public virtual void FetchText()
    {
        SetCommoditiesTypeToText();
    }


    /// <summary>
    /// Helper function
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    protected string GetLocalizedKeyByEnum(CommoditieType type)
    {
        switch (type)
        {
            case CommoditieType.WINE:
                return "commo_wine";
            case CommoditieType.COTTON:
                return "commo_cotton";
            case CommoditieType.OIL:
                return "commo_oil";
            case CommoditieType.ALMONDS:
                return "commo_almonds";
            case CommoditieType.MILK:
                return "commo_milk";
            default:
                return "notFound";
        }
    }

    public static bool IsProgressBar()
    {
        return SettingsModel.Instance.ActiveGamifications[GamificationMechanisms.ProgressBar];
        //bool status = true;
        //try
        //{
        //    status = RemoteConfigService.Instance.appConfig.GetBool("ProgressBarActive");
        //} catch { }
        //return status;
    }

    protected void SetProgressBar(int stageNum)
    {
        if (!IsProgressBar()) return;

        LanguageCode languageCode = SettingsModel.Instance.Language;
        string lngSuffix = languageCode == LanguageCode.EN ? "en" : "he";
        string spriteName = $"stage{stageNum}-{lngSuffix}";
        string spritePath = Path.Combine("Images", "ProgressBar", spriteName);

        // Load the sprite from Resources
        Image imageComponent = progressBar.GetComponent<Image>();
        if (imageComponent == null)
        {
            Debug.LogWarning("No Image component found on the GameObject.");
            return;
        }

        Sprite newSprite = Resources.Load<Sprite>(spritePath);

        if (newSprite != null)
        {
            // Assign the sprite to the GameObject's SpriteRenderer
            imageComponent.sprite = newSprite;
            imageComponent.SetAllDirty(); // force a sprite render update
        }
        else
        {
            Debug.LogError("Sprite not found at " + spritePath);
        }
    }

}