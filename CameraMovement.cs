using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of camera movement

    void Update()
    {
        float horizontalInput = 0f;
        float verticalInput = 0f;

        // Check for WASD keys for movement
        if (Input.GetKey(KeyCode.W))
        {
            verticalInput = 1f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            verticalInput = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            horizontalInput = 1f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            horizontalInput = -1f;
        }

        // Normalize the movement vector to ensure consistent speed diagonally
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // Translate the camera based on the move direction, speed, and deltaTime
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.Self);
    }
}