using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum BossState
{
    ArrowPhase,
    Weakened, 
    SwordPhase
}


public class SemiBoss : MonoBehaviour
{

    private float weakenedDamageTaken = 0f;
    [Header("Ataque Melee")]
    public Transform attackPoint;
    public float weaponRange = 1f;
    public int damage = 1;
    public float knockbackForce = 4f;
    public float stunTime = 1f;
    private bool canAttack = true;

    public BossState currentState = BossState.SwordPhase;

    private Transform player;
    private Rigidbody2D rb;
    public Animator anim;

    [Header("Fase Flechas")]
    public GameObject arrowPrefab;
    public Transform firePoint;
    public float arrowRate = 0.2f;
    public float arrowDuration = 10f;
    private bool isRetreating = false;
    private float retreatEndTime = 0f;
    public float retreatDuration = 1.2f;
    public float retreatSpeed = 10f;
    private bool isPreparingArrowPhase = false;
    private float prepareStartTime = 0f;
    public float prepareDuration = 0.5f; 
    private bool canShoot = false;
     public AudioSource SemiBossAttackSound;

private Vector2 retreatTargetPosition;

   

    [Header("Fase Espada")]
    public float swordDuration = 60f;
    public float moveSpeed = 3f;

    [Header("Detección")]
    public float attackRange = 8f;
    public float stopDistance = 1.5f;

    [Header("Vida")]
    public float maxHealth = 50f;
    private float realHealth;
    private float displayedHealth;

    public float stateTimer;
    private float arrowTimer;
    public bool isVulnerable = false;

    private int previousHealth;
    private float arrowPhaseDamageRemaining = 10f;
    private int arrowHits = 0;
    private bool canBeFinished = false;  // Solo puede recibir 10 de daño una vez

     public void SemiBossHitSound()
      {
         SemiBossAttackSound.Play();
      }

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
    previousHealth = Mathf.RoundToInt(maxHealth);

    // ❄️ Detener comportamiento por unos segundos
    StartCoroutine(FreezeBossAtStart(2f)); // ← Cambia el tiempo si quieres más o menos
}
private IEnumerator FreezeBossAtStart(float freezeTime)
{
    this.enabled = false; // 🧊 Congela todo el comportamiento (incluye Update)

    yield return new WaitForSeconds(freezeTime);

    this.enabled = true;  // ✅ Lo reactiva
    StartSwordPhase();    // Empieza normalmente
}

private IEnumerator DelayStartSwordPhase()
{
    canAttack = false; 
    yield return new WaitForSeconds(1f); 
    StartSwordPhase();
}

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
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

if (canAttack)
{
    anim.SetTrigger("Attack");
}

                }

                FacePlayer();

                if (Time.time >= stateTimer)
                    StartArrowPhase();
                break;
case BossState.ArrowPhase:

    FacePlayer();

    if (isPreparingArrowPhase)
    {
        rb.linearVelocity = Vector2.zero;
        anim.SetFloat("Speed", 0f);

        if (Time.time >= prepareStartTime + prepareDuration && !isRetreating)

        {
            isPreparingArrowPhase = false;

            // Calcula hacia dónde debe huir
            Vector2 awayDir = (transform.position - player.position).normalized;
            retreatTargetPosition = (Vector2)transform.position + awayDir * 30f; // alejarse 8 unidades
            ActuallyStartArrowPhase();
        }
    }
    else if (isRetreating)
    {
        Vector2 dir = (retreatTargetPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * retreatSpeed;
        anim.SetFloat("Speed", rb.linearVelocity.magnitude);

        // Si ya llegó suficientemente cerca del destino
        if (Vector2.Distance(transform.position, retreatTargetPosition) <= 0.2f || Time.time >= retreatEndTime)
        {
            isRetreating = false;
            rb.linearVelocity = Vector2.zero;
            anim.SetFloat("Speed", 0f);
        }
    }
    else
    {
        rb.linearVelocity = Vector2.zero;
        anim.SetFloat("Speed", 0f);

        if (Time.time >= stateTimer)
        {
            StartSwordPhase();
        }
else if (canShoot && Time.time >= arrowTimer)
{
    ShootArrow();
    anim.SetTrigger("ShootArrow");
    arrowTimer = Time.time + arrowRate;
}

    }

    break;


                case BossState.Weakened:
                rb.linearVelocity = Vector2.zero;
                anim.SetFloat("Speed", 0f);
                FacePlayer();

               if (Time.time >= stateTimer)
                {
                Debug.Log(" Demasiado tiempo en estado DEBILITADO, regresa a ESPADA");
                StartSwordPhase();
                }
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
    Debug.Log(" Se prepara para modo flechas");
    isPreparingArrowPhase = true;
    prepareStartTime = Time.time;
    rb.linearVelocity = Vector2.zero;
    anim.SetFloat("Speed", 0f);
    currentState = BossState.ArrowPhase;
    isVulnerable = false; 

}
private IEnumerator EnableShootingAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);
    canShoot = true;
    arrowTimer = Time.time; // ← permite disparar justo después del delay
}

private void ActuallyStartArrowPhase()
{
    Debug.Log(" Inicia modo flechas oficialmente");

    currentState = BossState.ArrowPhase;
    isVulnerable = true;
    isRetreating = true;
    retreatEndTime = Time.time + retreatDuration;

    stateTimer = Time.time + arrowDuration;

    anim.SetTrigger("EnterArrowPhase");

  
    canShoot = false; 
    StartCoroutine(EnableShootingAfterDelay(1f)); 
}


private IEnumerator EnableAttackAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);
    canAttack = true;
}

 public void StartSwordPhase()
{
    Debug.Log(" Entra en fase de espada");
    currentState = BossState.SwordPhase;
    isVulnerable = false;
    stateTimer = Time.time + swordDuration;

    anim.SetTrigger("EnterSwordPhase");

    canAttack = false;
    StartCoroutine(EnableAttackAfterDelay(1.5f)); // Espera 1.5 segundos antes de atacar
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

public void RegisterDamage(float amount)
{
    if (currentState == BossState.ArrowPhase && isVulnerable)
    {
        arrowHits++;
        Debug.Log($"🧮 Flechazos recibidos: {arrowHits}");

        if (arrowHits >= 2)
        {
            EnterWeakenedState();
        }
    }
    else if (currentState == BossState.Weakened && !canBeFinished)
    {
        weakenedDamageTaken += amount;

        Debug.Log($"🔥 Daño acumulado en Weakened: {weakenedDamageTaken}/10");

        if (weakenedDamageTaken >= 10f)
        {
            // Aplicamos daño real acumulado (una sola vez)
            realHealth -= weakenedDamageTaken;
            displayedHealth = realHealth;

            Debug.Log("💥 Se aplicó el daño total en estado debilitado y cambia a fase espada");

            canBeFinished = true;
            StartSwordPhase();
        }
    }

    if (realHealth <= 0)
        Die();
}




public void EnterWeakenedState()
{
    Debug.Log(" Entró en estado DEBILITADO");
    currentState = BossState.Weakened;
    isVulnerable = true;
    canBeFinished = false;
    weakenedDamageTaken = 0f; // ← importante
    rb.linearVelocity = Vector2.zero;
    anim.SetTrigger("EnterWeakened");

    stateTimer = Time.time + 10f; // 10 segundos de ventana
}



    public void TakeDamage(float amount)
    {
        if (currentState == BossState.ArrowPhase && isVulnerable)
        {
            realHealth -= amount;
            displayedHealth = realHealth;
            Debug.Log("💥 Daño completo (ArrowPhase)");
            StartSwordPhase();
        }
        else if (currentState == BossState.SwordPhase)
        {
            realHealth -= amount * 0.05f;
            Debug.Log("🛡️ Daño muy reducido (SwordPhase)");

            if (Random.value < 0.2f || Time.time >= stateTimer - 50f)
                StartArrowPhase();
        }

        if (realHealth <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("☠️ Boss derrotado");
        anim.SetTrigger("Death");
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

    // Por si quieres usarlo desde SemiBoss_Health
    public void CheckPhaseSwitchByHealth(int currentHealth)
    {
        if (currentState == BossState.ArrowPhase && (previousHealth - currentHealth) >= 10)
        {
            Debug.Log("🔁 Cambio de fase a espada por daño recibido");
            StartSwordPhase();
        }

        previousHealth = currentHealth;
    }
}
