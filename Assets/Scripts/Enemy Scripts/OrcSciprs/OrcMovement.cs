using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class OrcMovement : MonoBehaviour
{
     public SpriteRenderer spriteRenderer; 

    private bool isKnockedBack = false;
    private bool isAttacking = false;
    private bool isCoolingDown = false;

    [Header("Attack Settings")]
    public float attackCooldown = 1f;
    public float retreatTime = 5f;

    public int damage = 1;
    public Transform attackPoint;
    public float weaponRange = 1f;
    public float knockbackForce = 2f;
    public float stunTime = 1f;

    private Animator anim;
    private Rigidbody2D rb;

    [Header("Follow Player Settings")]
    public Transform player;
    public float detectionRange = 5f;
    public float stopDistance = 1.5f;
    public float moveSpeed = 2f;
    public Vector3 attackPointOffset = new Vector3(1.5f, 0f, 0f);

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (player == null)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length > 0)
                player = GetClosestPlayer(players).transform;
        }
    }

    private void FixedUpdate()
    {
        float speed = rb.linearVelocity.magnitude;
        anim.SetFloat("Speed", speed);

        if (isKnockedBack || isCoolingDown) return;

        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            if (!isAttacking && distance <= stopDistance)
            {
                rb.linearVelocity = Vector2.zero;
                FlipToFacePlayer();
                StartCoroutine(AttackAndRetreat());
            }
            else if (distance <= detectionRange)
            {
                Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
                rb.linearVelocity = direction * moveSpeed;
                FlipToFacePlayer();
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    private GameObject GetClosestPlayer(GameObject[] players)
    {
        GameObject closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject p in players)
        {
            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = p;
            }
        }

        return closest;
    }

    private IEnumerator AttackAndRetreat()
    {
        isAttacking = true;

        if (anim != null)
            anim.SetTrigger("Attack");

        yield return new WaitForSeconds(attackCooldown);

        ApplyAttackDamage();

        isAttacking = false;
        isCoolingDown = true;

        // Alejarse del jugador durante el cooldown
        float timer = 0f;
        while (timer < retreatTime)
        {
            if (player != null)
            {
                Vector2 direction = ((Vector2)transform.position - (Vector2)player.position).normalized;
                rb.linearVelocity = direction * moveSpeed;
            }
            timer += Time.deltaTime;
            yield return null;
        }

        isCoolingDown = false;
    }

private HashSet<GameObject> alreadyDamaged = new HashSet<GameObject>();

public void ApplyAttackDamage()
{
    alreadyDamaged.Clear(); // vaciar antes del ataque

    Collider2D[] hits = Physics2D.OverlapCircleAll(
        attackPoint.position, weaponRange, LayerMask.GetMask("Player"));

    foreach (Collider2D hit in hits)
    {
        if (!alreadyDamaged.Contains(hit.gameObject))
        {
            alreadyDamaged.Add(hit.gameObject);

            Player_Health playerHealth = hit.GetComponent<Player_Health>();
            PlayerMovement2 playerMovement = hit.GetComponent<PlayerMovement2>();

            if (playerHealth != null)
                playerHealth.ChangeHealth(-damage);

            if (playerMovement != null)
                playerMovement.Knockback(transform, knockbackForce, stunTime);
        }
    }
}


    private void FlipToFacePlayer()
    {
        if (player != null)
        {
            bool lookingRight = player.position.x > transform.position.x;
            transform.localScale = new Vector3(lookingRight ? 1 : -1, 1, 1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
        }
    }

public void ApplyKnockback(Vector2 direction, float force, float duration)
{
    if (rb != null)
    {
        isKnockedBack = true;
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        FlashWhiteBlink((stunTime / 2), 0.3f);
        StartCoroutine(RecoverFromKnockback(duration));
    }
}

    private IEnumerator RecoverFromKnockback(float time)
    {
        yield return new WaitForSeconds(time);
        isKnockedBack = false;
    }
    public void FlashWhiteBlink(float totalDuration, float blinkSpeed = 10.5f)
    {
        StartCoroutine(BlinkCoroutine(totalDuration, blinkSpeed));
    }

    private IEnumerator BlinkCoroutine(float totalDuration, float blinkSpeed)
    {
        float elapsed = 0f;
        Color originalColor = spriteRenderer.color;

        while (elapsed < totalDuration)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(blinkSpeed);

            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(blinkSpeed);

            elapsed += blinkSpeed * 2;
        }

        spriteRenderer.color = originalColor; // ensure it resets
    }

}
