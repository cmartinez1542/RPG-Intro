using UnityEngine;
using System.Collections;

public enum BossState
{
    ArrowPhase,
    SwordPhase
}

public class SemiBoss : MonoBehaviour
{
    [Header("Ataque Melee")]
    public Transform attackPoint;
    public float weaponRange = 1f;
    public int damage = 1;
    public float knockbackForce = 4f;
    public float stunTime = 1f;

    public BossState currentState = BossState.SwordPhase;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Fase Flechas")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float arrowRate = 0.2f;
    public float arrowDuration = 10f;

    [Header("Fase Espada")]
    public float swordDuration = 60f;
    public float moveSpeed = 3f;

    [Header("Detección")]
    public float attackRange = 8f;
    public float stopDistance = 1.5f;

    [Header("Vida")]
    public float maxHealth = 100f;
    private float realHealth;
    private float displayedHealth;

    private float stateTimer;
    private float arrowTimer;
    private bool isVulnerable = false;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;
        else
            Debug.LogWarning("❗No se encontró ningún objeto con el tag 'Player'");

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        realHealth = maxHealth;
        displayedHealth = realHealth;

        StartSwordPhase(); // Empieza directamente en modo espada
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case BossState.ArrowPhase:
                rb.linearVelocity = Vector2.zero;
                anim.SetFloat("Speed", 0f);
                FacePlayer();

                if (Time.time >= stateTimer)
                {
                    StartSwordPhase();
                }
                else if (Time.time >= arrowTimer)
                {
                    ShootArrow();
                    anim.SetTrigger("ShootArrow");
                    arrowTimer = Time.time + arrowRate;
                }
                break;

            case BossState.SwordPhase:
                if (distance > stopDistance)
                {
                    Vector2 direction = (player.position - transform.position).normalized;
                    rb.linearVelocity = direction * moveSpeed;
                    anim.SetFloat("Speed", rb.linearVelocity.magnitude);
                }
                else
                {
                    rb.linearVelocity = Vector2.zero;
                    anim.SetFloat("Speed", 0f);
                    anim.SetTrigger("Attack");
                }

                FacePlayer();

                if (Time.time >= stateTimer)
                    StartArrowPhase();
                break;
        }
    }

    private void FacePlayer()
    {
        if (player == null) return;
        Vector3 scale = transform.localScale;
        scale.x = player.position.x > transform.position.x ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void StartArrowPhase()
    {
        Debug.Log("▶️ Entra en fase de flechas");
        currentState = BossState.ArrowPhase;
        isVulnerable = true;
        stateTimer = Time.time + arrowDuration;
        arrowTimer = Time.time;
        anim.SetTrigger("EnterArrowPhase");
    }

    private void StartSwordPhase()
    {
        Debug.Log("⚔️ Entra en fase de espada");
        currentState = BossState.SwordPhase;
        isVulnerable = false;
        stateTimer = Time.time + swordDuration;
        anim.SetTrigger("EnterSwordPhase");
    }

    private void ShootArrow()
    {
        if (player == null || firePoint == null) return;

        Vector2 dir = (player.position - firePoint.position).normalized;
        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);

        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null)
            arrowScript.SetDirection(dir);
    }

    public void TakeDamage(float amount)
    {
        if (currentState == BossState.ArrowPhase && isVulnerable)
        {
            realHealth -= amount;
            displayedHealth = realHealth;
            Debug.Log("💥 Daño completo");
            StartSwordPhase(); // Lo interrumpe
        }
        else if (currentState == BossState.SwordPhase)
        {
            realHealth -= amount * 0.05f;
            Debug.Log("🛡️ Daño muy reducido");

            if (Random.value < 0.2f || Time.time >= stateTimer - 50f)
                StartArrowPhase();
        }

        if (realHealth <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("☠️ Boss derrotado");
        anim.SetTrigger("Die");
        rb.linearVelocity = Vector2.zero;
        Destroy(gameObject, 2f); // espera que termine la animación
    }

    // Llamado desde el evento de animación de ataque
    public void SemiBossApplySwordAttack()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("⚠️ attackPoint no asignado.");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position, weaponRange, LayerMask.GetMask("Player"));

        Debug.Log($"🎯 Jugadores detectados: {hits.Length}");

        foreach (Collider2D hit in hits)
        {
            Player_Health playerHealth = hit.GetComponent<Player_Health>();
            PlayerMovement2 playerMovement = hit.GetComponent<PlayerMovement2>();

            if (playerHealth != null)
            {
                playerHealth.ChangeHealth(-damage);
                Debug.Log("✅ Daño aplicado al jugador.");
            }

            if (playerMovement != null)
            {
                Vector2 direction = (hit.transform.position - transform.position).normalized;
                playerMovement.Knockback(transform, knockbackForce, stunTime);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
