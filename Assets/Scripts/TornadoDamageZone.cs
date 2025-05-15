using System.Collections.Generic;
using UnityEngine;

public class TornadoDamageZone : MonoBehaviour
{
    public int damagePerTick = 1;
    public float tickInterval = 0.5f;

    private Dictionary<Collider2D, float> tickTimers = new Dictionary<Collider2D, float>();

void Update()
{
    List<Collider2D> currentEnemies = new List<Collider2D>(tickTimers.Keys);

    foreach (var enemy in currentEnemies)
    {
        if (enemy == null)
        {
            tickTimers.Remove(enemy);
            continue;
        }

        tickTimers[enemy] += Time.deltaTime;

        if (tickTimers[enemy] >= tickInterval)
        {
            ApplyDamage(enemy);
            tickTimers[enemy] = 0f;
        }
    }
}


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") && !tickTimers.ContainsKey(collision.collider))
        {
            tickTimers[collision.collider] = 0f;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy") && !tickTimers.ContainsKey(collision.collider))
        {
            tickTimers[collision.collider] = 0f;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (tickTimers.ContainsKey(collision.collider))
        {
            tickTimers.Remove(collision.collider);
        }
    }

    void ApplyDamage(Collider2D enemyCollider)
    {
        Enemy_Health enemy = enemyCollider.GetComponent<Enemy_Health>();
        if (enemy != null)
        {
            enemy.TakeDamage(damagePerTick, transform);
            Debug.Log($"Tornado hit {enemyCollider.name} for {damagePerTick} damage.");
        }

        SemiBoss_Health semiBoss = enemyCollider.GetComponent<SemiBoss_Health>();
        if (semiBoss != null)
        {
            semiBoss.TakeDamage(damagePerTick, transform);
            Debug.Log($"Tornado hit SemiBoss {enemyCollider.name} for {damagePerTick} damage.");
        }
    }
}
