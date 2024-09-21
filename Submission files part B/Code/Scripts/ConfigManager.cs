using UnityEngine;
using Unity.RemoteConfig;
using System;
using Unity.Services;
using Unity.Services.RemoteConfig;
using System.Collections.Generic;
using System.Collections;
using System.Threading;

public class RemoteConfigManager : MonoBehaviour
{
    private static RemoteConfigManager _Instance;
    public static RemoteConfigManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new RemoteConfigManager();
            }
            return _Instance;

        }
    }
    private RemoteConfigManager()
    {
        RemoteConfigService.Instance.FetchCompleted += Instance_FetchCompleted;
        //RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
        RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
    }


    // User and App attributes (can be extended if needed)
    struct userAttributes { }
    struct appAttributes { }




    private void Instance_FetchCompleted(ConfigResponse configResponse)
    {
        //configResponse.body.GetValue("configs")["settings"]["BadgesActive"].ToString()
        SettingsModel.Instance.ActiveGamifications[GamificationMechanisms.Badges] = RemoteConfigService.Instance.appConfig.GetBool("BadgesActive", true);
        SettingsModel.Instance.ActiveGamifications[GamificationMechanisms.ProgressBar] = RemoteConfigService.Instance.appConfig.GetBool("ProgressBarActive", true);
        Debug.Log($"{SettingsModel.Instance.ActiveGamifications[GamificationMechanisms.Badges]} === {RemoteConfigService.Instance.appConfig.GetBool("BadgesActive", true)}");

        //try
        //{
        //    FindAnyObjectByType<HandleMenuClick>().BadgesContainer.SetActive(SettingsModel.Instance.ActiveGamifications[GamificationMechanisms.Badges]);
        //}
        //catch { }
    }


}
