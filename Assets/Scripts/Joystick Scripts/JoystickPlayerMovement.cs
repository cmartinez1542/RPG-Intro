using UnityEngine;

public class JoystickPlayerMovement : MonoBehaviour
{
    public Joystick joystick;
    public float speed = 5f;
    public Animator animator;
    private Rigidbody2D rb;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Get movement from joystick
        float moveX = joystick.Direction.x;
        float moveY = joystick.Direction.y;

        // Apply movement to Rigidbody
        rb.linearVelocity = new Vector2(moveX * speed, moveY * speed);

        // Update Animator
        animator.SetFloat("Speed", rb.linearVelocity.magnitude);
    }
}
