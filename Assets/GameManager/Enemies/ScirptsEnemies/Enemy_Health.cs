using UnityEngine;
using System.Collections;

public class Enemy_Health : MonoBehaviour
{
   public GameObject healthDropPrefab; // El prefab del steak

    public int currentHealth;

    private Animator anim;
    private Rigidbody2D rb;
    public ParticleSystem hitEffect;
    public Color particleColor = Color.white;
    public SpriteFlasher flasher;

    private void Start()
    {
        
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
IEnumerator FlashRed()
{
    SpriteRenderer sr = GetComponent<SpriteRenderer>();
    if (sr != null)
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f); // duración del flash
        sr.color = Color.white;
    }
}


public void PlayHitParticles(int damage, Vector2 hitDirection)
{
    if (hitEffect != null)
    {
        // Offset the position slightly *behind* the enemy, in the opposite direction of the hit
        float offsetDistance = -1.3f; // ← adjust this if needed
        Vector3 spawnOffset = -hitDirection.normalized * offsetDistance;
        hitEffect.transform.position = transform.position + spawnOffset;

        // Optional rotation for visual alignment
        float angle = Mathf.Atan2(hitDirection.y, hitDirection.x) * Mathf.Rad2Deg;
        hitEffect.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Particle velocity in hit direction
        var velocity = hitEffect.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.World;
        velocity.x = new ParticleSystem.MinMaxCurve(hitDirection.normalized.x * 5f);
        velocity.y = new ParticleSystem.MinMaxCurve(hitDirection.normalized.y * 5f);

        // Adjust emission burst based on damage
        var main = hitEffect.main;
        main.startColor = particleColor;

        var emission = hitEffect.emission;
        ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[emission.burstCount];
        emission.GetBursts(bursts);

        if (bursts.Length > 0)
        {
            bursts[0].count = new ParticleSystem.MinMaxCurve(damage * 3);
            emission.SetBursts(bursts);
        }

        hitEffect.Play();
    }
}




public void TakeDamage(int amount, Transform attacker)
{
    currentHealth -= amount;
    Debug.Log(" Enemigo recibió daño. Vida actual: " + currentHealth);

    if (anim != null)
        anim.SetTrigger("Hit");

    StartCoroutine(Blink());
    flasher.Flash(0.3f);

    if (rb != null)
    {
        Vector2 dir = (transform.position - attacker.position).normalized;
        rb.AddForce(dir * 2f, ForceMode2D.Impulse);
    }

    if (currentHealth <= 0)
        Die();
}


IEnumerator Blink()
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





    private void Die()
    {
        Debug.Log(" Enemigo eliminado.");

        if (anim != null)
            anim.SetTrigger("Death"); // Animación de muerte (opcional)

        // Desactiva colisión y movimiento
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Enemy_Movement movement = GetComponent<Enemy_Movement>();
        if (movement != null) movement.enabled = false;

        if (rb != null)
         rb.linearVelocity = Vector2.zero;
         rb.linearVelocity = Vector2.zero;
         rb.bodyType = RigidbodyType2D.Static; 
        if (healthDropPrefab != null)
{
    Instantiate(healthDropPrefab, transform.position, Quaternion.identity);
}


        // Destruye el objeto después de 1.5 segundos (para dejar animación de muerte)
        Destroy(gameObject, 1.5f);
    }
    
}

