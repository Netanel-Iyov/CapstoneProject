using System;
using UnityEngine;
public class StandDetector : MonoBehaviour
{

    /// <summary>
    /// Detect a collision if stage 2 is active.
    /// </summary>
    /// <param name="other"></param>
    void OnCollisionEnter(Collision other)
    {
        // open a popup when user enters the zone of a stand

        if (GameManager.Instance.GetCurrentStage() != 2) return;  // ignore other collisions

        // get the stand num
        string[] numStr = other.collider.tag.Split(new string[] { "Stand" }, StringSplitOptions.None);
        int num = (numStr.Length == 2 && int.TryParse(numStr[1], out int parsedNum)) ? parsedNum : 0;

        Stage2Controller controller = FindAnyObjectByType<Stage2Controller>();
        if (controller != null)
        {
            controller.OpenBuyPopup(num);
        }
    }


    private void OnCollisionExit(Collision other)
    {
        Stage2Controller controller = FindAnyObjectByType<Stage2Controller>();
        if (controller != null)
        {
            controller.CloseBuyPopup();
        }
    }
}