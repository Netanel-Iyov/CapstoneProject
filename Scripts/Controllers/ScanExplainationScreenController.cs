using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class ScanExplainationScreenController : MonoBehaviour, ILanguage
{
    public TextMeshProUGUI MainText;
    public GameObject SkipButton;
    public GameObject NextButton;
    public GameObject BackButton;
    public GameObject image;
    public GameObject ScanExplanationPopup;

    private LocalizationManager lm;
    private int currentPage;
    private const int endPageNumber = 2;

    // Start is called before the first frame update
    void Start()
    {
        lm = LocalizationManager.Instance;

        currentPage = 0;
        ControlNextBackButtonVis();
        FetchText();

    }
    private void SetMainText(int page)
    {
        string relevantText = string.Empty;

        switch (page)
        {
            case 0:
                relevantText = lm.GetLocalizedValue("scan_explanation_popup_0");
                break;

            case 1:
                relevantText = lm.GetLocalizedValue("scan_explanation_popup_1");
                break;

            case 2:
                relevantText = lm.GetLocalizedValue("scan_explanation_popup_2");
                break;

            default:
                break;
        }
        
        MainText.text = relevantText;

        string spritePath = Path.Combine("Images", "ScanExplanationImages", $"page-{page}");
        Image imageComponent = image.GetComponent<Image>();

        if (imageComponent == null)
        {
            Debug.LogWarning("No Image component found on the GameObject.");
            return;
        }

        Sprite newSprite = Resources.Load<Sprite>(spritePath);

        if (newSprite != null)
        {
            // Assign the sprite to the GameObject's SpriteRenderer
            imageComponent.sprite = newSprite;
            imageComponent.SetAllDirty(); // force a sprite render update
        }
        else
        {
            Debug.LogError("Sprite not found at " + spritePath);
        }
    }

     private void ControlNextBackButtonVis()
     {
         switch (currentPage)
         {
             case 0:
                 BackButton.GetComponent<Button>().interactable = false;
                 break;
             case 1:
                 BackButton.GetComponent<Button>().interactable = true;
                 NextButton.GetComponent<Button>().interactable = true;
                 break;
             case endPageNumber:
                 NextButton.GetComponent<Button>().interactable = false;
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

    public void onExit()
    {
        currentPage = 0;
        ScanExplanationPopup.SetActive(false);
    }

    public void FetchText()
    {
        SetMainText(currentPage); // refresh
        // SkipButton.GetComponentInChildren<TextMeshProUGUI>().text = lm.GetLocalizedValue("skip");
    }

}
