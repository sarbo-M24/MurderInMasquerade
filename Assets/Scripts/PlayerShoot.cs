using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 0.5f;
    private float nextTimeToFire = 0f;

    [Header("References")]
    public Camera fpsCam; // Drag your Main Camera here in the Inspector

    void Update()
    {
        // Check if the fire button (Left Mouse) is pressed and cooldown is over
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;

        // Origin: Camera Position
        // Direction: Camera's Forward Vector
        // 'out hit': Stores the collision data
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log("We hit: " + hit.transform.name);

            // Attempt to find the Target script on the object we hit
            Target enemy = hit.transform.GetComponent<Target>();
            
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        
        // VISUAL DEBUG: Draws a red line in the SCENE view so you can see the ray
        Debug.DrawRay(fpsCam.transform.position, fpsCam.transform.forward * range, Color.red, 1.0f);
    }
}