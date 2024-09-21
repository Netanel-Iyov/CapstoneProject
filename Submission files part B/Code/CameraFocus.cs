using Unity.Services.Analytics;
using UnityEngine;
using Vuforia;

public class CameraFocus : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        VuforiaApplication.Instance.OnVuforiaStarted += StartVuforiaFocus;
        //ConsentGiven();
        //AnalyticsService.Instance.StartDataCollection();
    }
    public void StartVuforiaFocus()
    {
        VuforiaBehaviour.Instance.CameraDevice.SetFocusMode(FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
    }
    // Update is called once per frame
    void Update()
    {

    }


}
