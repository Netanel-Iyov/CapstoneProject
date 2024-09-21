using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialShareController : MonoBehaviour
{
    ShareModel shareModel;
    // Start is called before the first frame update
    void Start()
    {
        shareModel = ShareModel.Instance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenShareOptions()
    {
        string subject = LocalizationManager.Instance.GetLocalizedValue("share_subject");
        string body = LocalizationManager.Instance.GetLocalizedValue("share_body");

#if UNITY_EDITOR
        return;
#endif

#if UNITY_ANDROID
        shareModel.OpenShareOptions(subject, body);
#endif
    }
}
