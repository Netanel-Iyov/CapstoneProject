using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using System.IO;
using UnityEditor;

public enum Badge
{
    FirstGame,              // First time playing the game
    TopScore,
    NewPersonalRecord,
    ConsistentPlayer,       // play the game at least 3 times (same username)
    SocialPlayer,           // share the game at least 1 time
    SmartInvestor,          // make high profit (ex. invest x money, and get 150% X money)
    PerfectTrade,           // sell all the commodities without any loss
}

[System.Serializable]
public class BadgeData
{
    public string name;
    public Dictionary<string, bool> badges;
}


public class BadgesModel
{
    string path = string.Empty;

    public BadgesModel()
    {
        path = Path.Combine(Application.persistentDataPath, "badges.json");
        if (!File.Exists(path))
            CreateFile();
    }

    #region Files
    /// <summary>
    /// Create if doesn't exists
    /// </summary>
    public void CreateFile()
    {
        // Create a new BadgeData object with default values
        BadgeData initialBadgeData = new BadgeData
        {
            name = "PlayerName",
            badges = new Dictionary<string, bool>
                {
                    { "FirstGameBadge", false },
                    { "ConsistentPlayerBadge", false },
                    { "PersonalRecordBadge", false },
                    { "PerfectTraderBadge", false },
                    { "HighScoreBadge", false },
                    { "SmartInvestorBadge", false },
                    { "SocialPlayerBadge", false }
                }
        };

        string json = JsonConvert.SerializeObject(initialBadgeData, Formatting.Indented);

        File.WriteAllText(path, json);
        Debug.Log($"Badges file created at {path} with initial data.");

    }
    public Dictionary<string, bool> LoadBadges()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            BadgeData badgeData = JsonConvert.DeserializeObject<BadgeData>(json);
            return badgeData.badges;
        }
        else
        {
            Debug.LogError("Badges JSON file is missing.");
            return null;
        }
    }

    public void UpdateSingleBadge(ref Dictionary<string, bool> badgesData, string key, bool value)
    {
        if (badgesData.ContainsKey(key))
            badgesData[key] = value;
        else
            badgesData.Add(key, value);
    }

    public void SaveBadgesToFile(BadgeData badgeData)
    {
        string updatedJson = JsonConvert.SerializeObject(badgeData, Formatting.Indented);
        File.WriteAllText(path, updatedJson);
        Debug.Log($"Badges data has been saved to {path}");
    }

    #endregion



}

//public class BadgesHandler{
//    private static BadgesHandler _instance;
//    public static BadgesHandler Instance
//    {
//        get
//        {
//            if (_instance == null)
//            {
//                _instance = new BadgesHandler();
//            }
//            return _instance;
//        }
//    }

//    private BadgesHandler()
//    {

//    }


//}