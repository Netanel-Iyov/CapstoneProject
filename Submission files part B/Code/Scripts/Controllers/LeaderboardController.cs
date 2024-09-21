using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using Unity.VisualScripting;
using Assets.Scripts.Helpers;
using Unity.Services.RemoteConfig;

public class LeaderboardController : MonoBehaviour, ILanguage
{
    public GameObject entryTemplate;
    public Transform entryContainer;
    public GameObject entryTemplateHeader;
    [SerializeField] public TextMeshProUGUI headerText;
    [SerializeField] public TextMeshProUGUI headerPlayer;
    [SerializeField] public TextMeshProUGUI headerScore;
    [SerializeField] public TextMeshProUGUI headerPlace;

    // buttons
    [SerializeField] public TextMeshProUGUI leaveFeedbackBtn;
    [SerializeField] public TextMeshProUGUI shareToFriendsBtn;
    [SerializeField] public TextMeshProUGUI viewBadgesBtn;
    [SerializeField] public TextMeshProUGUI newGameBtn;
    [SerializeField] public GameObject newGameBtnObj;
    [SerializeField] public GameObject viewBadgesObj;

    public Leaderboard leaderboard;

    void OnEnable()
    {
        // check if game has been in progress, or got from main menu
        FindAnyObjectByType<HandleMenuClick>().helpButton.SetActive(false);
        leaderboard = Leaderboard.Instance;

        if (GameManager.Instance.IsGameEnded)
        {
            LeaderboardEntry entry = new LeaderboardEntry(SettingsModel.Instance.PlayerName, GameManager.Instance.MoneyAmount);
            GiveUserBadges(entry);
            leaderboard.AddEntry(entry);
        }
        PopulateLeaderboard();

        newGameBtnObj.SetActive(GameManager.Instance.GetCurrentStage() != 0); // if played at least once...
        //viewBadgesObj.SetActive(RemoteConfigService.Instance.appConfig.GetBool("BadgesActive"));
        viewBadgesObj.SetActive(SettingsModel.Instance.ActiveGamifications[GamificationMechanisms.Badges]);
    }

    private void GiveUserBadges(LeaderboardEntry entry)
    {
        bool isFirstGame = leaderboard.IsFirstGame();

        if (isFirstGame) leaderboard.GiveBadge("FirstGameBadge");
        if (leaderboard.IsConsistentPlayer()) leaderboard.GiveBadge("ConsistentPlayerBadge");
        if (leaderboard.IsPersonalRecord(entry)) leaderboard.GiveBadge("PersonalRecordBadge");
        if (leaderboard.IsHighestScore(entry)) leaderboard.GiveBadge("HighScoreBadge");
    }

    public void FetchText()
    {
        headerText.text = LocalizationManager.Instance.GetLocalizedValue("leaderboard_title");
        headerPlayer.text = LocalizationManager.Instance.GetLocalizedValue("leaderboard_player");
        headerScore.text = LocalizationManager.Instance.GetLocalizedValue("leaderboard_score");
        headerPlace.text = LocalizationManager.Instance.GetLocalizedValue("leaderboard_place");

        leaveFeedbackBtn.text = LocalizationManager.Instance.GetLocalizedValue("leaderboard_btn_feedback");
        shareToFriendsBtn.text = LocalizationManager.Instance.GetLocalizedValue("leaderboard_btn_share");
        viewBadgesBtn.text = LocalizationManager.Instance.GetLocalizedValue("leaderboard_btn_badges");
        newGameBtn.text = LocalizationManager.Instance.GetLocalizedValue("leaderboard_btn_newgame");


    }

    void PopulateLeaderboard()
    {
        foreach (Transform child in entryContainer)
        {
            if (child.name != "EntryTemplateHeader")
            {
                Destroy(child.gameObject); // Clear existing entries
            }
        }

        //float yOffset = 0f;
        //float spacing = 30f;

        //foreach (var entry in leaderboard.entries)
        //{
        //    GameObject entryObject = Instantiate(entryTemplate, entryContainer);
        //    entryObject.SetActive(true);

        //    // position with space between elem
        //    RectTransform entryRectTransform = entryObject.GetComponent<RectTransform>();
        //    entryRectTransform.anchoredPosition = new Vector2(entryRectTransform.anchoredPosition.x, yOffset);


        //    TextMeshProUGUI[] texts = entryObject.GetComponentsInChildren<TextMeshProUGUI>();
        //    if (texts.Length >= 2)
        //    {
        //        texts[0].text = entry.playerName;
        //        texts[1].text = entry.score.ToString();
        //    }

        //    yOffset -= spacing;

        //}

        float yOffset = -38f;
        float spacing = 23.5f;

        for (int i = 0; i < leaderboard.entries.Count; i++)
        {
            LeaderboardEntry entry = leaderboard.entries[i];
            GameObject entryObject = Instantiate(entryTemplate, entryContainer);
            TextMeshProUGUI playerTxt = entryObject.transform.Find("Player").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI playerScore = entryObject.transform.Find("Score").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI playerPlace = entryObject.transform.Find("Place").GetComponent<TextMeshProUGUI>();

            playerTxt.text = entry.playerName;
            playerTxt.isRightToLeftText = CommonFunctions.IsHebrewText(playerTxt.text) ? true : false;

            playerScore.text = entry.score.ToString();
            playerScore.isRightToLeftText = false;

            playerPlace.text = getPlaceString(i + 1);
            playerPlace.isRightToLeftText = false;

            if (i % 2 == 0)
            {
                Image bg = entryObject.GetComponent<Image>();
                Destroy(bg);
            }

            // position with space between elem
            RectTransform entryRectTransform = entryObject.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(entryRectTransform.anchoredPosition.x, yOffset);

            yOffset -= spacing;
        }
    }


    string getPlaceString(int place)
    {
        // <color=green>green</color>
        LanguageCode languageCode = SettingsModel.Instance.Language;
        string placeString = "";
        string toReturn = "";
        switch (place)
        {
            // <color=#00ff00ff>colorfully</color>
            case 1:
                placeString = languageCode == LanguageCode.EN ? "1st" : "1";
                return $"<color=#00ff00ff>{placeString}</color>";
            case 2:
                placeString = languageCode == LanguageCode.EN ? "2nd" : "2";
                return $"<color=#dfdf00ff>{placeString}</color>";
            case 3:
                placeString = languageCode == LanguageCode.EN ? "3rd" : "3";
                return $"<color=#ff4600ff>{placeString}</color>";
            default:
                return languageCode == LanguageCode.EN ? $"{place}th" : $"{place}";
        }

    }
}
