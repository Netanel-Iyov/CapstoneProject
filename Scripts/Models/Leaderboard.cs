using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SocialPlatforms.Impl;
using System.Linq;

public class Leaderboard
{
    private static Leaderboard _Instance;
    private string filePath = Path.Combine(Application.persistentDataPath, "leaderboard.json");
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

    public static Leaderboard Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = new Leaderboard();
            return _Instance;
        }
    }

    private Leaderboard()
    {
        LoadFromFile();
    }

    public void AddEntry(LeaderboardEntry entry)
    {
        entries.Add(entry);
        entries.Sort((x, y) => y.score.CompareTo(x.score)); // Sort descending by score
        TruncateListToFirstTen(entries);
        SaveToFile();
    }

    void TruncateListToFirstTen<T>(List<T> list)
    {
        if (list.Count > 10)
        {
            list.RemoveRange(10, list.Count - 10);
        }
    }

    public void SaveToFile()
    {
        EnsureFileExists(filePath);
        string json = JsonUtility.ToJson(this, true);
        File.WriteAllText(filePath, json);
    }

    public void LoadFromFile()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            JsonUtility.FromJsonOverwrite(json, this);
        }
    }


    private void EnsureFileExists(string filePath)
    {
        if (!File.Exists(filePath))
        {
            try
            {
                // Create the file using File.CreateText (for text files)
                using (StreamWriter writer = File.CreateText(filePath))
                {
                }
            }
            catch (IOException ex)
            {
                Debug.LogError($"Error creating the file: {ex.Message}");
            }
        }
    }


    #region Badges

    public bool IsFirstGame() => entries.Count == 0;
    public bool IsConsistentPlayer() => entries.Count >= 3;
    public bool IsPersonalRecord(LeaderboardEntry currentEntryGame)
    {
        var highestEntryForPlayer = entries
                .Where(entry => entry.playerName == currentEntryGame.playerName)
                .OrderByDescending(entry => entry.score)
                .FirstOrDefault();

        // if no highestEntryForPlayer exist so its "empty" true...
        return highestEntryForPlayer == null || currentEntryGame.score > highestEntryForPlayer.score;
    }

    
    public bool IsHighestScore(LeaderboardEntry currentEntryGame)
    {
        var highestScoreEntry = entries
        .OrderByDescending(entry => entry.score)
        .FirstOrDefault();

        // like above
        return highestScoreEntry == null || currentEntryGame.score > highestScoreEntry.score;


    }


    public void GiveBadge(string key)
    {
        //TODO: Add popup if enabled
        var badgesModel = new BadgesModel();
        var data = badgesModel.LoadBadges();
        badgesModel.UpdateSingleBadge(ref data, key, true);
        badgesModel.SaveBadgesToFile(new BadgeData { name = "PlayerName", badges = data });
        //badgesModel.SaveBadgesToFile(new BadgeData { name = "PlayerName", badges = Badge.FirstGame });
    }

    #endregion



}


[Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;

    public LeaderboardEntry(string playerName, int score)
    {
        this.playerName = playerName;
        this.score = score;
    }
}
