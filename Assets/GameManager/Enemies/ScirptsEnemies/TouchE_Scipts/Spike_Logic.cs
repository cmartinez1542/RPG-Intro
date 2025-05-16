using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike_Logic : MonoBehaviour
{
    private bool isKnockedBack = false;

    [Header("Attack Settings")]
    public float attackCooldown = 1f;
    private bool isAttacking = false;

    public int damage = 1;
    public Transform attackPoint;
    public float weaponRange = 1f;
    public float knockbackForce = 4f;
    public float stunTime = 1f;

    public bool missDashing = false;

    private Animator anim;
    private Rigidbody2D rb;

    private bool isTriggered = false; // To prevent multiple coroutine calls
    public AudioManager audiomanager;

    [Header("Follow Player Settings")]
    public Transform player;
    public float detectionRange = 5f;
    public float stopDistance = 1.5f;
    public float moveSpeed = 2f;

    [Header("Stun Settinngs")]
    public SpriteRenderer spriteRenderer; // Drag your enemy's SpriteRenderer in Inspector
    public Vector3 attackPointOffset = new Vector3(1.5f, 0f, 0f);

    private void Start() {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if(!missDashing) { //If this is not flagged it to miss if the player dashes, then leave the spikes as up
            anim.SetBool("AlwaysOut",true);
            anim.SetBool("PlayAnimation",true);
        }
    }

    private void Update() {
        if (player == null) { //If player does not exist, then find the objects with the Player label and get the closest player
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length > 0)
                player = GetClosestPlayer(players).transform;
        }
    }

    //Get the closest player
    private GameObject GetClosestPlayer(GameObject[] players) {
        GameObject closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject p in players) {
            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist < minDistance) {
                minDistance = dist;
                closest = p;
            }
        }

        return closest;
    }

    private void FixedUpdate() {
        float speed = rb.linearVelocity.magnitude;
        anim.SetFloat("Speed", speed);

        if (isKnockedBack) return; // ⛔ No moverse si está siendo empujado

        if (player != null) {
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= detectionRange && distance > stopDistance) {
                Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
                rb.linearVelocity = direction * moveSpeed;
            } else if (distance <= stopDistance) {
                rb.linearVelocity = Vector2.zero;

                if (!isAttacking)
                    StartCoroutine(AttackPlayer());
            }
            else
                rb.linearVelocity = Vector2.zero;
        }
    }

    //Actions to do when colliding
    private void OnCollisionEnter2D(Collision2D collision) {
        Player_Health playerHealth = collision.gameObject.GetComponent<Player_Health>();
        PlayerMovement2 playerMovement = collision.gameObject.GetComponent<PlayerMovement2>();

        //Deal damage to player if health exist
        if (playerHealth != null)
            playerHealth.ChangeHealth(-damage);

        //Deal knockback to player if movement exist
        if (playerMovement != null)
            playerMovement.Knockback(transform, knockbackForce, stunTime);
    }

    //Execute attack if stepped on
    private void OnTriggerStay2D(Collider2D collision) {
        //If it is not triggered
        if(!isTriggered) {
            Player_Health playerHealth = collision.gameObject.GetComponent<Player_Health>();
            PlayerMovement2 playerMovement = collision.gameObject.GetComponent<PlayerMovement2>();
            
            //If this is supposed to miss on dashing and the player is dashing then dont deal damage, else do damage
            if(!(missDashing && playerMovement.isDashing)) {
                if (playerHealth != null)
                    playerHealth.ChangeHealth(-damage);

                if (playerMovement != null)
                    playerMovement.Knockback(transform, knockbackForce, stunTime);
            }
            //Delay the attack
            StartCoroutine(TriggerWithDelayCoroutine());
        }
    }

    //Delay the spikes attack cooldown as to not drain the player's health
    private IEnumerator TriggerWithDelayCoroutine() {
        isTriggered = true; // Prevent re-triggering
        anim.SetBool("PlayAnimation",true);
        yield return new WaitForSeconds(attackCooldown); // Delay by attackCooldown seconds
        Debug.Log("Triggered after delay!");
        isTriggered = false; // Allow re-triggering if needed
        anim.SetBool("PlayAnimation",false);
    }

    //Execute the logic to attack the player
    IEnumerator AttackPlayer() {
        isAttacking = true;

        while (true) {
            // Verificar si el jugador aún está dentro del rango de ataque
            if (player == null || Vector2.Distance(transform.position, player.position) > stopDistance) {
                isAttacking = false;
                yield break; // Salir de la corrutina si el jugador está fuera de rango
            }

            yield return new WaitForSeconds(attackCooldown);
        }
    }

    //Apply damage to the player
    public void ApplyAttackDamage() {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position, weaponRange, LayerMask.GetMask("Player"));

        Debug.Log($"🎯 Jugadores detectados: {hits.Length}");

        //For each collider that it collides with
        foreach (Collider2D hit in hits) {
            Player_Health playerHealth = hit.GetComponent<Player_Health>();
            PlayerMovement2 playerMovement = hit.GetComponent<PlayerMovement2>();

            //If player has health, deal damage
            if (playerHealth != null) {
                playerHealth.ChangeHealth(-damage);
                Debug.Log("✅ Daño aplicado al jugador.");
            }

            //If player has movement, deal knockback
            if (playerMovement != null)
                playerMovement.Knockback(transform, knockbackForce, stunTime);
        }
    }

    //
    private void OnDrawGizmosSelected() {
        if (attackPoint != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
        }
    }

    //Update the direction for the attack point
    private void UpdateAttackPointDirection() {
        if (player == null || attackPoint == null) return;

        Vector2 direction = (player.position - transform.position).normalized;

        // Decidir dirección dominante (horizontal o vertical)
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
            // Ataque horizontal (izquierda o derecha)
            attackPoint.localPosition = new Vector3(direction.x > 0 ? 1.5f : -1.5f, 0f, 0f);

            // Volteamos sprite horizontalmente
            transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
        } else {
            // Ataque vertical (arriba o abajo)
            attackPoint.localPosition = new Vector3(0f, direction.y > 0 ? 1.5f : -1.5f, 0f);
        }
        
    }

    //Recover from knockback
    IEnumerator RecoverFromKnockback(float time) {
        yield return new WaitForSeconds(time);
        isKnockedBack = false;
    }


}