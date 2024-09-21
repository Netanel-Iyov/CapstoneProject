using UnityEngine;
using Vuforia;

public class ChildObjectController : MonoBehaviour
{
    private GameObject activeTarget;  // the current active image target

    public GameObject[] childObjects;
    public GameObject[] imageTargets;

    public void EnableChildObjects(GameObject target)
    {
        if (activeTarget != null && activeTarget != target)
        {
            activeTarget.GetComponent<ChildObjectController>().DisableChildObjects();
        }

        foreach (GameObject childObject in childObjects)
        {
            childObject.SetActive(true);
        }

        activeTarget = target;
    }

    public void DisableChildObjects()
    {
        if (IsImageTracked()) return;
        foreach (GameObject childObject in childObjects)
        {
            childObject.SetActive(false);
        }
        activeTarget = null;
    }


    private bool IsImageTracked()
    {
        foreach (GameObject target in imageTargets)
        {
            ImageTargetBehaviour trackableBehaviour = target.GetComponent<ImageTargetBehaviour>();

            bool isTracked = trackableBehaviour.TargetStatus.Equals(Status.TRACKED) || trackableBehaviour.TargetStatus.Equals(Status.EXTENDED_TRACKED) || trackableBehaviour.TargetStatus.Equals(Status.LIMITED);
            if (trackableBehaviour != null && isTracked)
            {
                Debug.Log(target.name + " is found.");
                return true;
            }
        }
        return false;
    }
}
