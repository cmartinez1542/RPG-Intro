using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public Rigidbody2D rb;
    public Animator anim;
    public int facingDirection = 1;

    public Joystick joystick; // Add Joystick reference
    public Player_Combat player_combat;
    public bool isButtonPressed = false; // Flag to track button state
    
    public float pressStartTime = 0f;

    public float pressDuration= 0f;

    public UIButtonPressHandler attackButtonHandler;

    private Vector2 movementInput;
    private bool slashed = false;
    private CharacterController controller;
    public SpriteRenderer spriteRenderer;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D is missing on " + gameObject.name);
        }

        player_combat = GetComponent<Player_Combat>();

        if (player_combat == null)
        {
            Debug.LogError("Player_Combat script is missing on " + gameObject.name);
        }
    }

    public void OnMove(InputAction.CallbackContext context) {
        movementInput = context.ReadValue<Vector2>();
        Debug.Log("Move Input: " + movementInput);

    }

    public void OnSlash(InputAction.CallbackContext context){
        slashed = context.ReadValue<bool>();
        slashed = context.action.triggered;
        if (context.performed)
        {
            Debug.Log($"{gameObject.name} performed a slash!");
            player_combat.Attack();
            // Add attack animation or effect here
        }
        
    }

    private void Update()
    {
       /* if (attackButtonHandler != null)
        {
            if (attackButtonHandler.isPressed == false && attackButtonHandler.pressDuration > 0.75f)
            {
                player_combat.SecondAttack();
                attackButtonHandler.pressDuration = 0;
                
                // Button is currently pressed; 
                //Debug.Log("Button is held down for " + attackButtonHandler.currentDuration);// 
            }
            else if (attackButtonHandler.isPressed == false && attackButtonHandler.pressDuration  > 0f &&  attackButtonHandler.pressDuration < 0.3f || slashed)
            {
                
                player_combat.Attack();
                attackButtonHandler.pressDuration = 0;
            }
        }
        */
    }
    
    // FixedUpdate is called 50x per frame
    void FixedUpdate()
    {
        // Get input from both keyboard and joystick
        float horizontal = Input.GetAxis("Horizontal") + joystick.Direction.x + movementInput.x ;
        float vertical = Input.GetAxis("Vertical") + joystick.Direction.y + movementInput.y;

        // Handle flipping for left/right movement
        /*
        if ((horizontal > 0 && transform.localScale.x < 0) || 
            (horizontal < 0 && transform.localScale.x > 0))
        {
            Flip();
        }

        // Update Animator with movement input
        anim.SetFloat("horizontal", Mathf.Abs(horizontal));
        anim.SetFloat("vertical", Mathf.Abs(vertical));
        */

        // Move player using Rigidbody
        //controller.Move(PlayerMovement * Time.deltaTime * speed);
        movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (rb != null)
        {
            rb.linearVelocity = movementInput * speed;
            Debug.Log("FixedUpdate - Applying velocity: " + rb.linearVelocity);
        }
        rb.linearVelocity = new Vector2(horizontal, vertical).normalized * speed; // Normalized for consistent diagonal speed
        Debug.Log("FixedUpdate - Applying velocity: " + rb.linearVelocity);
        /* 
        if(joystick.dash && facingDirection ==1 )
        {
            rb.linearVelocity = new Vector3(horizontal += 200, vertical);
            new WaitForSeconds(0.3f);
            joystick.dash = false;
            
        }
        else if (joystick.dash && facingDirection == -1 )
        {
            rb.linearVelocity = new Vector3(horizontal -= 200, vertical);
            new WaitForSeconds(0.3f);
            joystick.dash = false;
        }
        */
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

        // Custom method for initializing position and sprite
    public void InitializePlayer(Vector3 spawnPosition, Sprite outfit)
    {
        // Set Rigidbody2D position (recommended for physics objects)
        rb.position = spawnPosition;

        // Assign sprite (outfit)
        if (spriteRenderer != null && outfit != null)
        {
            spriteRenderer.sprite = outfit;
        }
        else
        {
            Debug.LogWarning("SpriteRenderer or Outfit sprite not assigned.");
        }
    }

   /* public void OnSlash(InputAction.CallbackContext context)
    {
        if (context.performed)
            {
            Debug.Log("Slash Attack Triggered!");
            // Add attack logic here
            }
    } */
}

