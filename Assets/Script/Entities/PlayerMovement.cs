using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        // Initialize Rigidbody2D component reference
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get movement input (WASD / Arrow keys)
        moveInput.x = Input.GetAxisRaw("Horizontal"); // A/D or ←/→
        moveInput.y = Input.GetAxisRaw("Vertical");   // W/S or ↑/↓

        // Normalize to avoid faster diagonal speed
        moveInput.Normalize();
    }

    void FixedUpdate()
    {
        // Apply velocity for smooth physics-based movement
        rb.linearVelocity = moveInput * moveSpeed;
    }

    // === Upgrade hook ===
    // Multiplies current move speed (e.g., 1.05f = +5%)
    public void AddSpeedMultiplier(float multiplier)
    {
        moveSpeed *= multiplier;     // uses moveSpeed
        Debug.Log($"Speed updated to: {moveSpeed}");
    }

    // Returns current movement speed (used by UpgradeStatusPanel)
    public float CurrentSpeed => moveSpeed;
}