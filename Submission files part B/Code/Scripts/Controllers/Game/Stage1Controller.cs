using Assets.Scripts.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

class Stage1Controller : StageController, ILanguage
{
    Stage1Model currentModel;
    private List<Transform> cretesLocations = new();
    public GameObject newCrates;
    public GameObject port;
    public GameObject stage1Game;
    public TextMeshProUGUI CrateProgress;

    public void Start()
    {
        SaveOrSetInitialCratesLocation(true); // save
    }

    public void SaveOrSetInitialCratesLocation(bool save = false)
    {
        //var cratesParent = gameObject.transform.Find("Stage1Game").transform.Find("Crates");
        // iterate eachj crate
        if (save)
        {
            newCrates.SetActive(true);
            foreach (Transform crateTransform in newCrates.transform)
                cretesLocations.Add(crateTransform);
        }
        else
        {
            for (int i = 1; i < newCrates.transform.childCount; i++)
                if (cretesLocations.Count >= i)
                    newCrates.transform.GetChild(i).position = cretesLocations[i].position;

        }

    }
    public void OnEnable()
    {
        currentModel = new Stage1Model();
        currentModel.StartStage();
        SaveOrSetInitialCratesLocation(); // set
        if (IsProgressBar()) InitalizeProgressBar();
        SetCrateNames();
        StartCoroutine(StartTimerCheckForHelp(15f));
    }
    
    private void OnDisable()
    {
        if (newCrates != null)
            newCrates.SetActive(false);
    }

    private void InitalizeProgressBar()
    {
        progressBar.SetActive(true);
        SetProgressBar(1);
    }

    /// <summary>
    /// Called when user drag from ship to port
    /// </summary>
    public void OnDropCommo()
    {
        if (IsValidDrop())
        {
            currentModel.OnCommodityUnloaded();
            needHelp = false;
        }
        CrateProgress.text = currentModel.GetProgress();
        // else do nothing ?
    }

    /// <summary>
    /// Checks if the drop was to port
    /// </summary>
    /// <returns></returns>
    public bool IsValidDrop()
    {
        return true;
    }

    private void SetCrateNames()
    {
        var cratesParent = gameObject.transform.Find("Stage1Game").transform.Find("Crates");
        var len = cratesParent.transform.childCount;
        int i = 0;

        // iterate eachj crate
        foreach (Transform crateTransform in cratesParent.transform)
        {
            GameObject crate = crateTransform.gameObject;
            TextMeshPro[] tmpComponents = crate.GetComponentsInChildren<TextMeshPro>();

            var textToSet = (i++) < len / 2 ? typeToText[CommoditieType.WINE] : typeToText[CommoditieType.ALMONDS];

            // iterate all text sides
            foreach (TextMeshPro tmp in tmpComponents)
            {
                tmp.text = textToSet;
                tmp.isRightToLeftText = CommonFunctions.IsHebrewText(tmp.text);
            }

        }

    }

    public override void FetchText()
    {
        base.FetchText();
        try
        {
            SetCrateNames();
        }
        catch
        {
        }
    }
}

