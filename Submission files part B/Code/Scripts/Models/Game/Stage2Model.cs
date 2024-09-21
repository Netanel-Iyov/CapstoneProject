using Assets.Scripts.Helpers;
using UnityEngine;

public class Stage2Model : StageModel
{
    private AudioClip buySound;

    public override async void StartStage()
    {
        base.StartStage();
        if (!StageController.IsProgressBar()) PopupManager.Instance.Popup(PopupTypes.STAGE_TITLE, LocalizationManager.Instance.GetLocalizedValue("stage_2_title"));
        await ScenesManager.Instance.PlayScene(SceneIdentifier.STAGE_2_INTRO);
    }
    public void SetSounds(AudioClip ac) => buySound = ac;
    public int GetPriceOfCommodity(CommoditieType type, int amount)
    {
        var commoditie = GetCommoditieByType(type);
        return commoditie == null ? int.MaxValue : (commoditie.Price * amount); // if commodity not found  - return high price to not allow this

    }
    public void BuyCommoditie(CommoditieType type, int amount)
    {
        // uppdate money or other sspecific logic
        var commoditie = GetCommoditieByType(type);
        if (commoditie != null && (GameManager.Instance.MoneyAmount - (commoditie.Price * amount) >= 0))
            BuySuccess(commoditie, amount);
        else
            BuyFailed();
    }

    private async void BuySuccess(Commoditie commoditie, int amount)
    {
        if (amount <= 0) return;
        GameManager.Instance.MoneyAmount -= (commoditie.Price * amount);
        // add or update
        if (!UserCurrentCommodities.ContainsKey(commoditie.Type))
            UserCurrentCommodities.Add(commoditie.Type, commoditie); // save the commoditie the user bought

        UserCurrentCommodities[commoditie.Type].Amount += amount;

        await SoundManager.Instance.PlayAudioAsync(buySound);

        int randomSuccess = Random.Range((int)SceneIdentifier.STAGE_2_SAILOR_SUCCESS_1, (int)SceneIdentifier.STAGE_2_SAILOR_SUCCESS_2);
        await ScenesManager.Instance.PlayScene((SceneIdentifier)randomSuccess, new string[] { CommonFunctions.GetMoneyString() });
    }

    private async void BuyFailed()
    {
        await ScenesManager.Instance.PlayScene(SceneIdentifier.STAGE_2_NO_MONEY);
    }
    public override async void CompleteStage()
    {
        await ScenesManager.Instance.PlayScene(SceneIdentifier.STAGE_2_END);
        base.CompleteStage();
    }
}
