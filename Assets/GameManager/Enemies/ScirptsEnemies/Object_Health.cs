using UnityEngine;
using System.Collections;

public class Object_Health : MonoBehaviour
{
   public GameObject healthDropPrefab; // El prefab del steak

    public int currentHealth;

    public string vulnerability;

    private Animator anim;
    private Rigidbody2D rb;

    public AudioManager audiomanager;

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


public void TakeDamage(int amount, Transform attacker, Animator animState)
{
    if(animState.GetBool(vulnerability) || vulnerability == "") {
        currentHealth -= amount;
        Debug.Log("🩸 Enemigo recibió daño. Vida actual: " + currentHealth);

        if (anim != null)
            anim.SetTrigger("Hit");

        StartCoroutine(Blink());

        if (rb != null)
        {
            Vector2 dir = (transform.position - attacker.position).normalized;
            rb.AddForce(dir * 2f, ForceMode2D.Impulse);
        }

        if (currentHealth <= 0)
            Die();
    }
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
        Debug.Log("☠️ Enemigo eliminado.");

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

