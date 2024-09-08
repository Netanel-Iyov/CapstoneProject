using Assets.Scripts.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

class Stage3Controller : StageController
{
    Stage3Model currentModel;
    [SerializeField] private TextMeshProUGUI moneyLbl;
    [SerializeField] private TextMeshProUGUI endBtn;
    [SerializeField] private TextMeshProUGUI backBtn;
    [SerializeField] private GameObject crateTemplate;
    [SerializeField] private GameObject environment;
    [SerializeField] private GameObject modelTargetInstance;
    [SerializeField] private GameObject initialPlaceForCrates;
    [SerializeField] private GameObject fence;

    [SerializeField] private AudioClip moneySound1;
    [SerializeField] private AudioClip moneySound2;


    private List<GameObject> generatedCrates = new();


    public void OnEnable()
    {
        SetProgressBar(3);
        if (currentModel == null) currentModel = new Stage3Model();  // check if loop
        currentModel.SetSounds(moneySound1, moneySound2); // pass the sounds

        Camera.main.GetComponent<BoxCollider>().enabled = false;
        if (fence != null)
        {
            fence.SetActive(true);
            GameObject.Find("FenceText").GetComponent<TextMeshPro>().text = "סחורות שקנית";

        }
        currentModel.StartStage();
        UpdateMoneyTxt();
        SetCommoditiesBoxesInScene(); // refresh commodities based on user from last stage

        StartCoroutine(StartTimerCheckForHelp(30f));
    }
    private void OnDisable()
    {
        var obj = GameObject.Find("FenceText");
        if (obj != null) obj.GetComponent<TextMeshPro>().text = "שוק";

        if (fence != null) fence.SetActive(false);
    }
    /// <summary>
    /// Called when user drag from ship to port
    /// </summary>
    public bool OnDropCommo(Commoditie commo)
    {
        needHelp = false;
        bool res = currentModel.IsValidDrop(commo);
        currentModel.OnCommodityLoaded(commo.Type);
        UpdateMoneyTxt();
        return res;
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
        generatedCrates.ForEach(go => Destroy(go)); // clean
        environment.SetActive(false); // disable environment
        currentModel.CompleteStage();
        CloseProgressBar();
    }

    private void CloseProgressBar()
    {
        if (IsProgressBar()) progressBar.SetActive(false);
    }

    /// <summary>
    /// Generate commodities boxes / crates in port.
    /// Same commoditie will stack together while others will keep in distance (on x axis)
    /// </summary>


    private void SetCommoditiesBoxesInScene()
    {
        generatedCrates.ForEach(go => Destroy(go));
        generatedCrates.Clear();

        var userCommodities = StageModel.GetUserCurrentCommodities();
        Vector3 initialPosition = initialPlaceForCrates.transform.position;//new Vector3(-1f, -0.8f, 0f); // initial position

        foreach (Commoditie comm in userCommodities)
        {
            var startCreate = Instantiate(crateTemplate, gameObject.transform);
            startCreate.transform.position = initialPosition;
            SetBoxScript(startCreate, comm);


            for (int i = 1; i < comm.Amount; i++)
            {
                var moreGo = Instantiate(crateTemplate, startCreate.transform);
                moreGo.transform.position = new Vector3(startCreate.transform.position.x, startCreate.transform.position.y + 1.5f * i, startCreate.transform.position.z);
                moreGo.transform.localScale = new Vector3(1f, 1f, 1f);
                moreGo.GetComponent<Rigidbody>().isKinematic = false;
                SetBoxScript(moreGo, comm);
                generatedCrates.Add(moreGo);
            }
            SetCrateNames(startCreate, typeToText[comm.Type]);
            startCreate.GetComponent<Rigidbody>().isKinematic = false;
            generatedCrates.Add(startCreate);
            // Move x for the next commodity
            initialPosition.x -= .5f;
        }
    }


    //private void SetCommoditiesBoxesInScene()
    //{
    //    generatedCrates.ForEach(go => Destroy(go));
    //    generatedCrates.Clear();

    //    var userCommodities = StageModel.GetUserCurrentCommodities();
    //    Vector3 initialPosition = initialPlaceForCrates.transform.position;//new Vector3(-1f, -0.8f, 0f); // initial position

    //    foreach (Commoditie comm in userCommodities)
    //    {
    //        var startCreate = Instantiate(crateTemplate, gameObject.transform);
    //        startCreate.transform.position = initialPosition;
    //        SetBoxScript(startCreate, comm);


    //        for (int i = 1; i < comm.Amount; i++)
    //        {
    //            var moreGo = Instantiate(crateTemplate, startCreate.transform);
    //            moreGo.transform.position = new Vector3(startCreate.transform.position.x, startCreate.transform.position.y + 1.5f * i, startCreate.transform.position.z);
    //            moreGo.transform.localScale = new Vector3(1f, 1f, 1f);
    //            moreGo.GetComponent<Rigidbody>().isKinematic = false;
    //            SetBoxScript(moreGo, comm);
    //            generatedCrates.Add(moreGo);
    //        }
    //        SetCrateNames(startCreate, typeToText[comm.Type]);
    //        startCreate.GetComponent<Rigidbody>().isKinematic = false;
    //        generatedCrates.Add(startCreate);
    //        // Move x for the next commodity
    //        initialPosition.x -= .5f;
    //    }
    //}

    private void SetBoxScript(GameObject crate, Commoditie commo)
    {
        var moveBoxScript = crate.AddComponent<MoveBoxOnClick>();
        moveBoxScript.modelTarget = modelTargetInstance.transform; // Set the model target for this instance
        moveBoxScript.boxCommodite = commo;
    }

    private void SetCrateNames(GameObject crate, string textToSet)
    {

        TextMeshPro[] tmpComponents = crate.GetComponentsInChildren<TextMeshPro>();
        // iterate all text sides
        foreach (TextMeshPro tmp in tmpComponents)
        {
            tmp.text = textToSet;
            tmp.isRightToLeftText = CommonFunctions.IsHebrewText(tmp.text);
        }
    }

    public void GoBack()
    {
        PopupManager.Instance.HidePopUp();
        GameManager.Instance.GoBackToStage2();
    }

    public override void FetchText()
    {
        base.FetchText();
        endBtn.text = LocalizationManager.Instance.GetLocalizedValue("end");
        backBtn.text = LocalizationManager.Instance.GetLocalizedValue("back");
    }

}

