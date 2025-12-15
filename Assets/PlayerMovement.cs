using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    public bool canMove = true;

    private float yVelocity = 0f;
    private bool isGrounded = true;

    void Update()
    {
        if (!canMove) return;

        // --- INPUT ---
        float x = Input.GetAxisRaw("Horizontal");  // A/D or Left/Right Arrow
        float z = Input.GetAxisRaw("Vertical");    // W/S or Up/Down Arrow

        // Movement direction relative to player orientation
        Vector3 move = transform.right * x + transform.forward * z;

        // Prevent faster diagonal movement
        if (move.magnitude > 1f)
            move.Normalize();

        // --- SPRINT ---
        float speed = walkSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
            speed = sprintSpeed;

        // --- HORIZONTAL MOVEMENT ---
        Vector3 pos = transform.position;
        pos += move * speed * Time.deltaTime;

        // --- GRAVITY ---
        if (isGrounded && yVelocity < 0)
            yVelocity = -2f;

        yVelocity += gravity * Time.deltaTime;

        // --- JUMP ---
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            yVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isGrounded = false;
        }

        // --- APPLY VERTICAL MOVEMENT ---
        pos.y += yVelocity * Time.deltaTime;

        // --- SIMPLE FLOOR COLLISION (ground at y = 0) ---
        if (pos.y <= 0f)
        {
            pos.y = 0f;
            yVelocity = 0f;
            isGrounded = true;
        }

        // --- FINAL MOVEMENT APPLY ---
        transform.position = pos;
    }
}
