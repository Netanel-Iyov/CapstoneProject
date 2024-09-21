using Assets.Scripts.Helpers;
using System.Collections.Generic;
using UnityEngine;

public class Stage3Model : StageModel
{
    private const int ProfitPercentage = 60; // means user gets value 60% more of the base price
    private List<AudioClip> moneySounds = new();

    public override async void StartStage()
    {
        base.StartStage();
        if (!StageController.IsProgressBar()) PopupManager.Instance.Popup(PopupTypes.STAGE_TITLE, LocalizationManager.Instance.GetLocalizedValue("stage_3_title"));
        await ScenesManager.Instance.PlayScene(SceneIdentifier.STAGE_3_INTRO);
    }
    public void SetSounds(AudioClip ac1, AudioClip ac2)
    {
        moneySounds = new() { ac1, ac2 };
    }
    public bool IsValidDrop(Commoditie comm)
    {
        return (UserCurrentCommodities.ContainsKey(comm.Type) &&
             UserCurrentCommodities[comm.Type].IsValid);
    }
    public async void OnCommodityLoaded(CommoditieType type)
    {
        // TODO: The game logic here... 

        /* Trigger -User Drag N Drop some item
         * Check if commoditie is acceptable
         * Show text and audio
         * 
         */
        int sceneFromRandom;
        if (!UserCurrentCommodities.ContainsKey(type)) return;  // in case somehow user drops a commodity he does not actually has
        Commoditie loadedCommoditie = UserCurrentCommodities[type];
        if (loadedCommoditie != null && loadedCommoditie.IsValid)
        {
            GameManager.Instance.MoneyAmount += Mathf.RoundToInt(loadedCommoditie.Price * (1 + (ProfitPercentage / 100f)));
            // update amount / remove 
            if (UserCurrentCommodities[type].Amount > 1) UserCurrentCommodities[type].Amount -= 1;
            else UserCurrentCommodities.Remove(type);

            // random
            sceneFromRandom = Random.Range((int)SceneIdentifier.STAGE_3_SELL_GOOD_1, (int)SceneIdentifier.STAGE_3_SELL_GOOD_3+1);
            int randomSound = Random.Range(0, 2);
            await SoundManager.Instance.PlayAudioAsync(moneySounds[randomSound]);  // play random success sounds (money)

        }
        else
        {
            sceneFromRandom = Random.Range((int)SceneIdentifier.STAGE_3_SELL_BAD_1, (int)SceneIdentifier.STAGE_3_SELL_BAD_2);
        }
        await ScenesManager.Instance.PlayScene((SceneIdentifier)sceneFromRandom,
            new string[] { loadedCommoditie.GetColorizedString(StageController.typeToText[type]), Mathf.RoundToInt(loadedCommoditie.Price * (1 + (ProfitPercentage / 100f))).ToString() });

    }
    public override async void CompleteStage()
    {
        // TODO: add a lock here like in others stages

        GiveBadgesOnInvesting();

        Camera.main.GetComponent<BoxCollider>().enabled = false;
        await ScenesManager.Instance.PlayScene(SceneIdentifier.STAGE_3_END, new string[] { CommonFunctions.GetMoneyString() });
        base.CompleteStage();
    }

    private void GiveBadgesOnInvesting()
    {
        // give badge
        if (GetUserCurrentCommodities().Count == 0)
            Leaderboard.Instance.GiveBadge("PerfectTraderBadge");

        // I can also put the "Number of trades" here if we want...
        if (GameManager.Instance.MoneyAmount >= 1.5 * GameManager.StartMoney)
            Leaderboard.Instance.GiveBadge("SmartInvestorBadge");


    }

}
