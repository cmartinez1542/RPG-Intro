using UnityEngine;

public class BossHealth : MonoBehaviour
{

    public int currentHealth;

    private Rigidbody2D rb;
    private BossAnimationController animController;
    private BossAudioManager bossSound;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animController = GetComponent<BossAnimationController>();
        bossSound = GetComponent<BossAudioManager>();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Boss took damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
            Die();
        else if (animController != null)
            animController.PlayHurt();
    }

    private void Die()
    {
        Debug.Log("Boss defeated.");

        if (animController != null)
            animController.PlayDeath();

        // Disable collision and movement
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Boss_Movement movement = GetComponent<Boss_Movement>();
        if (movement != null) movement.enabled = false;

        if (rb != null) rb.linearVelocity = Vector2.zero;

        bossSound.PlayBossDies();
    }
}