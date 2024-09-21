using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using Assets.Scripts.Helpers;
using System.Text.RegularExpressions;

public class LocalizationManager
{
    public TextAsset enJSONFile { get; set; }
    public TextAsset heJSONFile { get; set; }
    private static LocalizationManager _instance;
    public static LocalizationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new();
            }
            return _instance;
        }

    }
    private Dictionary<string, string> loadedData;
    public string MissingTextString { get; private set; } = "Localized text not found";  //default 
    public List<string> ExcludedListByText { get; set; } = new();

    /// <summary>
    /// private constructor
    /// </summary>
    private LocalizationManager()
    {
        // combobox text - always english
        ExcludedListByText.Add("hsilgnE");
        ExcludedListByText.Add("עברית");
    }


    /// <summary>
    /// Load language file (he.json, en.json) etc...
    /// </summary>
    /// <param name="fileName"></param>
    public void LoadLocalizedText(LanguageCode languageCode)
    {
        string dataAsJson = languageCode == LanguageCode.EN ? enJSONFile.text : heJSONFile.text;
        loadedData = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataAsJson);
        Debug.Log("Data loaded, dictionary contains: " + loadedData.Count + " entries");
    }

    /// <summary>
    /// Generic function. get the value by key 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetLocalizedValue(string key)
    {
        if (loadedData == null)
            LoadLocalizedText(LanguageCode.HE);  // default
        return loadedData.ContainsKey(key) ? loadedData[key] : MissingTextString;
    }


    /// <summary>
    /// Refresh all localized text in the UI
    /// </summary>
    public void UpdateDirectionForAllTMP(bool changeToRTL = false)
    {
        TextMeshProUGUI[] allTextComponents = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();

        // iterate over all textcomponents
        foreach (TextMeshProUGUI textComponent in allTextComponents)
        {

            if (IsNumericOrCurrency(textComponent.text))
            {
                textComponent.isRightToLeftText = false; // always left to right to numbers
                continue;
            }
            //if (int.TryParse(textComponent.text, out int number)) continue; // don't change location for numbers
            //if (textComponent.text == "hsilgnE" || textComponent.text == "Hebrew") continue; // consider to make a blacklist for this const in app
            //if (ExcludedListByText.Contains(textComponent.text)) continue;
            if (HasParentWithName(textComponent.gameObject, "LanguageSelection"))
            {
                textComponent.isRightToLeftText = false;
                continue;
            }


            //if (SettingsModel.Instance.Language == LanguageCode.HE && !CommonFunctions.IsHebrewText(textComponent.text))
            //    continue; // dont change if still english text in scene (hard coded)
            // covert direction
            textComponent.isRightToLeftText = changeToRTL;

            // important!!!! can't do UpdateAlignmentForText() because i want to keep text centered
            if (textComponent.alignment == TextAlignmentOptions.Left && changeToRTL)
                textComponent.alignment = TextAlignmentOptions.Right;

            else if (textComponent.alignment == TextAlignmentOptions.Right && !changeToRTL)
                textComponent.alignment = TextAlignmentOptions.Left;
        }
    }

    bool HasParentWithName(GameObject obj, string name)
    {
        Transform parent = obj.transform.parent;

        while (parent != null)
        {
            if (parent.name == name)
            {
                return true;
            }
            parent = parent.parent;
        }

        return false;
    }

    public void UpdateAlignmentForText(TextMeshProUGUI textObject)
    {
        LanguageCode languageCode = SettingsModel.Instance.Language;
        TextAlignmentOptions alignment = languageCode == LanguageCode.EN ? TextAlignmentOptions.Left : TextAlignmentOptions.Right;
        textObject.alignment = alignment;
    }

    private bool IsNumericOrCurrency(string text)
    {
        // if text is directly a number - dont change
        if (int.TryParse(text, out int number)) return true;

        string[] currencySymbols = { "$", "₪" };

        
        Match match = Regex.Match(text, @"^₪?\s*(\d+)\s*₪?$");
        return match.Success;

        //// if the text contains a number followed by a currency symbol 
        //foreach (var symbol in currencySymbols)
        //{
        //    if ((text.EndsWith(symbol)) && int.TryParse(text.Substring(0, text.Length - symbol.Length), out _))
        //    {
        //        return true;
        //    }
        //}

        //return false;
    }
}
