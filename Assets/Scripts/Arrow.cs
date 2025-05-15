using UnityEngine;

public class Arrow : MonoBehaviour
{
   public AudioSource hitSound;
public AudioSource trowSound;
    public float speed = 5f;
    public float lifeTime = 3f;

    private Vector2 direction;
    public int damage = 1;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
          if (trowSound != null)
    {
        trowSound.Play();
    }

    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("Player"))
    {
        Player_Health playerHealth = collision.GetComponent<Player_Health>();

        if (playerHealth != null)
        {
            playerHealth.ChangeHealth(-1); 
            Debug.Log("Flecha hizo daño al jugador.");
        }

        PlayerMovement2 playerMovement = collision.GetComponent<PlayerMovement2>();
        if (playerMovement != null)
        {
            Vector2 knockDir = (collision.transform.position - transform.position).normalized;
            playerMovement.Knockback(transform, 50f, 0.5f); 
        }
    }

    // Reproducir sonido si existe
    if (hitSound != null)
    {
        hitSound.Play();
    }

    // Desactivar el objeto en vez de destruirlo inmediatamente
    GetComponent<SpriteRenderer>().enabled = false;
    GetComponent<Collider2D>().enabled = false;
    speed = 0f;

    // Destruir después de que suene el clip
    Destroy(gameObject, hitSound.clip.length);
}

  

}
