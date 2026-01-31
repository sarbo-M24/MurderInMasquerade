using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SFPSC_PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private bool enableMovement = true;

    [Header("References")]
    public Transform playerCamera; // Drag your Main Camera here in the Inspector

    [Header("Movement properties")]
    public float walkSpeed = 8.0f;
    public float runSpeed = 12.0f;
    public float changeInStageSpeed = 10.0f; 
    public float maximumPlayerSpeed = 150.0f;
    
    private float vInput, hInput;
    private Vector3 inputForce;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Automatically try to find the camera if not assigned
        if (playerCamera == null && Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }
    }

    private void Update()
    {
        if (!enableMovement) return;

        vInput = Input.GetAxisRaw("Vertical");
        hInput = Input.GetAxisRaw("Horizontal");

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // 1. Get camera directions
        Vector3 camForward = playerCamera.forward;
        Vector3 camRight = playerCamera.right;

        // 2. Flatten directions (ignore Y) so player doesn't fly/sink when looking up/down
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // 3. Calculate move direction relative to camera
        inputForce = (camForward * vInput + camRight * hInput).normalized * currentSpeed;
    }

    private void FixedUpdate()
    {
        if (!enableMovement) return;

        // Apply movement while preserving gravity (Y velocity)
        Vector3 targetVelocity = new Vector3(inputForce.x, rb.linearVelocity.y, inputForce.z);
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, changeInStageSpeed * Time.fixedDeltaTime);

        // Clamp speed
        if (rb.linearVelocity.magnitude > maximumPlayerSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maximumPlayerSpeed;
        }
    }

    public void EnableMovement() => enableMovement = true;
    public void DisableMovement() => enableMovement = false;
}