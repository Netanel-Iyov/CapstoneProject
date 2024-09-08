using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Unity.Services.Authentication;

public class AnalyticsManager
{
    private static AnalyticsManager _instance;
    public static AnalyticsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AnalyticsManager();
            }
            return _instance;
        }
    }

    /// <summary>
    /// private constructor
    /// </summary>
    private AnalyticsManager()
    {

    }



    /// <summary>
    /// Init the services 
    /// </summary>
    /// <returns></returns>
    public async Task InitializeUnityServices()
    {
        try
        {
            if (UnityServices.State != ServicesInitializationState.Initialized &&
                UnityServices.State != ServicesInitializationState.Initializing)
            {
                await UnityServices.InitializeAsync();
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
                Debug.Log("Unity Services initialized successfully.");
            }
            else
            {
                Debug.Log("Unity Services are already initializing or initialized.");
            }

        }
        catch (ServicesInitializationException ex)
        {
            Debug.LogError("Failed to initialize Unity Services: " + ex);
        }
    }

    /// <summary>
    /// After user conset, try to start data collection
    /// </summary>
    public void StartAnalyticsDataCollection()
    {
        if (!IsAnalyticsInit()) return;


        try
        {
            AnalyticsService.Instance.StartDataCollection();
            Debug.Log("Analytics data collection started.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to start data collection: " + ex);
        }
    }

    /// <summary>
    /// Not in use for now.
    /// </summary>
    public void StopAnalyticsDataCollection()
    {
        if (!IsAnalyticsInit()) return;


        try
        {
            AnalyticsService.Instance.StopDataCollection();
            Debug.Log("Analytics data collection started.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to start data collection: " + ex);
        }
    }


    private bool IsAnalyticsInit()
    {
        if (AnalyticsService.Instance != null) return true;
        Debug.LogError("AnalyticsService instance is null.");
        return false;

    }
    #region events


    private DateTime scanARStartTime, stageStartTime;

    /// <summary>
    ///  should be called on stageStart
    /// </summary>
    public void OnStageStart()
    {
        stageStartTime = DateTime.UtcNow;
    }

    public void OnStageEnd(int stageNumber, int points = 0)
    {
        if (stageStartTime == DateTime.MinValue) return; // show message here? 

        // diff
        DateTime stageEndTime = DateTime.UtcNow;
        TimeSpan timeSpent = stageEndTime - stageStartTime;

        // I made custom event for completed stage, 
        Dictionary<string, object> eventParameters = new Dictionary<string, object>
        {
            { "PlayerName", SettingsModel.Instance.PlayerName },
            { "stageNumber", stageNumber },
            { "timeSpentInSeconds", timeSpent.TotalSeconds.ToString() },
            { "points", points }
        };

        AnalyticsService.Instance.CustomData("stageComplete", eventParameters);


        // reset
        stageStartTime = DateTime.MinValue;


        // for debug, can delete later
        Debug.Log($"Stage {stageNumber} completed in {timeSpent.TotalSeconds} seconds.");
    }

    public void StartScanning() => scanARStartTime = DateTime.UtcNow;
    public void EndScanning()
    {
        if (scanARStartTime == DateTime.MinValue) return; // show message here? 

        // diff
        DateTime scanEndTime = DateTime.UtcNow;
        TimeSpan timeSpent = scanEndTime - scanARStartTime;

        Dictionary<string, object> eventParameters = new Dictionary<string, object>
        {
            { "timeSpentInSeconds", timeSpent.TotalSeconds.ToString() },
                    { "PlayerName", SettingsModel.Instance.PlayerName }
};

        AnalyticsService.Instance.CustomData("ARScanningTime", eventParameters);


        // reset
        scanARStartTime = DateTime.MinValue;

    }







    /// <summary>
    /// Wrapper
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="parameters"></param>
    private void SendEvent(string eventName, Dictionary<string, object> parameters = null)
    {
        try
        {
            if (!IsAnalyticsInit()) return;

            if (parameters != null)
                AnalyticsService.Instance.CustomData(eventName, parameters);
            else
                AnalyticsService.Instance.CustomData(eventName);
        }
        catch
        {
            Debug.LogError("failed to send event.");
            return;
        }
        Debug.Log("event sent successfully.");

    }



    #endregion

}
