using System.Collections.Generic;
using UnityEngine;

public class WindAttack : MonoBehaviour
{
    
    public float lifeTime = 1.5f;
    public float attractionForce = 5f;
    public float tickInterval = 0.5f;
    public int damagePerTick = 1;

    private Dictionary<Collider2D, float> tickTimers = new Dictionary<Collider2D, float>();
    private Animator animator;

    private Vector2 direction = Vector2.zero;
    public float speed = 10f;

public void SetDirection(Vector2 dir)
{
    direction = dir.normalized;

}




    void Start()
    {
        Destroy(gameObject, lifeTime);

        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("wind_attack");
        }
    }

    void Update()
    {
           transform.Translate(direction * speed * Time.deltaTime, Space.World);
        
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log($"Collision with: {collision.name}, tag: {collision.tag}");

       if (collision.gameObject.layer == LayerMask.NameToLayer("SemiBoss"))
         return;
        if (collision.CompareTag("Enemy"))
        {
            Vector2 pullDirection = (transform.position - collision.transform.position).normalized;
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(pullDirection * attractionForce * Time.deltaTime, ForceMode2D.Force);
            }

            if (!tickTimers.ContainsKey(collision))
                tickTimers[collision] = 0f;

            tickTimers[collision] += Time.deltaTime;

            if (tickTimers[collision] >= tickInterval)
            {
                /*
                Enemy_Health enemyHealth = collision.GetComponent<Enemy_Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.ChangeHealth(-damagePerTick);
                    Debug.Log($" {collision.name} took tick damage from tornado.");
                }

                tickTimers[collision] = 0f;
                */


                // second method 
                                                // Aplicar daño al enemigo
                Enemy_Health enemy = collision.GetComponent<Enemy_Health>();
                if (enemy != null)
                {
                    Debug.Log(" Enemy_Health encontrado");
                    enemy.TakeDamage(damagePerTick, transform);
                }

                tickTimers[collision] = 0f;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (tickTimers.ContainsKey(collision))
            tickTimers.Remove(collision);
    }
}
