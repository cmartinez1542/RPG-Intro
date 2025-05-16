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

    private void Start() {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    //Flash the object red for 0.1 seconds
    IEnumerator FlashRed() {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) {
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f); // duración del flash
            sr.color = Color.white;
        }
    }

    //Take damage from source, passing the source's amount of damage to take, the attacker's location, and the attacker's animation state
    public void TakeDamage(int amount, Transform attacker, Animator animState) {
        //If the animation state matches the vulnerability or there is no vulnerability
        if(animState.GetBool(vulnerability) || vulnerability == "") {
            //Reduce health by amount
            currentHealth -= amount;
            Debug.Log("🩸 Enemigo recibió daño. Vida actual: " + currentHealth);

            //Display hit animation
            if (anim != null)
                anim.SetTrigger("Hit");

            //Blink to show damage was taken
            StartCoroutine(Blink());

            //If this has a Rigidbody2D, add force to this object to move it
            if (rb != null) {
                Vector2 dir = (transform.position - attacker.position).normalized;
                rb.AddForce(dir * 2f, ForceMode2D.Impulse);
            }

            //If health drops to or below zero, then Die()
            if (currentHealth <= 0)
                Die();
        }
    }

    //Flashes the sprite on and off
    IEnumerator Blink() {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) yield break; //If there is no sprite render, exit

        //Flash 3 times
        for (int i = 0; i < 3; i++) {
            sr.enabled = false;
            yield return new WaitForSeconds(0.05f);
            sr.enabled = true;
            yield return new WaitForSeconds(0.05f);
        }
    }

    //Kill and destroy this object
    private void Die() {
        Debug.Log("☠️ Enemigo eliminado.");

        if (anim != null)
            anim.SetTrigger("Death"); // Animación de muerte (opcional)

        // Desactiva colisión y movimiento
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        Enemy_Movement movement = GetComponent<Enemy_Movement>();
        if (movement != null) movement.enabled = false;

        //If object has rigidBody2D, stop all of its movement
        if (rb != null) {
            rb.linearVelocity = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        //If it has a healthDrop Prefab, create a health drop
        if (healthDropPrefab != null) {
            Instantiate(healthDropPrefab, transform.position, Quaternion.identity);
        }

        // Destruye el objeto después de 1.5 segundos (para dejar animación de muerte)
        Destroy(gameObject, 1.5f);
    }
    
}