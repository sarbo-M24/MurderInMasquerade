using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Characters")]
    public GameObject imposterPrefab; // Keeps Imposter spawning

    [Header("Spawn Settings")]
    public Transform spawnPointsParent;

    // We store all points to pick a start spot, then track empty ones
    private List<Transform> allSpawnPoints = new List<Transform>();
    private List<Transform> emptySpawnPoints = new List<Transform>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CollectSpawnPoints();
        SpawnImposterOnly();
    }

    void CollectSpawnPoints()
    {
        foreach (Transform child in spawnPointsParent)
        {
            allSpawnPoints.Add(child);
        }
    }

    void SpawnImposterOnly()
    {
        // Safety Check
        if (allSpawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points found in parent!");
            return;
        }

        // 1. Shuffle or Pick Random Spot
        // We pick a random index for the Imposter's start position
        int startingIndex = Random.Range(0, allSpawnPoints.Count);
        Transform startSpot = allSpawnPoints[startingIndex];

        // 2. Spawn the Imposter
        GameObject imposter = Instantiate(imposterPrefab, startSpot.position, Quaternion.identity);

        // 3. Initialize Controller
        ImposterController controller = imposter.GetComponent<ImposterController>();
        if (controller != null)
        {
            controller.currentSpawnPoint = startSpot;
        }

        // 4. Register the Rest as Empty
        // Loop through all points. If it's NOT the start spot, it's empty.
        for (int i = 0; i < allSpawnPoints.Count; i++)
        {
            if (i != startingIndex)
            {
                emptySpawnPoints.Add(allSpawnPoints[i]);
            }
        }
    }

    // --- TELEPORT LOGIC (Unchanged) ---
    public Transform GetHiddenTeleportSpot(Transform currentSpotToFree)
    {
        if (emptySpawnPoints.Count == 0) return null;

        Camera cam = Camera.main;
        List<Transform> validHiddenSpots = new List<Transform>();

        // Filter: Find empty spots that are NOT currently visible
        foreach (Transform spot in emptySpawnPoints)
        {
            if (!IsPointOnScreen(cam, spot.position))
            {
                validHiddenSpots.Add(spot);
            }
        }

        if (validHiddenSpots.Count == 0) return null;

        int randomIndex = Random.Range(0, validHiddenSpots.Count);
        Transform newSpot = validHiddenSpots[randomIndex];

        // Swap Logic
        emptySpawnPoints.Add(currentSpotToFree);
        emptySpawnPoints.Remove(newSpot);

        return newSpot;
    }

    bool IsPointOnScreen(Camera cam, Vector3 worldPos)
    {
        Vector3 viewPos = cam.WorldToViewportPoint(worldPos);
        bool onScreen = (viewPos.x > 0 && viewPos.x < 1 &&
                         viewPos.y > 0 && viewPos.y < 1 &&
                         viewPos.z > 0);
        return onScreen;
    }
}