using UnityEngine;
using System.Collections;
using System.Xml.Serialization;

public class DraggableCube : MonoBehaviour
{
    private Vector3 offset;
    private Camera mainCamera;
    private bool dragging = false;
    private Vector3 initialPosition;
    private Rigidbody rb;
    private bool isAlreadyAtPort = false;
    [SerializeField] public AudioClip splashSound;
    [SerializeField] public AudioClip boxToPortSounds;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    void OnMouseDown()
    {
        offset = gameObject.transform.position - GetMouseWorldPosition();
        dragging = true;
        initialPosition = transform.position;
        rb.isKinematic = true; // disable physics during dragging
        rb.freezeRotation = true;  // disable rotation during draggin
    }

    void OnMouseDrag()
    {
        if (dragging)
        {
            Vector3 newPosition = GetMouseWorldPosition() + offset;
            transform.position = newPosition;
        }
    }

    void OnMouseUp()
    {
        dragging = false;
        rb.isKinematic = false; // enable physics
        rb.freezeRotation = false; // enable rotation (maybe the line above is enough)

        // insert waiting here
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Port"))
        {
            SoundManager.Instance.PlayAudio(boxToPortSounds);
            //SoundManager.Instance.PlayBeep(1, 600, 0.1f);
            if (!isAlreadyAtPort)
                FindAnyObjectByType<Stage1Controller>().OnDropCommo();
            isAlreadyAtPort = true;
        }
        else if (collision.collider.CompareTag("LikeSea"))
        {
            //SoundManager.Instance.PlayBeep(1, 200, 0.05f);
            SoundManager.Instance.PlayAudio(splashSound);
            rb.isKinematic = false;
            transform.position = initialPosition; // Return to initial position
            rb.isKinematic = true;
        }
    }


    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mainCamera.WorldToScreenPoint(gameObject.transform.position).z;

        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
}
