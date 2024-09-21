using System.Collections.Generic;
using UnityEngine;

public class Commoditie
{
    public CommoditieType Type { get; private set; }
    public int Price { get; private set; }
    public bool IsValid { get; private set; }

    public int Amount { get; set; } = 0; // for buy and sell
    public Color ColorOfText { get; set; } = Color.black;
    public Commoditie(CommoditieType type, int price, bool isValid)
    {
        Type = type;
        Price = price;
        IsValid = isValid;
    }

    
    public override string ToString()
    {
        return GetColorizedString(Type.ToString());
    }


    public string GetColorizedString(string text)
    {
        string colorAsHex = ColorUtility.ToHtmlStringRGB(ColorOfText);
        return $"<color=#{colorAsHex}>{text}</color>";

    }
}

public enum CommoditieType
{
    WINE,
    COTTON,
    OIL,
    ALMONDS,
    MILK
}
