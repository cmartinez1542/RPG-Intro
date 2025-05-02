using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player_Combat : MonoBehaviour

{

    private bool canAttack = true;
    public float attackCooldown = 0.0f; // Tiempo de espera entre ataques

    public Transform attackPoint;

    public Animator anim;
    public bool attackState;
    public AudioManager audiomanager;
    public float knockbackForce = 3f;
    public float PknockbackForce;
    public float attackRange = 1f;
    public float stunTime = 0.07f;
    public int damage = 1;
    public BoxCollider2D swordCollider;
    public LayerMask targetLayer;

    public void Attack()
    {
         if (!canAttack) return; // Bloquear si aún no ha pasado el cooldown

    canAttack = false; // Bloquear ataques nuevos
    StartCoroutine(ResetAttackCooldown()); // Empezar la espera

        knockbackForce = 3f;
        PknockbackForce = 5f;
        attackRange = 1f;
        CameraShake.Instance.shakeDuration = 0.1f;
        CameraShake.Instance.shakeMagnitude = 0.1f; // <- Stronger shake
        anim.SetBool("isAttacking", true);
        attackState = anim.GetBool("isAttacking");


        


        Debug.Log("Attack Started: isAttacking set to " + attackState);
        
        

    // audiomanager.PlayAttackSound();
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

                                // Aplicar daño al enemigo
                Enemy_Health enemy = hit.GetComponent<Enemy_Health>();
                if (enemy != null)
                {
                    Debug.Log(" Enemy_Health encontrado");
                    enemy.TakeDamage(damage, transform);
                }

                SemiBoss_Health enemySemiBoss = hit.GetComponent<SemiBoss_Health>();
                if (enemySemiBoss != null)
                {
                    Debug.Log(" Enemy_Health encontrado");
                    enemySemiBoss.TakeDamage(damage, transform);
                }

                // Aplicar knockback si tiene Rigidbody2D
            Enemy_Movement enemyMove = hit.GetComponent<Enemy_Movement>();
            if (enemyMove != null)
            {
                PlayerMovement2 playerMovement = GetComponent<PlayerMovement2>();
                Vector2 knockDir = (hit.transform.position - transform.position).normalized;
                enemyMove.ApplyKnockback(knockDir, knockbackForce, stunTime);

                Debug.Log($" Knockback con stun aplicado a {hit.name}");

                // Knockback to self (reverse direction)
                Vector2 selfDirection = (transform.position - hit.transform.position).normalized;
                GetComponent<Rigidbody2D>().linearVelocity = selfDirection * PknockbackForce;
                playerMovement.Knockback(transform, PknockbackForce, stunTime);

                //GetComponent<Rigidbody2D>().linearVelocity = selfDirection * PknockbackForce;

                // Camara Shake
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
        attackState = anim.GetBool("isAttacking");
        Debug.Log("First attack done, STATE: " + attackState);
    }

    public void SmashAttack()
{    
    knockbackForce = 5f;
    attackRange = 2f;  
    CameraShake.Instance.shakeMagnitude = 0.3f; // <- Stronger shake
    CameraShake.Instance.shakeDuration = 0.2f;
    anim.SetBool("Attack2", true);
    attackState = anim.GetBool("Attack2");
    Debug.Log($"{gameObject.name} used Rock Smash!");

    // anim.SetTrigger("RockSmash"); only if you use a different animation trigger

    // Call your magic/aoe/damage logic here (like area hitbox, spawn rock effect, etc.)
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
        audiomanager.PlayAttackSound2();
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
