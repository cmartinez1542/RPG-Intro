using UnityEngine;
using System.Collections;

public class SemiBoss_Health : MonoBehaviour
{
    public GameObject healthDropPrefab; // El prefab del steak
    public Transform puzzleDoor;
    public int currentHealth;
    private Animator anim;
    private Rigidbody2D rb;
    private SemiBoss semiBoss; // Referencia al script SemiBoss

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        semiBoss = GetComponent<SemiBoss>();

        currentHealth = 100; // o usar Mathf.RoundToInt(semiBoss.maxHealth);
    }

    public void TakeDamage(int amount, Transform attacker)
    {
        // Si está en modo espada, reducir el daño a un 5%
        if (semiBoss != null && semiBoss.currentState == BossState.SwordPhase)
        {
            amount = Mathf.Max(1, Mathf.RoundToInt(amount * 0.05f));
        }

        // Aplicar daño
        currentHealth -= amount;
        Debug.Log("Enemigo recibió daño. Vida actual: " + currentHealth);

        // Notificar al semiBoss cuánta vida perdió (para manejo interno)
        if (semiBoss != null)
        {
            semiBoss.RegisterDamage(amount);
        }

        // Reacción visual
        if (anim != null)
            anim.SetTrigger("Hit");

        StartCoroutine(Blink());

        // Knockback
        if (rb != null && attacker != null)
        {
            Vector2 dir = (transform.position - attacker.position).normalized;
            rb.AddForce(dir * 2f, ForceMode2D.Impulse);
        }

        // Verificar muerte
        if (currentHealth <= 0)
            Die();
    }

    private IEnumerator Blink()
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
        anim.SetTrigger("Death");

    // Desactivar colisión y movimiento
    Collider2D col = GetComponent<Collider2D>();
    if (col != null) col.enabled = false;

    Enemy_Movement movement = GetComponent<Enemy_Movement>();
    if (movement != null) movement.enabled = false;

    if (rb != null)
    {
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
    }

    if (healthDropPrefab != null)
    {
        Instantiate(healthDropPrefab, transform.position, Quaternion.identity);
    }

    // 🧱 Mover la puerta hacia arriba si está asignada
    if (puzzleDoor != null)
        StartCoroutine(OpenPuzzleDoor(puzzleDoor, new Vector3(0f, 8f, 0f), 2f));

    // Destruir después de 1.5 segundos
    Destroy(gameObject, 1.5f);
}
private IEnumerator OpenPuzzleDoor(Transform obj, Vector3 offset, float duration)
{
    Vector3 start = obj.position;
    Vector3 end = start + offset;
    float elapsed = 0f;

    while (elapsed < duration)
    {
        obj.position = Vector3.Lerp(start, end, elapsed / duration);
        elapsed += Time.deltaTime;
        yield return null;
    }

    obj.position = end; // asegurar que termine exacto
}

}


