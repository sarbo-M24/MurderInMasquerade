using UnityEngine;

public class ImposterController : MonoBehaviour
{
    [HideInInspector]
    public Transform currentSpawnPoint;

    private Renderer m_Renderer;
    private Camera mainCamera;
    
    private bool readyToTeleport = false; 
    private bool hasBeenSeenOnce = false; 

    // LayerMask to ensure Raycast hits walls (Default) but ignores triggers/UI if needed
    // You can set this in Inspector, or default to Everything
    public LayerMask obstacleMask = -1; 

    void Start()
    {
        m_Renderer = GetComponentInChildren<Renderer>();
        mainCamera = Camera.main;

        if (m_Renderer == null)
            Debug.LogError("Imposter needs a Renderer!");
    }

    void Update()
    {
        if (m_Renderer == null) return;

        bool currentlyVisible = IsVisibleToCamera();

        if (currentlyVisible)
        {
            // Player sees Imposter clearly (in frame AND no walls)
            hasBeenSeenOnce = true;
            readyToTeleport = true; 
        }
        else
        {
            // Player is NOT looking
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

        // Ask GameManager for a spot that is NOT currently visible
        Transform newLocation = GameManager.Instance.GetHiddenTeleportSpot(currentSpawnPoint);

        if (newLocation != null)
        {
            transform.position = newLocation.position;
            currentSpawnPoint = newLocation;
            Debug.Log("Imposter teleported to a hidden spot!");
        }
    }

    bool IsVisibleToCamera()
    {
        // 1. Frustum Check (Is it within the camera angle?)
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        if (!GeometryUtility.TestPlanesAABB(planes, m_Renderer.bounds))
        {
            return false; // Not in camera angle
        }

        // 2. Raycast Check (Is a wall blocking the view?)
        // We cast a line from Camera to Imposter's center
        RaycastHit hit;
        Vector3 direction = m_Renderer.bounds.center - mainCamera.transform.position;
        
        if (Physics.Raycast(mainCamera.transform.position, direction, out hit, Mathf.Infinity, obstacleMask))
        {
            // If we hit something that is NOT the imposter (like a wall), he is hidden
            // Note: Ensure the Imposter's collider is on the same object or child being hit
            if (hit.transform.root != this.transform.root)
            {
                return false; // Blocked by wall
            }
        }

        return true; // In angle and clear line of sight
    }
}