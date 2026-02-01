using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public GameObject tracerPrefab;
    public Vector3 muzzleOffset = new Vector3(0.4f, -0.4f, 1.0f);
    public Camera fpsCam;

    void Update()
    {
        // 1. Disable shooting if Game Over
        if (GunManager.isGameOver) return;

        if (Input.GetButtonDown("Fire1"))
        {
            if (GunManager.instance.isGunComplete && !GunManager.instance.hasFired)
            {
                Shoot();
                GunManager.instance.hasFired = true;
                
                // 2. Turn off UI immediately
                ToggleUI(false);
            }
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        Vector3 hitPoint;

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
        Vector3 fakeMuzzlePos = fpsCam.transform.TransformPoint(muzzleOffset);
        GameObject tracerGO = Instantiate(tracerPrefab, fakeMuzzlePos, Quaternion.identity);
        tracerGO.GetComponent<TracerEffect>().Setup(fakeMuzzlePos, targetPos);
    }

    void ToggleUI(bool state)
    {
        if (GunManager.instance.gunUIElement) GunManager.instance.gunUIElement.SetActive(state);
        if (GunManager.instance.crosshairUI) GunManager.instance.crosshairUI.SetActive(state);
        if (GunManager.instance.timerCircle) GunManager.instance.timerCircle.gameObject.SetActive(state);
    }
}