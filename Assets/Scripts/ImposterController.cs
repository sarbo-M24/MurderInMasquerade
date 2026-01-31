using UnityEngine;

public class ImposterController : MonoBehaviour
{
    [HideInInspector]
    public Transform currentSpawnPoint; // Assigned by GameManager on spawn

    private Renderer m_Renderer;
    private Camera mainCamera;
    
    // Logic flags
    private bool readyToTeleport = false; 
    private bool hasBeenSeenOnce = false; 

    void Start()
    {
        m_Renderer = GetComponentInChildren<Renderer>();
        mainCamera = Camera.main;

        if (m_Renderer == null)
        {
            Debug.LogError("Imposter needs a Renderer to detect visibility!");
        }
    }

    void Update()
    {
        if (m_Renderer == null) return;

        // Check visibility (Only Frustum check, no Raycast)
        bool currentlyVisible = IsVisibleToCamera();

        if (currentlyVisible)
        {
            // Player is looking in the direction of the Imposter
            // (Even if behind a wall)
            hasBeenSeenOnce = true;
            readyToTeleport = true; 
        }
        else
        {
            // Player is looking AWAY
            if (hasBeenSeenOnce && readyToTeleport)
            {
                Teleport();
                readyToTeleport = false;
            }
        }
    }

    void Teleport()
    {
        if (GameManager.Instance == null) return;

        // We still use GetHiddenTeleportSpot to ensure he doesn't 
        // teleport to a spot currently visible on the screen.
        Transform newLocation = GameManager.Instance.GetHiddenTeleportSpot(currentSpawnPoint);

        if (newLocation != null)
        {
            transform.position = newLocation.position;
            currentSpawnPoint = newLocation;
            Debug.Log("Imposter teleported!");
        }
    }

    bool IsVisibleToCamera()
    {
        // Calculate the camera's viewing frustum (the pyramid of view)
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        
        // Check if the Imposter's renderer bounds are inside that frustum
        return GeometryUtility.TestPlanesAABB(planes, m_Renderer.bounds);
    }
}