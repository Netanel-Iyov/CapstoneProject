using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnvironmentModel : MonoBehaviour
{


    private static EnvironmentModel _Instance;
    public static EnvironmentModel Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new EnvironmentModel();
            }
            return _Instance;

        }
    }
    private EnvironmentModel()
    {
        
    }

    public void SetupEnvironment(GameObject EnvironmentScreen)
    {
        EnvironmentScreen.SetActive(true);
        var cameraTransform = Camera.main.transform;
        // set the sea to be infront of camera, in locked position
        EnvironmentScreen.transform.position = cameraTransform.position + cameraTransform.forward * 20 + cameraTransform.up * (-300);
    }

}
