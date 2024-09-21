using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExplainationScreenController : MonoBehaviour, ILanguage
{
    public GameObject ExplainScreenUI;
    public TextMeshProUGUI MainText;
    public GameObject SkipButton;
    public GameObject NextButton;
    public GameObject BackButton;

    private LocalizationManager lm;
    private int currentPage;
    private const int endPageNumber = 4;

    // Start is called before the first frame update
    void Start()
    {
        lm = LocalizationManager.Instance;

        currentPage = 0;
        ControlNextBackButtonVis();
        SkipButton.SetActive(false);
        FetchText();

    }

    private void SetMainText(int page)
    {
        string relevantText = string.Empty;
        switch (page)
        {
            case 0:
                relevantText = lm.GetLocalizedValue("explain_intro");
                break;

            case 1:
                relevantText = lm.GetLocalizedValue("explain_p1");
                break;

            case 2:
                relevantText = lm.GetLocalizedValue("explain_p2");
                break;

            case 3:
                relevantText = lm.GetLocalizedValue("explain_p3");
                break;

            case 4:
                relevantText = lm.GetLocalizedValue("explain_p4");
                break;

            default:
                break;
        }
        MainText.text = relevantText;
    }



    public void onExit()
    {
        ExplainScreenUI.SetActive(false);
    }

    private void ControlNextBackButtonVis()
    {
        switch (currentPage)
        {
            case 0:
                BackButton.GetComponent<Button>().interactable = false;
                break;
            case 1:
            case 2:
            case 3:
                BackButton.GetComponent<Button>().interactable = true;
                NextButton.GetComponent<Button>().interactable = true;
                break;
            case endPageNumber:
                NextButton.GetComponent<Button>().interactable = false;
                SkipButton.SetActive(true);
                break;
            default:
                break;
        }

    }
    public void onForward()
    {
        currentPage++;
        ControlNextBackButtonVis();
        SetMainText(currentPage);
    }

    public void onBack()
    {
        currentPage--;
        ControlNextBackButtonVis();
        SetMainText(currentPage);
    }


    public void FetchText()
    {
        SetMainText(currentPage); // refresh
        SkipButton.GetComponentInChildren<TextMeshProUGUI>().text = lm.GetLocalizedValue("continue");
    }


}
