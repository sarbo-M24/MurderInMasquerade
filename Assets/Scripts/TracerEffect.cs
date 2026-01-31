using UnityEngine;

public class TracerEffect : MonoBehaviour
{
    private LineRenderer line;
    public float lifeTime = 0.05f; // How long the tracer stays visible

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        Destroy(gameObject, lifeTime); // Automatically clean up the tracer
    }

    public void Setup(Vector3 start, Vector3 end)
    {
        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }
}