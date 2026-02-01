using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SFPSC_PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    public Camera fpsCam; // Reference to your Main Camera
    public float walkSpeed = 8.0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; 
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void FixedUpdate()
    {
        // 1. Disable movement on Game Over
        if (GunManager.isGameOver)
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            return;
        }

        MovePlayer();
    }

    private void MovePlayer()
    {
        float vInput = Input.GetAxisRaw("Vertical");
        float hInput = Input.GetAxisRaw("Horizontal");

        // 2. Calculate direction based on Camera Angle
        // We take the camera's forward and right, but zero out the Y to prevent 'flying'
        Vector3 camForward = fpsCam.transform.forward;
        Vector3 camRight = fpsCam.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * vInput + camRight * hInput).normalized;

        // 3. Apply velocity
        rb.linearVelocity = new Vector3(moveDir.x * walkSpeed, rb.linearVelocity.y, moveDir.z * walkSpeed);
    }
}