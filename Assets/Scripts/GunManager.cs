using UnityEngine;
using UnityEngine.UI;

public class GunManager : MonoBehaviour
{
    public static GunManager instance;

    [Header("UI References")]
    public GameObject gunUIElement;   // The "Inventory" icon of the gun
    public GameObject crosshairUI;    // The actual aiming crosshair

    [Header("Collection State")]
    public int partsCollected = 0;
    public int totalPartsRequired = 4;
    
    [HideInInspector] public bool isGunComplete = false;
    [HideInInspector] public bool hasFired = false;

    void Awake()
    {
        if (instance == null) instance = this;
        
        // Hide both UI elements at the start
        if (gunUIElement != null) gunUIElement.SetActive(false);
        if (crosshairUI != null) crosshairUI.SetActive(false);
    }

    public void CollectPart()
    {
        partsCollected++;

        if (partsCollected >= totalPartsRequired)
        {
            isGunComplete = true;
            
            // Enable BOTH the icon and the crosshair
            if (gunUIElement != null) gunUIElement.SetActive(true);
            if (crosshairUI != null) crosshairUI.SetActive(true);
            
            Debug.Log("Gun complete! Aim with the crosshair.");
        }
    }
}