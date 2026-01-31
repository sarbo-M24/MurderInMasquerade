using UnityEngine;

// 10000 ensures this runs AFTER Physics, Input, Camera, and Cinemachine
[DefaultExecutionOrder(10000)]
public class BillboardFX : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        // 1. Get the direction the CAMERA is facing
        Vector3 direction = mainCamera.transform.forward;
        
        // 2. Flatten it (remove Up/Down tilt) so NPC stays upright
        direction.y = 0;

        // 3. Apply rotation (only if we have a valid direction)
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}