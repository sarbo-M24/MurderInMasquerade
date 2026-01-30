using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SFPSC_PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private bool enableMovement = true;

    [Header("Movement properties")]
    public float walkSpeed = 8.0f;
    public float runSpeed = 12.0f;
    public float changeInStageSpeed = 10.0f; // Transition smoothness
    public float maximumPlayerSpeed = 150.0f;
    
    private float vInput, hInput;
    private Vector3 inputForce;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Constrain rotation so the player doesn't tip over
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (!enableMovement) return;

        // 1. Capture Input every frame (Update is more responsive for input)
        vInput = Input.GetAxisRaw("Vertical");
        hInput = Input.GetAxisRaw("Horizontal");

        // 2. Determine speed (Shift to run)
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // 3. Calculate direction based on where the player is facing
        inputForce = (transform.forward * vInput + transform.right * hInput).normalized * currentSpeed;
    }

    private void FixedUpdate()
    {
        // Apply the movement in FixedUpdate for consistent physics
        if (!enableMovement) return;

        // Clamping overall velocity to the maximum allowed
        rb.linearVelocity = ClampMag(rb.linearVelocity, maximumPlayerSpeed);

        // Apply movement using Lerp for a smooth "weighty" feel
        // We maintain the current Y velocity so gravity still works
        Vector3 targetVelocity = new Vector3(inputForce.x, rb.linearVelocity.y, inputForce.z);
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, changeInStageSpeed * Time.fixedDeltaTime);
    }

    private static Vector3 ClampMag(Vector3 vec, float maxMag)
    {
        if (vec.sqrMagnitude > maxMag * maxMag)
            vec = vec.normalized * maxMag;
        return vec;
    }

    public void EnableMovement() => enableMovement = true;
    public void DisableMovement() => enableMovement = false;
}