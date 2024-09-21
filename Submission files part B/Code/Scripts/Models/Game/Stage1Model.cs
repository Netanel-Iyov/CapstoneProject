using System.Collections.Generic;
using UnityEngine;

public class Stage1Model : StageModel
{
    private int commoditiesUnloaded = 0;
    private const int MAX_COMMODITIES = 5;


    public override async void StartStage()
    {
        base.StartStage();
        if (!StageController.IsProgressBar()) PopupManager.Instance.Popup(PopupTypes.STAGE_TITLE, LocalizationManager.Instance.GetLocalizedValue("stage_1_title"));
        await ScenesManager.Instance.PlayScene(SceneIdentifier.STAGE_1_INTRO);
    }

    public string GetProgress() => $"{commoditiesUnloaded}/{MAX_COMMODITIES}";
    /// <summary>
    /// Function should be called when user drag n drop a commoditie
    /// </summary>
    public void OnCommodityUnloaded()
    {
        commoditiesUnloaded++;
        if (commoditiesUnloaded >= MAX_COMMODITIES && !completeStageInProgress)
            CompleteStage();
    }
    private bool completeStageInProgress = false;
    public override async void CompleteStage()
    {
        completeStageInProgress = true;
        List<Commoditie> commodities = new();
        commodities.Add(new Commoditie(CommoditieType.WINE, 0, true) { ColorOfText = Color.red });
        commodities.Add(new Commoditie(CommoditieType.ALMONDS, 0, true) { ColorOfText = Color.green });

        string[] paramToScene =
        {
            commodities[0].GetColorizedString(StageController.typeToText[commodities[0].Type]),
            commodities[1].GetColorizedString(StageController.typeToText[commodities[1].Type])
        };
        await ScenesManager.Instance.PlayScene(SceneIdentifier.STAGE_1_END, paramToScene);  // simulate that the user help to unload the wine and Olive oil amphoras
        base.CompleteStage();
    }
}
