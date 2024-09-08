using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class QuitController : MonoBehaviour
{
    public GameObject QuitPopUpPanel;
    private string feedbackURL_HE = "https://docs.google.com/forms/d/1A9mSidMcC4OjHuiwL8kiCBWt7MnflF_mgz3FK56sTVU/viewform";
    private string feedbackURL_EN = "https://docs.google.com/forms/d/12yAdiS70PFSojDZF5tT1pGOkz_I5mg2vmojEKTeMF-4/viewform";
    private string userNameEntry = "entry.1916197790";

    void Start()
    {
        QuitPopUpPanel.SetActive(false);
    }

    void Update()
    {
        // Check if the back button is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!QuitPopUpPanel.activeSelf)
            {
                // Show the confirmation dialog
                QuitPopUpPanel.SetActive(true);
            }
            else
            {
                // If the dialog is already visible, and another escape was pressed, then exit the application
                Application.Quit();
            }
        }
    }

    public void ConfirmQuit()
    {
        Application.Quit();
    }

    // This method is called when the cancel button on the dialog is clicked
    public void RedirectToFeedback()
    {
        string baseURL = SettingsModel.Instance.Language == LanguageCode.HE ? feedbackURL_HE : feedbackURL_EN;
        string username = string.Empty;
        string pattern = @"[A-Za-z0-9\u0590-\u05FF]+";
        Match match = Regex.Match(SettingsModel.Instance.PlayerName, pattern);

        if (match.Success) username = match.Value;
    

        string URLwithUsername = $"{baseURL}?{userNameEntry}={username}&";
        Application.OpenURL(URLwithUsername);
        
        Debug.Log(URLwithUsername);

    }
}