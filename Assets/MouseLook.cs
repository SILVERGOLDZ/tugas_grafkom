using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;  // Drag character root here
    public Transform cameraRoot;  // Drag CameraRoot here

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // Hides/locks cursor
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;  // Pitch (up/down)
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraRoot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);  // Rotate camera X
        Vector3 currentRotation = playerBody.eulerAngles;
        currentRotation.y += mouseX;
        playerBody.eulerAngles = currentRotation; // Rotate body Y (yaw)
    }
}