using Assets.Scripts.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

class Stage2Controller : StageController
{
    protected Stage2Model currentModel;
    [SerializeField] private TextMeshProUGUI moneyLbl;
    [SerializeField] private TextMeshProUGUI commoditiesPanelTitle;
    [SerializeField] private TextMeshProUGUI commoditiesBackpackLbl;
    [SerializeField] private TextMeshProUGUI endStageText;
    [SerializeField] GameObject buyPopupPrefab;
    [SerializeField] private GameObject fence;
    [SerializeField] private AudioClip buySound;

    [SerializeField] List<GameObject> stands; // the stands objects

    // from BuyPopup prefab
    [SerializeField]
    private TMP_InputField amountInputField;

    private bool isBuyPopupOpen = false;
    private int StandNum { get; set; }
    private Dictionary<int, CommoditieType> CommoditieInStand;


    public void OnEnable()
    {
        if (currentModel == null)
            currentModel = new Stage2Model();
        Camera.main.GetComponent<BoxCollider>().enabled = true;
        currentModel.StartStage();
        currentModel.SetSounds(buySound);
        stands.ForEach(stand => stand.SetActive(true));
        SetCommoditiesToStands();
        FetchText();
        fence.SetActive(true);
        SetProgressBar(2);
        StartCoroutine(StartTimerCheckForHelp(20f));


    }
    public void OnDisable()
    {
        try
        {
            var component = Camera.main.GetComponent<Collider>();
            if (component != null) component.enabled = false;
            if (stands != null)
                stands.ForEach(stand => stand.SetActive(false));

        }
        catch { }

    }
    /// <summary>
    /// Initialize dict of <standNum, commoditieToSell>
    /// </summary>
    /// 
    private void SetCommoditiesToStands()
    {
        CommoditieInStand = new();
        CommoditieInStand.Add(1, CommoditieType.WINE);
        CommoditieInStand.Add(2, CommoditieType.COTTON);
        CommoditieInStand.Add(3, CommoditieType.OIL);
        CommoditieInStand.Add(4, CommoditieType.ALMONDS);
        CommoditieInStand.Add(5, CommoditieType.MILK);
    }

    public void OpenBuyPopup(int standNum)
    {
        // open when nearby
        if (standNum == 0 || isBuyPopupOpen) return;
        StandNum = standNum;
        buyPopupPrefab.SetActive(true);
        SetPriceToPopup();
        PlayCorrespondingScene(standNum);

        isBuyPopupOpen = true;
        needHelp = false;
    }
    public void SetPriceToPopup()
    {
        // get from model
        int amount = int.TryParse(amountInputField.text, out int res) ? res : 0;
        int price = currentModel.GetPriceOfCommodity(CommoditieInStand[StandNum], amount);

        // set to popup
        buyPopupPrefab.GetComponent<BuyPopupController>().TotalPrice = price;

    }
    public void CloseBuyPopup()
    {
        // close from walking away
        if (!isBuyPopupOpen) return;
        buyPopupPrefab.SetActive(false);
        isBuyPopupOpen = false;
    }

    /// <summary>
    /// Called when user buy from some stand ? 
    /// </summary>
    public void OnClickBuy()
    {
        var amount = amountInputField.text;
        // TODO: check if valid...

        currentModel.BuyCommoditie(CommoditieInStand[StandNum], int.TryParse(amount, out int res) ? res : 0);

        // update money on GUI
        UpdateMoneyTxt();

        // update right panel of current commodities 
        RefreshCommoditiesTextList();
    }
   

    /// <summary>
    /// update the right panel of commodities and amounts
    /// </summary>
    private void RefreshCommoditiesTextList()
    {
        string txt = "";
        foreach (Commoditie comm in StageModel.GetUserCurrentCommodities())
        {
            txt += LocalizationManager.Instance.GetLocalizedValue(GetLocalizedKeyByEnum(comm.Type));
            txt += " - " + comm.Amount;
            txt += "\n";
        }

        commoditiesBackpackLbl.text = txt;
    }


    private void UpdateMoneyTxt()
    {
        moneyLbl.text = GameManager.Instance.MoneyAmount + "₪";
    }
    /// <summary>
    /// When user wants to end the stage
    /// </summary>
    public void UserEndStage()
    {
        currentModel.CompleteStage();
    }

    /// <summary>
    /// Called when user infront of stand (meter away?)
    /// </summary>
    /// <param name="standNum"></param>
    private async void PlayCorrespondingScene(int standNum)
    {
        SceneIdentifier sceneToPlay;
        string content = string.Empty;

        switch (standNum)
        {
            case 1:
            case 4:
                sceneToPlay = SceneIdentifier.STAGE_2_SALES_1;
                content = ScenesManager.Instance.GetTextOfSceneWithParams(SceneIdentifier.STAGE_2_SALES_1, new string[] { typeToText[CommoditieInStand[standNum]] });
                break;
            case 2:
            case 5:
                sceneToPlay = SceneIdentifier.STAGE_2_SALES_2;
                content = ScenesManager.Instance.GetTextOfSceneWithParams(SceneIdentifier.STAGE_2_SALES_2, new string[] { typeToText[CommoditieInStand[standNum]] });
                break;
            case 3:
                sceneToPlay = SceneIdentifier.STAGE_2_SALES_3;
                content = ScenesManager.Instance.GetTextOfSceneWithParams(SceneIdentifier.STAGE_2_SALES_3, new string[] { typeToText[CommoditieInStand[standNum]] });
                break;

            default:
                return;
        }

        //await ScenesManager.Instance.PlayScene(sceneToPlay);
        SoundManager.Instance.PlayAudio(ScenesManager.Instance.GetAudioOfScene(sceneToPlay));
        buyPopupPrefab.GetComponent<BuyPopupController>().Content = content;
    }


    /// <summary>
    /// Each stand object MUST have the EXACT name for labels
    /// </summary>
    private void SetStands3DNames()
    {
        int i = 1;
        foreach (GameObject stand in stands)
        {
            CommoditieType commoditieForStand = CommoditieInStand[i++];
            List<TextMeshPro> textLst = new();
            textLst.Add(stand.transform.Find("frontText").GetComponent<TextMeshPro>());
            textLst.Add(stand.transform.Find("backText").GetComponent<TextMeshPro>());
            textLst.Add(stand.transform.Find("rightText").GetComponent<TextMeshPro>());
            textLst.Add(stand.transform.Find("leftText").GetComponent<TextMeshPro>());
            SetSingleStand(textLst, commoditieForStand);

        }
    }

    private void SetSingleStand(List<TextMeshPro> lst, CommoditieType type)
    {
        foreach (TextMeshPro text3D in lst)
        {
            text3D.text = typeToText[type];
            text3D.isRightToLeftText = SettingsModel.Instance.Language == LanguageCode.HE; // if hebrew - so change to RTL
        }
    }


    public override void FetchText()
    {
        base.FetchText();
        SetStands3DNames();
        UpdateMoneyTxt();
        commoditiesPanelTitle.text = LocalizationManager.Instance.GetLocalizedValue("stage_2_panel_title");
        RefreshCommoditiesTextList();
        endStageText.text = LocalizationManager.Instance.GetLocalizedValue("stage_2_end_btn");

    }


}

