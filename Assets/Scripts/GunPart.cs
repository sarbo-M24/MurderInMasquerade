using UnityEngine;

public class GunPart : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Ensure the Player object has the tag "Player"
        if (other.CompareTag("Player"))
        {
            GunManager.instance.CollectPart();
            Destroy(gameObject); // Remove part from world
        }
    }
}