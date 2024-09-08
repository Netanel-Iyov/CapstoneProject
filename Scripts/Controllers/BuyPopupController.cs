using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

class BuyPopupController : MonoBehaviour, ILanguage
{
    [SerializeField] TMP_InputField amount;
    [SerializeField] TextMeshProUGUI totalPrice;
    [SerializeField] TextMeshProUGUI contentObject;
    [SerializeField] TextMeshProUGUI buyBtnText;
    public int TotalPrice
    {
        get
        {
            if (totalPrice == null) return 0;

            string pattern = @"(\d+)\s*₪";
            Match match = Regex.Match(totalPrice.text, pattern);

            return match.Success ? int.Parse(match.Groups[1].Value) : 0;
        }
        set
        {
            totalPrice.text = value.ToString() + "₪";
        }
    }
    public void OnEnable()
    {
        amount.text = "0"; // default
    }

    public string Content
    {
        get
        {
            return contentObject is null ? "" : contentObject.text;
        }
        set
        {
            contentObject.text = value;
        }
    }
    public void PlusBtnClicked()
    {
        int num = int.Parse((Regex.Match(amount.text, @"\d+")).ToString());
        amount.text = (++num).ToString();
        FindAnyObjectByType<Stage2Controller>().SetPriceToPopup(); // refresh
    }
    public void MinusBtnClicked()
    {
        int num = int.Parse((Regex.Match(amount.text, @"\d+")).ToString());
        if (num == 0) return;
        amount.text = (--num).ToString();
        FindAnyObjectByType<Stage2Controller>().SetPriceToPopup(); // refresh
    }

    public void FetchText()
    {
        buyBtnText.text = LocalizationManager.Instance.GetLocalizedValue("buy");
    }
}

