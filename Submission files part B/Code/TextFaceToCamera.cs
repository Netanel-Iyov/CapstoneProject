using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this script to each text mesh to keep it in front of the camera.
/// </summary>
public class TextFaceToCamera : MonoBehaviour
{
    public Transform standTransform; // The transform of the stand
    public Vector3 frontOffset = new Vector3(0, 0, 0); // Offset when the user is in front of the stand
    public Vector3 backOffset = new Vector3(0, 0, -0.27f); // Offset when the user is behind the stand
    public Vector3 rightOffset = new Vector3(1.24f, 0, 0); // Offset when the user is to the right of the stand
    public Vector3 leftOffset = new Vector3(-1.24f, 0, 0); // Offset when the user is to the left of the stand

    void Update()
    {
        Vector3 userPosition = Camera.main.transform.position;
        Vector3 standPosition = standTransform.position;
        Vector3 directionToUser = userPosition - standPosition;
        float angle = Vector3.SignedAngle(directionToUser, standTransform.forward, Vector3.up);

        if (angle > -45 && angle <= 45)
        {
            // User is in front of the stand
            transform.position = standPosition + frontOffset;
            transform.rotation = Quaternion.LookRotation(standTransform.forward);
        }
        else if (angle > 45 && angle <= 135)
        {
            // User is to the right of the stand
            transform.position = standPosition + rightOffset;
            transform.rotation = Quaternion.LookRotation(standTransform.right);
        }
        else if (angle > -135 && angle <= -45)
        {
            // User is to the left of the stand
            transform.position = standPosition + leftOffset;
            transform.rotation = Quaternion.LookRotation(-standTransform.right);
        }
        else
        {
            // User is behind the stand
            transform.position = standPosition + backOffset;
            transform.rotation = Quaternion.LookRotation(-standTransform.forward);
        }
    }
}
