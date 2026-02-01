using UnityEngine;

public class ImposterController : MonoBehaviour
{
    [HideInInspector]
    public Transform currentSpawnPoint; // Assigned by GameManager on spawn

    [Header("Line of Sight Settings")]
    [SerializeField] private LayerMask obstacleMask; // Assign layers that block vision (walls, etc.)
    [SerializeField] private float maxViewDistance = 100f; // Max distance to check for LOS

    private Renderer m_Renderer;
    private Camera mainCamera;

    // Logic flags
    private bool readyToTeleport = false;
    private bool hasBeenSeenOnce = false;
    private bool hadDirectLineOfSight = false; // New flag to track if player has clear view

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

        // Check if in camera frustum first (cheaper check)
        bool inFrustum = IsVisibleToCamera();

        // Only do raycast if in frustum
        bool hasDirectLOS = false;
        if (inFrustum)
        {
            hasDirectLOS = HasDirectLineOfSight();
        }

        if (hasDirectLOS)
        {
            // Player is looking DIRECTLY at the Imposter with NO obstacles
            hasBeenSeenOnce = true;
            hadDirectLineOfSight = true;
            readyToTeleport = false; // Reset teleport flag while being watched
        }
        else
        {
            // Either not in frustum OR there's an obstacle blocking view
            if (hasBeenSeenOnce && hadDirectLineOfSight)
            {
                // Player looked away or obstacle appeared
                readyToTeleport = true;
            }
        }

        // Teleport when ready and player is no longer looking
        if (readyToTeleport && !hasDirectLOS)
        {
            Teleport();
            readyToTeleport = false;
            hadDirectLineOfSight = false;
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
            hasBeenSeenOnce = false; // Reset for new location
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

    bool HasDirectLineOfSight()
    {
        // Get center of imposter's bounds for more accurate check
        Vector3 imposterCenter = m_Renderer.bounds.center;
        Vector3 cameraPosition = mainCamera.transform.position;

        // Direction from camera to imposter
        Vector3 directionToImposter = imposterCenter - cameraPosition;
        float distanceToImposter = directionToImposter.magnitude;

        // Check if within max view distance
        if (distanceToImposter > maxViewDistance)
            return false;

        // Cast ray from camera to imposter
        if (Physics.Raycast(cameraPosition, directionToImposter.normalized, out RaycastHit hit, distanceToImposter, obstacleMask))
        {
            // Something is blocking the view
            // Check if what we hit is the imposter itself (or its children)
            if (hit.collider.transform.IsChildOf(transform) || hit.collider.transform == transform)
            {
                // We hit the imposter directly, no obstacles
                return true;
            }

            // Hit something else - obstacle is blocking
            return false;
        }

        // No obstacles detected in the ray, player has clear line of sight
        return true;
    }

    // Optional: Visualize the raycast in Scene view for debugging
    void OnDrawGizmos()
    {
        if (mainCamera == null || m_Renderer == null) return;

        Vector3 imposterCenter = m_Renderer.bounds.center;
        Vector3 cameraPosition = mainCamera.transform.position;

        // Draw line from camera to imposter
        if (HasDirectLineOfSight())
        {
            Gizmos.color = Color.green; // Clear line of sight
        }
        else
        {
            Gizmos.color = Color.red; // Blocked
        }

        Gizmos.DrawLine(cameraPosition, imposterCenter);
    }
}
