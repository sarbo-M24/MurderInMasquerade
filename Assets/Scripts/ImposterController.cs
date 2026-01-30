using UnityEngine;

public class ImposterController : MonoBehaviour
{
    [HideInInspector]
    public Transform currentSpawnPoint; // Assigned by GameManager on spawn

    private Renderer m_Renderer;
    private Camera mainCamera;
    
    // The boolean required by your logic
    private bool readyToTeleport = false; 
    
    // To ensure he doesn't move before the game really starts
    private bool hasBeenSeenOnce = false; 

    void Start()
    {
        m_Renderer = GetComponentInChildren<Renderer>();
        mainCamera = Camera.main;

        if (m_Renderer == null)
        {
            Debug.LogError("Imposter needs a Renderer (MeshRenderer or SkinnedMeshRenderer) to detect visibility!");
        }
    }

    void Update()
    {
        if (m_Renderer == null) return;

        bool currentlyVisible = IsVisibleToCamera();

        if (currentlyVisible)
        {
            // 1. Player is looking at Imposter. 
            // He freezes, but becomes "Charged" (ready) to teleport when looked away.
            hasBeenSeenOnce = true;
            readyToTeleport = true; 
        }
        else
        {
            // 2. Player is NOT looking.
            // If we have been seen at least once, AND we are charged to teleport...
            if (hasBeenSeenOnce && readyToTeleport)
            {
                Teleport();
                
                // IMPORTANT: We set this to false immediately.
                // This ensures he teleports ONLY ONCE until looked at again.
                readyToTeleport = false;
            }
        }
    }

    void Teleport()
    {
        if (GameManager.Instance == null) return;

        // Ask GameManager for a valid spot and swap the lists
        Transform newLocation = GameManager.Instance.GetNewTeleportSpot(currentSpawnPoint);

        if (newLocation != null)
        {
            // Update physical position
            transform.position = newLocation.position;
            
            // Update our internal reference
            currentSpawnPoint = newLocation;
            
            Debug.Log("Imposter teleported!");
        }
    }

    // Helper to strictly check if Main Camera sees the object
    // (Renderer.isVisible can be buggy in Editor if Scene View is open)
    bool IsVisibleToCamera()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        return GeometryUtility.TestPlanesAABB(planes, m_Renderer.bounds);
    }
}