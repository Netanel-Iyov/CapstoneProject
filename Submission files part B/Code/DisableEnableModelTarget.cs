using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Vuforia;


public class DisableEnableModelTarget : MonoBehaviour
{
    private ObserverBehaviour observerBehaviour;
    public GameObject Terrains;
    GameObject port;

    public static bool IsDetected { get; set; } = false; // disable by default

    void Start()
    {
        observerBehaviour = GetComponent<ObserverBehaviour>();
        if (observerBehaviour)
            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;

    }


    //private void Update()
    //{
    //    if (IsDetected)
    //    {
    //        port.transform.rotation = Quaternion.Euler(0, -90, 0); //port.transform.rotation.eulerAngles.y

    //    }
    //}

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (!IsDetected && status.Status == Status.TRACKED)
        {

            IsDetected = true;
            Terrains.SetActive(true);
            var cameraPos = Camera.main.transform.position;
            Terrains.transform.position = new Vector3(cameraPos.x, cameraPos.y - 1, cameraPos.z);
            //Terrains.transform.rotation = Quaternion.identity;
            GameManager.Instance.ShipDetected();


        }
    }

    void OnDestroy()
    {
        if (observerBehaviour)
        {
            observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
    }
}



/// <summary>
/// This script controls the activation / disactivation of the ModelTarget 
/// We want to keep the gameobject active, but not the detection
/// </summary>
//public class DisableEnableModelTarget : MonoBehaviour
//{
//    private ObserverBehaviour observerBehaviour;
//    public static bool IsDetected { get; set; } = false; // disable by default
//    [SerializeField] GameObject Port;
//    [SerializeField] GameObject Sea;
//    [SerializeField] GameObject SeaObjects;
//    [SerializeField] GameObject MeshObject;
//    [SerializeField] GameObject AllGameobjects;

//    [SerializeField] MeshCollider shipMesh;
//    [SerializeField] BoxCollider LikeSeaBoxCollider;
//    [SerializeField] BoxCollider PortBoxCollider;
//    [SerializeField] MeshRenderer PortMeshRenderer;
//    [SerializeField] MeshRenderer WaterBlockMeshRenderer;
//    [SerializeField] Terrain Terrain;
//    [SerializeField] TerrainCollider TerrainCollider;

//    private Vector3 initialPortPosition;
//    private Quaternion initialPortRotation;
//    private Vector3 initialSeaPosition;
//    private Quaternion initialSeaRotation;

//    void Start()
//    {
//        observerBehaviour = GetComponent<ObserverBehaviour>();
//        if (observerBehaviour)
//            observerBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;

//        // Save initial positions and rotations
//        initialPortPosition = Port.transform.position;
//        initialPortRotation = Port.transform.rotation;
//        initialSeaPosition = Sea.transform.position;
//        initialSeaRotation = Sea.transform.rotation;

//        shipMesh.enabled = true;
//    }
//    //private void Update()
//    //{
//    //    if (initialPositionFromDetection != null)
//    //    {

//    //        gameObject.transform.SetPositionAndRotation(initialPositionFromDetection.transform.position,
//    //            Quaternion.identity);


//    //    }
//    //}
//    private void EnableComponents()
//    {
//        shipMesh.enabled = true;
//        LikeSeaBoxCollider.enabled = true;
//        PortBoxCollider.enabled = true;
//        PortMeshRenderer.enabled = true;
//        WaterBlockMeshRenderer.enabled = true;
//        Terrain.enabled = true;
//        TerrainCollider.enabled = true;
//    }

//    private void OnEnable()
//    {
//        GetComponentInChildren<MeshCollider>().enabled = true;
//        shipMesh.enabled = true;
//    }
//    public static Status TargetStatus = Status.NO_POSE;
//    private bool isFirst = true;
//    private Transform initialPositionFromDetection;

//    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
//    {
//        // only if in right stage (allow detect) AND
//        // the ship hasn't been detected yet AND
//        // status is TRACKED
//        // SO - Mark as good detection 
//        TargetStatus = status.Status;
//        var mesh = GetComponentInChildren<MeshCollider>();
//        if (mesh != null) mesh.enabled = true;
//        if (shipMesh != null) shipMesh.enabled = true;
//        if (!IsDetected && status.Status == Status.TRACKED)
//        {
//            if (isFirst)
//            {
//                isFirst = false;
//            }
//            //observerBehaviour.enabled = false;
//            EnableComponents();

//            //VuforiaBehaviour.Instance.SetWorldCenter(WorldCenterMode.SPECIFIC_TARGET, observerBehaviour);
//            //Port.transform.SetParent(null);
//            //Sea.transform.SetParent(null);
//            //SeaObjects.transform.SetParent(null);
//            //StartCoroutine(HandleDelayedAction());
//            IsDetected = true;

//            GameManager.Instance.ShipDetected();

//            //// Preserve the current position and rotation of Port and Sea
//            //Port.transform.position = initialPortPosition;
//            //Port.transform.rotation = initialPortRotation;
//            //Sea.transform.position = initialSeaPosition;
//            //Sea.transform.rotation = initialSeaRotation;
//        }
//        else if (IsDetected)
//        {
//            // Ensure components remain active and in the correct place
//            Port?.SetActive(true);
//            Sea?.SetActive(true);


//            //// Restore the initial position and rotation
//            //Port.transform.position = initialPortPosition;
//            //Port.transform.rotation = initialPortRotation;
//            //Sea.transform.position = initialSeaPosition;
//            //Sea.transform.rotation = initialSeaRotation;
//        }

//    }
//    private IEnumerator HandleDelayedAction()
//    {
//        yield return new WaitForSeconds(5);
//        initialPositionFromDetection = gameObject.transform;

//        AllGameobjects.transform.SetParent(null);
//        gameObject.SetActive(false);
//    }
//    void EnableAllComponents(GameObject obj)
//    {
//        var components = obj.GetComponents<Component>();
//        foreach (var component in components)
//        {
//            if (component is Behaviour behaviourComponent)
//                behaviourComponent.enabled = true;
//        }

//        // for each child like sea ..
//        foreach (Transform child in obj.transform)
//            EnableAllComponents(child.gameObject);
//    }

//    void OnDestroy()
//    {
//        if (observerBehaviour)
//        {
//            observerBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
//        }
//    }
//}
