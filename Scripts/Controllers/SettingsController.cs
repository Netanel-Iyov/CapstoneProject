using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
public enum LanguageCode
{
    EN,
    HE
}


public enum GamificationMechanisms
{
    Points,
    Leaderboard,
    Badges,
    ProgressBar
}
public class SettingsController : MonoBehaviour, ILanguage
{
    private SettingsModel settingsModel;
    private Slider volumeSlider;
    private TextMeshProUGUI volumeText;
    private TMP_Dropdown languageDropdown;
    private bool isInitialize = true;
    [SerializeField] TextMeshProUGUI settingsHeader;
    [SerializeField] TextMeshProUGUI changeLang;
    [SerializeField] TextMeshProUGUI changeVol;


    public void Start()
    {
        settingsModel = SettingsModel.Instance;
        InitUIComponents();
    }

    private void InitUIComponents()
    {
        // Volume Slider
        GameObject sliderObject = GameObject.Find("VolumeSlider");
        volumeSlider = sliderObject.GetComponent<Slider>();
        volumeSlider.minValue = 0;
        volumeSlider.maxValue = 100;

        // Volume Text
        GameObject soundTextObject = GameObject.Find("SoundCurrentVolText");
        volumeText = soundTextObject.GetComponent<TextMeshProUGUI>();
        volumeSlider.value = settingsModel.AudioLevel;

        // Language Dropdown
        GameObject comboboxLanguage = GameObject.Find("LanguageSelection");
        languageDropdown = comboboxLanguage.GetComponent<TMP_Dropdown>();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>
        {
            new TMP_Dropdown.OptionData("English"),
            new TMP_Dropdown.OptionData("תירבע")
        };
        
        languageDropdown.options = options;
        languageDropdown.value = (int)settingsModel.Language;

        isInitialize = false;
    }

    public void Btn_Close()
    {
        gameObject.SetActive(false);
    }


    public void DropDownValChanged()
    {
        if (isInitialize) return;
        LanguageCode languageCode = (LanguageCode)languageDropdown.value;
        settingsModel.SwitchLanguage(languageCode);
    }


    public void OnSliderValChanged()
    {
        volumeText.text = volumeSlider.value.ToString();
        settingsModel.ChangeAudioLevel(volumeSlider.value);
    }
    public void PlayBeepTestSound()
    {
        SoundManager.Instance.PlayBeep(settingsModel.AudioLevel / 100);
    }

    public void FetchText()
    {
        settingsHeader.text = LocalizationManager.Instance.GetLocalizedValue("settings_title");
        changeVol.text = LocalizationManager.Instance.GetLocalizedValue("settings_change_vol");
        changeLang.text = LocalizationManager.Instance.GetLocalizedValue("settings_change_lang");
    }
}

