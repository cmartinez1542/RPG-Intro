using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Health : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;
    public Slider slider;
    public Animator anim;

    void Start()
    {
        currentHealth = maxHealth;
        slider.maxValue = maxHealth;
        slider.value = currentHealth;
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // evita desbordes
        slider.value = currentHealth;

        Debug.Log($"[💔] Vida actual: {currentHealth} / {maxHealth}");

        // 🩸 Reproducir animación de daño si pierde vida
        if (amount < 0 && anim != null)
        {
            StartCoroutine(PlayHurtAnimation());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator PlayHurtAnimation()
    {
        anim.SetBool("Hurt", true);
        yield return new WaitForSeconds(0.4f);
        anim.SetBool("Hurt", false);
    }

    private void Die()
    {
        Debug.Log("☠️ El jugador ha muerto.");

        if (anim != null)
        {
            anim.SetBool("Die", true); 
        }

        // Desactivar colisiones y movimiento
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        MonoBehaviour movementScript = GetComponent<PlayerMovement2>(); // cambia si usas otro script de movimiento
        if (movementScript != null) movementScript.enabled = false;

        // Desactivar el objeto después del tiempo que dura la animación
        StartCoroutine(DisableAfterDelay(1.5f));
    }

    private IEnumerator DisableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 🔴 IMPORTANTE: desactiva el bool antes de apagar el GameObject (opcional si quieres reiniciar luego)
        if (anim != null)
        {
            anim.SetBool("Die", false);
        }

        gameObject.SetActive(false);
    }
}
