using UnityEngine;

public class SFPSC_FPSCamera : MonoBehaviour
{
    public float sensitivity = 2.0f;
    public Transform player;           // The Player Body
    public Transform CameraPosition;   // Empty object at player's head

    private float rotX = 0f;
    private float rotY = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. Check if game is over
        if (GunManager.isGameOver) return;

        // 2. Mouse look
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, -80f, 80f);
        rotY += mouseX;

        // Rotate camera up/down
        transform.localRotation = Quaternion.Euler(rotX, rotY, 0f);
        
        // Rotate body left/right
        player.Rotate(Vector3.up * mouseX);
    }

    void LateUpdate()
    {
        // 3. Glue camera to the player's head
        transform.position = CameraPosition.position;
    }
}