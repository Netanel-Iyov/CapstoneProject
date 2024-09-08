using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapScreenController : MonoBehaviour, ILanguage
{
    public GameObject MapScreenUI;
    public Image floorImage;
    public GameObject backIcon;
    public GameObject forwardIcon;
    public GameObject floorHeaderObject;
    public GameObject rightPanelHeaderObject;
    public GameObject rightPanelBodyObject;

    private TextMeshProUGUI floorHeaderText;
    private TextMeshProUGUI rightPanelHeaderText;
    private TextMeshProUGUI rightPanelBodyText;
    private int currentFloor;
    private LocalizationManager lm;
    private MapModel mapModel;

    // Start is called before the first frame update
    void Start()
    {
        mapModel = MapModel.Instance;
        lm = LocalizationManager.Instance;
        floorHeaderText = floorHeaderObject.GetComponent<TextMeshProUGUI>();
        rightPanelHeaderText = rightPanelHeaderObject.GetComponent<TextMeshProUGUI>();
        rightPanelBodyText = rightPanelBodyObject.GetComponent<TextMeshProUGUI>();
        currentFloor = 2;

        SetFloor();
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }


    public void onExit()
    {
        MapScreenUI.SetActive(false);
    }

    public void onForward()
    {
        if (currentFloor == 0)
        {
            backIcon.SetActive(true);
        }

        currentFloor += 1;

        if (currentFloor == 3)
        {
            forwardIcon.SetActive(false);
        }

        SetFloor();
    }

    public void onBack()
    {
        if (currentFloor == 3)
        {
            forwardIcon.SetActive(true);
        }
        
        currentFloor -= 1;
        
        if (currentFloor == 0)
        {
            backIcon.SetActive(false);
        }

        SetFloor();
    }

    private void SetFloor()
    {
        SetImage();
        SetFloorHeader();
        SetRightPanel();
    }

    private void SetFloorHeader()
    {
        floorHeaderText.text = lm.GetLocalizedValue($"floor{currentFloor}_map_header");
    }

    private void SetRightPanel()
    {
        rightPanelHeaderText.text = lm.GetLocalizedValue($"floor{currentFloor}_panel_header");
        rightPanelBodyText.text = lm.GetLocalizedValue($"floor{currentFloor}_panel_body");
    }

    private void SetImage()
    {
        floorImage.sprite = mapModel.GetImageAsSprite($"Images/floor{currentFloor}");
    }

    public void FetchText()
    {
        SetFloor(); // refresh
        lm.UpdateAlignmentForText(rightPanelBodyText);
    }

    // for some reason, without this function and since all the other signatures doesnt have parameters,
    // unity does not recognize the other functions when trying to attach them to an onClick event on the unity editor
    // you should not use this function
    public void dummyFunction(string a)
    {
        return;
    }
}
