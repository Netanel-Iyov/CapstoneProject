using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameManager
{
    private StageController currentStage;
    public const int StartMoney = 500;
    public int MoneyAmount
    {
        get
        {
            return PlayerPrefs.GetInt("MoneyAmount", StartMoney);
        }
        set
        {
            PlayerPrefs.SetInt("MoneyAmount", value);
            PlayerPrefs.Save();
        }
    }

    private ScenesManager sceneManager;
    private int currentStageNum { get; set; } = 0;
    public int GetCurrentStage() => currentStageNum;
    public void ResetGame() => currentStageNum = 0;
    public const int FINAL_STAGE = 3;
    public bool IsGameEnded { get; private set; } = false;

    private List<GameObject> stages;
    private GameObject ARStage, environmentScreen, endGameScreen;

    private static GameManager _Instance;
    public static GameManager Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = new GameManager();
            return _Instance;
        }


    }

    private GameManager()
    {
        sceneManager = ScenesManager.Instance;
    }

    public void InsertStagesScreens(List<GameObject> stages, GameObject ARStage, GameObject environmentScreen, GameObject endGameScreen)
    {
        this.stages = stages;
        this.ARStage = ARStage;
        this.environmentScreen = environmentScreen;
        this.endGameScreen = endGameScreen;
        stages.ForEach(s => s.SetActive(false));
    }

    #region AR Detection
    Timer askDetect;
    public async void AskDetect()
    {
        AnalyticsManager.Instance.StartScanning();
        ARStage.SetActive(true);
        await sceneManager.PlayScene(SceneIdentifier.STAGE_0_ASKDETECT);
        await sceneManager.PlayScene(SceneIdentifier.STAGE_0_SCAN_INSTRUCTIONS);
        //askDetect = new Timer(async (t) =>
        //{
        //    // every 5 seconds play again
        //    await sceneManager.PlayScene(SceneIdentifier.STAGE_0_ASKDETECT);
        //}, null, 0, 1000*5);
    }


    public async void ShipDetected()
    {
        AnalyticsManager.Instance.EndScanning();
        ARStage.SetActive(false);  // deactivate detection screen
        if (askDetect != null)
            askDetect.Dispose();
        await sceneManager.PlayScene(SceneIdentifier.STAGE_0_DETECTED);
        EnvironmentModel.Instance.SetupEnvironment(environmentScreen); // init
        StartGame();
    }
    #endregion

    public bool IsGameActive { get; private set; }
    public async void StartGame()
    {
        InitGame();
        //await ScenesManager.Instance.PlayScene(SceneIdentifier.GAME_INTRO);
        StartNewStage(1);
    }

    public void EndGame()
    {
        IsGameEnded = true;
        // show leaderboard
        endGameScreen.SetActive(true);

    }

    /// <summary>
    /// Initialize a new game, reset
    /// </summary>
    public void InitGame()
    {
        if (PlayerPrefs.HasKey("MoneyAmount"))
            PlayerPrefs.DeleteKey("MoneyAmount"); // the next get will restore it by default
        currentStageNum = 1;


    }

    public void StartNewStage(int stageNum)
    {
        AnalyticsManager.Instance.OnStageStart();
        currentStage = (stages[stageNum - 1]).GetComponent<StageController>();
        currentStage.gameObject.SetActive(true); // start the stage
    }
    public void OnStageComplete()
    {
        AnalyticsManager.Instance.OnStageEnd(currentStageNum, MoneyAmount);
        (stages[currentStageNum - 1]).GetComponent<StageController>().gameObject.SetActive(false); // disable the previous 
        if (currentStageNum != FINAL_STAGE)
            StartNewStage(++currentStageNum);
        else
            EndGame();
    }

    public void GoBackToStage2()
    {
        if (currentStageNum != 3) return;
        (stages[currentStageNum - 1]).GetComponent<StageController>().gameObject.SetActive(false); // disable the previous 
        StartNewStage(--currentStageNum);

    }
}
