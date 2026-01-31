using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float damage = 10f;
    public float range = 100f;

    [Header("Visual Effects")]
    public GameObject tracerPrefab; // Drag your BulletTracer prefab here
    public Vector3 muzzleOffset = new Vector3(0.5f, -0.5f, 1f); // Offset from camera center

    public Camera fpsCam;

    void Update()
{
    if (Input.GetButtonDown("Fire1"))
    {
        if (GunManager.instance.isGunComplete && !GunManager.instance.hasFired)
        {
            Shoot();
            GunManager.instance.hasFired = true;
            
            // Disable UI after the shot
            if (GunManager.instance.gunUIElement != null) 
                GunManager.instance.gunUIElement.SetActive(false);
            
            if (GunManager.instance.crosshairUI != null) 
                GunManager.instance.crosshairUI.SetActive(false);
        }
    }
}

    void Shoot()
    {
        RaycastHit hit;
        Vector3 hitPoint;

        // The Raycast still starts exactly at the camera center for perfect aim
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            hitPoint = hit.point;
            Target enemy = hit.transform.GetComponent<Target>();
            if (enemy != null) enemy.TakeDamage(damage);
        }
        else
        {
            hitPoint = fpsCam.transform.position + (fpsCam.transform.forward * range);
        }

        SpawnTracer(hitPoint);
    }

    void SpawnTracer(Vector3 targetPos)
    {
        // Calculate a fake muzzle position in 3D space based on the camera
        // transform.TransformPoint converts the offset into "world space"
        Vector3 fakeMuzzlePos = fpsCam.transform.TransformPoint(muzzleOffset);

        GameObject tracerGO = Instantiate(tracerPrefab, fakeMuzzlePos, Quaternion.identity);
        TracerEffect tracer = tracerGO.GetComponent<TracerEffect>();
        
        tracer.Setup(fakeMuzzlePos, targetPos);
    }
}