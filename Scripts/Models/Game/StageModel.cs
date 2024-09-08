using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class StageModel
{
    protected List<Commoditie> Commodities;
    protected static Dictionary<CommoditieType, Commoditie> UserCurrentCommodities;
    private bool completeStageInProgress = false;

    public StageModel()
    {
        Commodities = new();
        SetCommoditiesAndPrices();
        if (UserCurrentCommodities == null)
            UserCurrentCommodities = new();
    }

    public static void ResetUserCommodities()
    {
        UserCurrentCommodities = null;
    }
    private void SetCommoditiesAndPrices()
    {
        Commodities.Add(new Commoditie(CommoditieType.WINE, 30, true));
        Commodities.Add(new Commoditie(CommoditieType.COTTON, 40, false));
        Commodities.Add(new Commoditie(CommoditieType.OIL, 88, true));
        Commodities.Add(new Commoditie(CommoditieType.ALMONDS, 50, true));
        Commodities.Add(new Commoditie(CommoditieType.MILK, 25, false));
    }

    public virtual void StartStage()
    {
        completeStageInProgress = false;
    }


    public virtual void CompleteStage()
    {
        if (completeStageInProgress) return;
        completeStageInProgress = true;
        PopupManager.Instance.HidePopUp();
        GameManager.Instance.OnStageComplete();
    }

    protected Commoditie GetCommoditieByType(CommoditieType type)
    {
        foreach (Commoditie com in Commodities)
            if (com.Type == type) return com;
        return null;
    }

    public static List<Commoditie> GetUserCurrentCommodities()
    {
        return UserCurrentCommodities.Values.ToList();
    }
}

