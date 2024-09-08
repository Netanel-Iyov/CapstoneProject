using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

public class BadgesController : MonoBehaviour, ILanguage
{
    [SerializeField] TextMeshProUGUI Title;
    [SerializeField] TextMeshProUGUI TitleDescription;
    [SerializeField] GameObject FirstGameBadge;
    [SerializeField] GameObject ConsistentPlayerBadge;
    [SerializeField] GameObject PersonalRecordBadge;
    [SerializeField] GameObject PerfectTraderBadge;
    [SerializeField] GameObject HighScoreBadge;
    [SerializeField] GameObject SmartInvestorBadge;
    [SerializeField] GameObject SocialPlayerBadge;
    //[SerializeField] TextAsset badgesJSONFile;

    private Dictionary<string, bool> badgesData;  // for text
    Dictionary<Badge, BadgeInGame> badges = new();
    BadgesModel badgesModel;

    private void Start()
    {
        // set relevant badges active
        badgesModel = new BadgesModel();
        //badgesData = badgesModel.LoadBadges();
        SetBadges();
        LoadAndSetBadges();

        //// for debug
        //SaveBadge(badges[Badge.FirstGame].GameObjectC.name, true);
        //SaveBadge(badges[Badge.ConsistentPlayer].GameObjectC.name, true);
        //LoadAndSetBadges();
    }

    private void OnEnable()
    {
        if (badgesData != null)
            LoadAndSetBadges();
    }
    public void LoadAndSetBadges()
    {
        badgesData = badgesModel.LoadBadges();

        //SetBadgesActive();
        SetRelevantBadgesActive();
        InitTextToBadge();
    }
    private void InitTextToBadge()
    {
        FirstGameBadge.GetComponentInChildren<TextMeshProUGUI>().text = badges[Badge.FirstGame].ShortDescription;
        ConsistentPlayerBadge.GetComponentInChildren<TextMeshProUGUI>().text = badges[Badge.ConsistentPlayer].ShortDescription;
        PersonalRecordBadge.GetComponentInChildren<TextMeshProUGUI>().text = badges[Badge.NewPersonalRecord].ShortDescription;
        PerfectTraderBadge.GetComponentInChildren<TextMeshProUGUI>().text = badges[Badge.PerfectTrade].ShortDescription;
        HighScoreBadge.GetComponentInChildren<TextMeshProUGUI>().text = badges[Badge.TopScore].ShortDescription;
        SmartInvestorBadge.GetComponentInChildren<TextMeshProUGUI>().text = badges[Badge.SmartInvestor].ShortDescription;
        SocialPlayerBadge.GetComponentInChildren<TextMeshProUGUI>().text = badges[Badge.SocialPlayer].ShortDescription;

    }



    // show tooltip / text about the badge  -- triggered from click on a badge
    public void ShowDescription(string badgeName)
    {
        switch (badgeName.ToLower())
        {
            case "firstgame":
                FirstGameBadge.GetComponentInChildren<TextMeshProUGUI>().text = badges[Badge.FirstGame].GetRelevantText();
                break;
            case "top":
                HighScoreBadge.GetComponentInChildren<TextMeshProUGUI>().text = badges[Badge.TopScore].GetRelevantText();
                break;
            case "consist":
                ConsistentPlayerBadge.GetComponentInChildren<TextMeshProUGUI>().text = badges[Badge.ConsistentPlayer].GetRelevantText();
                break;
            case "newrecord":
                PersonalRecordBadge.GetComponentInChildren<TextMeshProUGUI>().text = badges[Badge.NewPersonalRecord].GetRelevantText();
                break;
            case "social":
                SocialPlayerBadge.GetComponentInChildren<TextMeshProUGUI>().text = badges[Badge.SocialPlayer].GetRelevantText();
                break;
            case "smartinvestor":
                SmartInvestorBadge.GetComponentInChildren<TextMeshProUGUI>().text = badges[Badge.SmartInvestor].GetRelevantText();
                break;
            case "perfect":
                PerfectTraderBadge.GetComponentInChildren<TextMeshProUGUI>().text = badges[Badge.PerfectTrade].GetRelevantText();
                break;
            default:
                break;
        }
    }
    public void FetchText()
    {
        Title.text = LocalizationManager.Instance.GetLocalizedValue("badges_title");
        TitleDescription.text = LocalizationManager.Instance.GetLocalizedValue("badges_title_description");

        // delete those...
        // using language from .json
        foreach (var badge in badges.Values)
            badge.RefreshStrings();

        LoadAndSetBadges();

    }

    /// <summary>
    /// Set state for badges. If player didn't get the badge - there is a lock panel and the button is disabled.
    /// /// </summary>
    private void SetRelevantBadgesActive()
    {
        foreach (var badge in badges.Values)
        {
            var playerHasAchivement = badgesData[badge.GameObjectC.name];
            badge.GameObjectC.transform.Find("LockPanel").gameObject.SetActive(!playerHasAchivement);
            badge.GameObjectC.GetComponentInChildren<UnityEngine.UI.Button>().enabled = playerHasAchivement;
        }
    }
    /// <summary>
    /// Define all badges dict
    /// </summary>
    private void SetBadges()
    {
        badges.Add(Badge.FirstGame,
           new(Badge.FirstGame, "first_game", FirstGameBadge));

        badges.Add(Badge.TopScore,
           new(Badge.TopScore, "top_score", HighScoreBadge));

        badges.Add(Badge.ConsistentPlayer,
                 new(Badge.ConsistentPlayer, "consistent_player", ConsistentPlayerBadge));

        badges.Add(Badge.NewPersonalRecord,
           new(Badge.NewPersonalRecord, "new_personal_record", PersonalRecordBadge));

        badges.Add(Badge.SocialPlayer,
           new(Badge.SocialPlayer, "social_player", SocialPlayerBadge));


        badges.Add(Badge.SmartInvestor,
           new(Badge.SmartInvestor, "smart_investor", SmartInvestorBadge));

        badges.Add(Badge.PerfectTrade,
           new(Badge.PerfectTrade, "perfect_trade", PerfectTraderBadge));


    }



    public void SaveBadge(string badgeNameInFile, bool value)
    {
        if (badgesModel == null) badgesModel = new BadgesModel();
        badgesModel.UpdateSingleBadge(ref badgesData, badgeNameInFile, value);
        badgesModel.SaveBadgesToFile(new BadgeData { name = "PlayerName", badges = badgesData });

    }

}

public class BadgeInGame
{
    public Badge Type { get; set; }
    public string ShortDescription { get; set; }
    public string Description { get; set; }
    public string NameForLangauge { get; set; }

    public bool IsShortDescriptionActive { get; set; } = true;
    public GameObject GameObjectC { get; set; }


    public BadgeInGame(Badge type, string nameForLangauge, GameObject gameObjectC)
    {
        this.Type = type;
        this.NameForLangauge = nameForLangauge;
        this.GameObjectC = gameObjectC;
        RefreshStrings();
    }


    public string GetRelevantText()
    {
        IsShortDescriptionActive = !IsShortDescriptionActive;
        return IsShortDescriptionActive ? ShortDescription : Description;
    }

    public void RefreshStrings()
    {
        Description = LocalizationManager.Instance.GetLocalizedValue(NameForLangauge);
        ShortDescription = LocalizationManager.Instance.GetLocalizedValue(NameForLangauge + "_short");
    }

}
