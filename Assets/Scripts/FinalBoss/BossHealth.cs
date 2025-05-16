using UnityEngine;
using System.Collections;
public class BossHealth : MonoBehaviour
{
    // basic declaration
    public int currentHealth;

    private Rigidbody2D rb;
    private BossAnimationController animController;
    private BossAudioManager bossSound;

    private void Start()
    { // get some components
        rb = GetComponent<Rigidbody2D>();
        animController = GetComponent<BossAnimationController>();
        bossSound = GetComponent<BossAudioManager>();
    }

    public void TakeDamage(int amount)
    { // boss got hurt, 
        currentHealth -= amount;
        Debug.Log("Boss took damage. Current health: " + currentHealth);
        StartCoroutine(Blink());
        if (currentHealth <= 0) // kill boss if low health
            Die();
        else if (animController != null)
            animController.PlayHurt();
    }

    private IEnumerator Blink() // idk what this is or who added this ngl... don't think it works?
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        for (int i = 0; i < 3; i++)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(0.05f);
            sr.enabled = true;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void Die() // kill boss function
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