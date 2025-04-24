using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player_Combat : MonoBehaviour

{

    private bool canAttack = true;
public float attackCooldown = 0.8f; // Tiempo de espera entre ataques

    public Transform attackPoint;

    public Animator anim;
    public bool attackState;
    public AudioManager audiomanager;
    public float knockbackForce = 3f;
    public float attackRange = 1f;
    public float stunTime = 0.2f;
    public int damage = 1;

    public void Attack()
    {
         if (!canAttack) return; // Bloquear si aún no ha pasado el cooldown

    canAttack = false; // Bloquear ataques nuevos
    StartCoroutine(ResetAttackCooldown()); // Empezar la espera

        knockbackForce = 3f;
        attackRange = 1f;
        anim.SetBool("isAttacking", true);
        attackState = anim.GetBool("isAttacking");
        Debug.Log("Attack Started: isAttacking set to " + attackState);
        
         DealDamage(); // Aplica daño
    // audiomanager.PlayAttackSound();
    }

    public void DealDamage()
    {
        Debug.Log($"[{gameObject.name}] Checking for targets in range {attackRange}");

        // Detect all colliders in range (no layer mask)
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);

        Debug.Log($"[{gameObject.name}] Detected {hits.Length} colliders");

        foreach (var hit in hits)
        {
            string tag = hit.tag;
            int layer = hit.gameObject.layer;
            string layerName = LayerMask.LayerToName(layer);

            // Check if it matches "Enemy" tag OR is on "Enemy" layer
            if (tag != "Enemy" && layerName != "Enemy")
            {
                Debug.Log($"⛔ Skipped {hit.name} (tag: {tag}, layer: {layerName})");
                continue;
            }

            Debug.Log($"🎯 Valid target: {hit.name} (tag: {tag}, layer: {layerName})");

            // Check for either health component
            Enemy_Health enemy = hit.GetComponent<Enemy_Health>();
            BossHealth boss = hit.GetComponent<BossHealth>();

            if (enemy != null)
            {
                Debug.Log("💥 Enemy_Health found");
                enemy.TakeDamage(damage);
            }
            else if (boss != null)
            {
                Debug.Log("👑 BossHealth found");
                boss.TakeDamage(damage);
            }
            else
            {
                Debug.Log($"⚠️ No health script found on {hit.name}");
            }

            // Apply knockback if it has a movement script
            Enemy_Movement enemyMove = hit.GetComponent<Enemy_Movement>();
            if (enemyMove != null)
            {
                Vector2 knockDir = (hit.transform.position - transform.position).normalized;
                enemyMove.ApplyKnockback(knockDir, knockbackForce, stunTime);
                Debug.Log($"🌀 Knockback applied to {hit.name}");
            }
        }
    }

    void OnDrawGizmosSelected()
{
    if (attackPoint == null) return;
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(attackPoint.position, attackRange);
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
        anim.SetBool("Attack2", true);
        attackState = anim.GetBool("Attack2");
        Debug.Log($"{gameObject.name} used Rock Smash!");
        // anim.SetTrigger("RockSmash"); // sólo si tienes una animación separada
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
