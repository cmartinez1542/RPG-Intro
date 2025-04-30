using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2 : MonoBehaviour
{
    [SerializeField] public float speed = 5f;

    public Rigidbody2D rb;
    public Animator anim;
    public int facingDirection = 1;

    public Joystick joystick; // Opcional para controles móviles
    public Player_Combat player_combat;

    private Vector2 movementInput = Vector2.zero;
    private bool slashed = false;

    public static int playerCount = 0; // Cuenta global de jugadores
    public SpriteRenderer spriteRenderer;
    private bool isKnockedBacked;
    public int attackRange;

    // Dash Settings
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    [SerializeField] private TrailRenderer tr;

  
    
   
   private void Start()
{
    rb = GetComponent<Rigidbody2D>();
    if (rb == null)
    {
        Debug.LogError("Rigidbody2D is missing on " + gameObject.name);
    }

    anim = GetComponent<Animator>();
    if (anim == null)
    {
        Debug.LogError("Animator is missing on " + gameObject.name);
    }

    player_combat = GetComponent<Player_Combat>();
    if (player_combat == null)
    {
        Debug.LogError("Player_Combat script is missing on " + gameObject.name);
    }

    playerCount++; // Incrementar conteo de jugadores para asignar atuendos
}


    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        Debug.Log(" Move Input Received: " + movementInput);
    }

    public void OnSlash(InputAction.CallbackContext context)
    {
        slashed = context.action.triggered;

        if (context.performed)
        {
            Debug.Log($" {gameObject.name} performed a slash!");
            player_combat.Attack();
        }

    }

    public void OnSmash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log($"{gameObject.name} performed Rock Smash!");
            player_combat.SmashAttack(); // or SmashAttack()
        }
    }
        

    private void FixedUpdate()
    {
        if (rb == null)
        {
            Debug.LogError(" Rigidbody2D is NULL in FixedUpdate!");
            return;
        }



        if (isKnockedBacked == false)
        {       
            
            // Entrada por teclado y joystick
            float horizontal = movementInput.x;
            float vertical = movementInput.y;

            rb.linearVelocity = new Vector2(horizontal * speed, vertical * speed);

            

            if (joystick != null)
            {
                horizontal += joystick.Direction.x;
                vertical += joystick.Direction.y;
            }

                    // Flip del sprite basado en dirección
            if (horizontal > 0 && facingDirection < 0)
                Flip();
            else if (horizontal < 0 && facingDirection > 0)
                Flip();

            anim.SetFloat("horizontal", Mathf.Abs(horizontal));
            anim.SetFloat("vertical", vertical);

            Vector2 move = new Vector2(horizontal, vertical).normalized;

            rb.linearVelocity = move * speed;
            


            Debug.Log($" Horizontal: {horizontal}");
            Debug.Log($" Vertical: {vertical}");
            Debug.Log($" Move direction: {move}");
            Debug.Log($" FixedUpdate - Applying velocity: {rb.linearVelocity}");
        }




        // Animaciones (opcional)
        /*
        anim.SetFloat("Speed", move.sqrMagnitude);
        */
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }


    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        true.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        true.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = true;
        yield return new WaitForSeconds(dashingCooldown);
        yield return new WaitForSeconds (dashingCooldown);
        canDash = true;
    }


public void Knockback(Transform enemy, float force, float stunTime)
{
    Debug.Log("Knockback triggered!");

    isKnockedBacked = true;

    Vector2 direction = (transform.position - enemy.position).normalized;

    // Aplica fuerza en forma de impulso
    rb.AddForce(direction * force, ForceMode2D.Impulse);

    StartCoroutine(EndKnockback(stunTime));
    
}

IEnumerator EndKnockback(float stunTime)
{
    yield return new WaitForSeconds(stunTime);
    isKnockedBacked = false;
}


    // Inicializar posición y atuendo
    public void InitializePlayer(Vector3 spawnPosition, Sprite outfit)
    {
        transform.position = spawnPosition;

        if (spriteRenderer != null && outfit != null)
        {
            spriteRenderer.sprite = outfit;
            Debug.Log($"Player {playerCount} assigned outfit: {outfit.name}");
        }
        else
        {
            Debug.LogWarning($"No sprite assigned to Player {playerCount}");
        }
    }
}


