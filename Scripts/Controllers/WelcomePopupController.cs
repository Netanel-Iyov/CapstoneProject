using Assets.Scripts.Helpers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WelcomePopupController : MonoBehaviour, ILanguage
{
    [SerializeField] TextMeshProUGUI welcomeText;
    [SerializeField] TextMeshProUGUI consentText;
    [SerializeField] TextMeshProUGUI namePlaceholder;
    [SerializeField] TextMeshProUGUI btn_text;
    [SerializeField] TextMeshProUGUI changeLangBtnText;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button continueBtn;
    private void OnEnable()
    {
        SetButtonInteractable();
    }
    public void FetchText()
    {
        welcomeText.text = LocalizationManager.Instance.GetLocalizedValue("welcomepopup_maintext");
        consentText.text = LocalizationManager.Instance.GetLocalizedValue("welcomepopup_consenttext");
        namePlaceholder.text = LocalizationManager.Instance.GetLocalizedValue("welcomepopup_nameplaceholder");
        btn_text.text = LocalizationManager.Instance.GetLocalizedValue("continue");
        changeLangBtnText.text = LocalizationManager.Instance.GetLocalizedValue("mainscreen_changeLanguage");
    }
    public void SetButtonInteractable()
    {
        // disable continue button if name is empty
        continueBtn.interactable = inputField.textComponent.text.ToString().Length > 1;
    }
    public void SetTextDirection()
    {

        SetButtonInteractable();
        if (CommonFunctions.IsHebrewText(inputField.textComponent.text))
        {
            inputField.textComponent.alignment = TextAlignmentOptions.Right;
            inputField.textComponent.isRightToLeftText = true;

        }
        else
        {
            inputField.textComponent.isRightToLeftText = false;

            inputField.textComponent.alignment = TextAlignmentOptions.Left;
        }
    }

    public void ToggleLanguage()
    {
        var currentLang = SettingsModel.Instance.Language;
        SettingsModel.Instance.SwitchLanguage(currentLang == LanguageCode.EN ? LanguageCode.HE: LanguageCode.EN);
    }

}
