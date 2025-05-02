using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public GameObject healthDropPrefab; // The steak (health) drop prefab

    public int currentHealth;

    private Animator anim;
    private Rigidbody2D rb;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Boss took damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("Boss defeated.");

        if (anim != null)
            anim.SetTrigger("Death"); // Optional death animation

        // Disable collision and movement
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Enemy_Movement movement = GetComponent<Enemy_Movement>();
        if (movement != null) movement.enabled = false;

        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Drop health if prefab is assigned
        if (healthDropPrefab != null)
        {
            Instantiate(healthDropPrefab, transform.position, Quaternion.identity);
        }

        // Destroy the object after 1.5 seconds to allow death animation to play
        Destroy(gameObject, 1.5f);
    }
}