using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement2 : MonoBehaviour
{
    [SerializeField] public float speed = 5f;
    public AudioManager audiomanager; // audiomanager.PlayDash();
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
    public bool isDashing;
    public float dashingPower = 100f;
    private float dashingTime = 0.4f;
    private float dashingCooldown = 1f;
    public ParticleSystem dust;

    private Vector2 lastDirection = Vector2.right; 



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

    playerCount++; 
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
        
    public void OnDash(InputAction.CallbackContext context)
    {

        if (context.performed && canDash && !isDashing)
        {
            Debug.Log("Dash triggered!");
            StartCoroutine(Dash());
            audiomanager.PlayDash();
        }
    }

    private void FixedUpdate()
    {
        if (rb == null)
        {
            Debug.LogError(" Rigidbody2D is NULL in FixedUpdate!");
            return;
        }



        if (!isKnockedBacked && !isDashing)
        {       
            // Entrada por teclado y joystick
            float horizontal = movementInput.x;
            float vertical = movementInput.y;

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

            if (move != Vector2.zero)
            {
                lastDirection = move;
            }

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
        dust.Play();
    
    }



    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;


        rb.linearVelocity = lastDirection.normalized * dashingPower;
        
        
        //rb.AddForce(lastDirection * dashingPower, ForceMode2D.Impulse);
        tr.emitting = true;

        yield return new WaitForSeconds(dashingTime);

        tr.emitting = false;       
        //rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }


public void Knockback(Transform enemy, float force, float stunTime)
{
    Debug.Log("Knockback triggered!");

    isKnockedBacked = true;

    dust.Play();

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

        public void OnCharge(InputAction.CallbackContext context)
    {
        Player_Magic magic = GetComponent<Player_Magic>();

        if (context.performed)
        {
            magic.StartCharging();
        
        }

        if (context.canceled)
        {
            magic.StopCharging();
        }
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


