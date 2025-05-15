using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player_Combat : MonoBehaviour

{
    //Wind Attack
    public GameObject windAttackPrefab;         
    public Transform windSpawnPoint;            
    public float windAttackCooldown = 2f;
    private bool canUseWindAttack = true;

    private bool canAttack = true;
    public float attackCooldown = 1f; 

    public Transform attackPoint;

    public Animator anim;
    public bool attackState;
    public AudioManager audiomanager;
    public float knockbackForce = 3f;
    public float PknockbackForce;
    public float attackRange = 1f;
    public float stunTime = 0.07f;
    public int damage = 1;
    public int magicCost = 1;
    public bool magicApplied = false;
    public BoxCollider2D swordCollider;
    public LayerMask targetLayer;



public void Attack()
{
    if (!canAttack || anim.GetBool("isAttacking")) return; 

    canAttack = false;
    StartCoroutine(ResetAttackCooldown());

    knockbackForce = 3f;
    PknockbackForce = 5f;
    attackRange = 1f;
    CameraShake.Instance.shakeDuration = 0.1f;
    CameraShake.Instance.shakeMagnitude = 0.1f;

    anim.SetBool("isAttacking", true);
    attackState = true;


}



public void PlaySwordAttackSound()
{
    audiomanager.PlayAttackSound();
}

public void PlayRockSmashSound()
{
    audiomanager.PlayAttackRock();
}


        public void DealDamageSword()
    {
        Debug.Log($"[{gameObject.name}] Checking for targets using sword collider");

        // Get the world position and size of the sword collider
        Vector2 swordPos = swordCollider.bounds.center;
        Vector2 swordSize = swordCollider.bounds.size;

        // Check for players in the sword’s hitbox
        Collider2D[] hits = Physics2D.OverlapBoxAll(swordCollider.bounds.center, swordCollider.bounds.size, targetLayer);

        Debug.Log($"[{gameObject.name}] Detected {hits.Length} targets");

        foreach (var hit in hits)
        {
            if (hit.gameObject != gameObject && hit.CompareTag("Player"))
            {
                Debug.Log($"[{gameObject.name}] Hit player: {hit.name}");

                PlayerMovement2 otherPlayer = hit.GetComponent<PlayerMovement2>();
                if (otherPlayer != null)
                {
                    otherPlayer.Knockback(transform, knockbackForce, stunTime);
                    hit.GetComponent<Player_Health>().ChangeHealth(-damage);
                    Debug.Log($"[{gameObject.name}] Damage applied to {hit.name}");
                }
            }
        }
    }

    // Deal Damage using player rigid body and Attack range (ThunderStrike/RockSmash)
    public void DealDamage()
    {
        Debug.Log($"[{gameObject.name}] Checking for targets in range {attackRange}");

        // Detectar colisiones con enemigos usando LayerMask "Enemy"
      Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, LayerMask.GetMask("Enemy"));

        Debug.Log($"[{gameObject.name}] Detected {hits.Length} colliders");

        foreach (var hit in hits)
        {
            Debug.Log($"[{gameObject.name}] Hit: {hit.name}, Tag: {hit.tag}");

            if (hit.gameObject != gameObject && hit.CompareTag("Player"))
            {
                Debug.Log($"[{gameObject.name}] Target is a valid Player! Applying knockback.");

                PlayerMovement2 otherPlayer = hit.GetComponent<PlayerMovement2>();
                PlayerMovement2 playerMovement = GetComponent<PlayerMovement2>();

                if (otherPlayer != null)
                {
                    otherPlayer.Knockback(transform, knockbackForce,stunTime);

                            // Knockback to self (reverse direction)
                    Vector2 selfDirection = (transform.position - hit.transform.position).normalized;
                    GetComponent<Rigidbody2D>().linearVelocity = selfDirection * PknockbackForce;
                     Vector2 knockDir = (hit.transform.position - transform.position).normalized;
                    playerMovement.Knockback(transform, PknockbackForce, stunTime);
                    hit.GetComponent<Player_Health>().ChangeHealth(-damage);


                    Debug.Log($"[{gameObject.name}] Knockback called on {hit.name}");
                    CameraShake.Instance.Shake();

                }
                else
                {
                    Debug.LogWarning($"[{gameObject.name}] Hit object {hit.name} has no PlayerMovement2!");
                }
            }

                  Object_Health crate = hit.GetComponent<Object_Health>();
                if (crate != null)
                {
                    Debug.Log(" Obj_Health encontrado");
                    crate.TakeDamage(damage, transform, anim);
                }


                Enemy_Health enemy = hit.GetComponent<Enemy_Health>();
                if (enemy != null)
                {
                    Debug.Log(" Enemy_Health encontrado");
                    enemy.TakeDamage(damage, transform);
                    Vector2 hitDirection = (hit.transform.position - transform.position).normalized;
                    enemy.PlayHitParticles(damage, hitDirection); 


                }

                                BossHealth enemyBoss = hit.GetComponent<BossHealth>();
                if (enemyBoss != null)
                {
                    Debug.Log(" Enemy_Health encontrado");
                    enemyBoss.TakeDamage(damage);
                }

                SemiBoss_Health enemySemiBoss = hit.GetComponent<SemiBoss_Health>();
                if (enemySemiBoss != null)
                {
                    Debug.Log(" Enemy_Health encontrado");
                    enemySemiBoss.TakeDamage(damage, transform);
                }

                // Aplicar knockback si tiene Rigidbody2D
                Enemy_Movement enemyMove = hit.GetComponent<Enemy_Movement>();
                if (enemyMove != null && !hit.CompareTag("Boss"))
                {
                PlayerMovement2 playerMovement = GetComponent<PlayerMovement2>();
                Vector2 knockDir = (hit.transform.position - transform.position).normalized;
                enemyMove.ApplyKnockback(knockDir, knockbackForce, stunTime);

                Debug.Log($" Knockback con stun aplicado a {hit.name}");

                Vector2 selfDirection = (transform.position - hit.transform.position).normalized;
                GetComponent<Rigidbody2D>().linearVelocity = selfDirection * PknockbackForce;
                playerMovement.Knockback(transform, PknockbackForce, stunTime);

                CameraShake.Instance.Shake();
                }


              EnemyArcher enemyArrowMove = hit.GetComponent<EnemyArcher>();
            if (enemyArrowMove != null)
            {
                PlayerMovement2 playerMovement = GetComponent<PlayerMovement2>();
                Vector2 knockDir = (hit.transform.position - transform.position).normalized;
                enemyArrowMove.ApplyKnockback(knockDir, knockbackForce, stunTime);
                
                Debug.Log($" Knockback con stun aplicado a {hit.name}");

                // Knockback to self (reverse direction)
                Vector2 selfDirection = (transform.position - hit.transform.position).normalized;
                GetComponent<Rigidbody2D>().linearVelocity = selfDirection * PknockbackForce;
                playerMovement.Knockback(transform, PknockbackForce, stunTime);

                //GetComponent<Rigidbody2D>().linearVelocity = selfDirection * PknockbackForce;

                // Camara Shake
                CameraShake.Instance.Shake();

            }

                          OrcMovement enemyOrcMove = hit.GetComponent<OrcMovement>();
            if (enemyOrcMove != null)
            {
                PlayerMovement2 playerMovement = GetComponent<PlayerMovement2>();
                Vector2 knockDir = (hit.transform.position - transform.position).normalized;
                enemyOrcMove.ApplyKnockback(knockDir, knockbackForce, stunTime);
                
                Debug.Log($" Knockback con stun aplicado a {hit.name}");

                // Knockback to self (reverse direction)
                Vector2 selfDirection = (transform.position - hit.transform.position).normalized;
                GetComponent<Rigidbody2D>().linearVelocity = selfDirection * PknockbackForce;
                playerMovement.Knockback(transform, PknockbackForce, stunTime);

                //GetComponent<Rigidbody2D>().linearVelocity = selfDirection * PknockbackForce;

                // Camara Shake
                CameraShake.Instance.Shake();

            }
            
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
public void FinishAttacking()
{
    anim.SetBool("isAttacking", false);
    attackState = false;
    Debug.Log("✔️ Animación terminada");
}

public void Wind_Attack()
{
    if (!canUseWindAttack) return; // Prevent spamming during cooldown

    knockbackForce = 0f;
    attackRange = 1f;
    CameraShake.Instance.shakeMagnitude = 0.3f;
    CameraShake.Instance.shakeDuration = 0.2f;
    magicCost = 3;
    magicApplied = true;
   
    Player_Magic magic = GetComponent<Player_Magic>();

    if (magic.currentMagic >= magicCost)
    {
        canUseWindAttack = false;
        StartCoroutine(ResetWindAttackCooldown());

        magic.ChangeMagic(-magicCost);
        anim.SetTrigger("wind_attack");
        audiomanager.PlayWindAttackSound();
        if (windAttackPrefab != null)
        {
            // Get directional input from player
            Vector2 inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            // Default to player facing direction if no input
            if (inputDirection == Vector2.zero)
            {
                inputDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            }

            // Calculate offset based on direction
            Vector3 spawnOffset = Vector3.zero;
            float offsetDistance = 1.2f; // tweak this as needed

            if (Mathf.Abs(inputDirection.x) > Mathf.Abs(inputDirection.y))
            {
                // Left or Right
                spawnOffset = new Vector3(offsetDistance * Mathf.Sign(inputDirection.x), 0f, 0f);
            }
            else
            {
                // Up or Down
                spawnOffset = new Vector3(0f, offsetDistance * Mathf.Sign(inputDirection.y), 0f);
            }

            // Final position from player center
            Vector3 spawnPosition = transform.position + spawnOffset;

            // Instantiate wind attack
            GameObject windInstance = Instantiate(windAttackPrefab, spawnPosition, Quaternion.identity);

            // Set direction
            WindAttack windScript = windInstance.GetComponent<WindAttack>();
            if (windScript != null)
            {
                windScript.SetDirection(inputDirection);
            }

            Debug.Log("Wind attack cast in direction: " + inputDirection);
        }
        else
        {
            Debug.LogWarning("Wind prefab not assigned.");
        }
    }
    else
    {
        Debug.Log("Not enough magic to cast Wind Attack.");
    }
}




    private IEnumerator ResetWindAttackCooldown()
{
    yield return new WaitForSeconds(windAttackCooldown);
    canUseWindAttack = true;
    Debug.Log("Wind Attack is ready again.");
}


    
    public void FinishWindAttack()
    {
        anim.SetBool("Wind", false);

        attackState = anim.GetBool("Wind");
        Debug.Log("Wind Attack Ended: Continue_Attacking set to FALSE, Current Value: " + attackState);
    }

public void SmashAttack()
{
    if (!canAttack || anim.GetBool("Attack2")) return;

    knockbackForce = 5f;
    attackRange = 2f;  
    CameraShake.Instance.shakeMagnitude = 0.3f;
    CameraShake.Instance.shakeDuration = 0.2f;
    magicCost = 3;
    magicApplied = true;

    Player_Magic magic = GetComponent<Player_Magic>();

    if (magic.currentMagic >= magicCost)
    {
        canAttack = false;
        StartCoroutine(ResetAttackCooldown());

        anim.SetBool("Attack2", true);
        attackState = true;

        magic.ChangeMagic(-magicCost);

        // Quita esta línea:
        // audiomanager.PlayAttackRock(); <-- Esto va con Animation Event
    }
}


    public void FinishSmashAttack()
    {
        anim.SetBool("Attack2", false);

        attackState = anim.GetBool("Attack2");
        Debug.Log("Second Attack Ended: Continue_Attacking set to FALSE, Current Value: " + attackState);
    }

    public void SecondAttack()
    {
        anim.SetBool("Continue_Attacking", true);
        attackState = anim.GetBool("Continue_Attacking");
        Debug.Log("Second Attack Ended Current Value: " + attackState);
       
    }

    public void FinishSecondAttack()
    {
        anim.SetBool("Continue_Attacking", false);
        attackState = anim.GetBool("Continue_Attacking");
        Debug.Log("Second Attack Ended: Continue_Attacking set to FALSE, Current Value: " + attackState);
    }
    private IEnumerator ResetAttackCooldown()
{
    yield return new WaitForSeconds(attackCooldown);
    canAttack = true; // ✅ Ya puede volver a atacar
    Debug.Log("✅ Ataque desbloqueado");
}

    
}
