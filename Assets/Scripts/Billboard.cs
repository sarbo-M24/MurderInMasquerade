using UnityEngine;

public class BillboardFX : MonoBehaviour
{
    private Transform playerTransform;

    void Start()
    {
        // 1. Find the Player automatically by Tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError("BillboardFX could not find an object tagged 'Player'!");
        }
    }

    void LateUpdate()
    {
        if (playerTransform == null) return;

        // 2. Calculate direction from NPC to Player
        Vector3 directionToPlayer = playerTransform.position - transform.position;

        // 3. Lock the Y-axis (so they don't tilt up/down)
        directionToPlayer.y = 0;

        // 4. Rotate to face the player
        if (directionToPlayer != Vector3.zero)
        {
            // Create the rotation looking at the player
            Quaternion lookRotation = Quaternion.LookRotation(-directionToPlayer);

            // Apply it
            transform.rotation = lookRotation;
        }
    }
}