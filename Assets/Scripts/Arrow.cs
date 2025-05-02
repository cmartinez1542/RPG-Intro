using UnityEngine;

public class Arrow : MonoBehaviour
{
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
            playerHealth.ChangeHealth(-1); // o usa -damage si tienes variable pública
            Debug.Log("✅ Flecha hizo daño al jugador.");
        }

        // Opcional: si tienes un PlayerMovement2 y quieres aplicar knockback:
        PlayerMovement2 playerMovement = collision.GetComponent<PlayerMovement2>();
        if (playerMovement != null)
        {
            Vector2 knockDir = (collision.transform.position - transform.position).normalized;
            playerMovement.Knockback(transform, 2f, 0.5f); // usa los mismos valores que tu melee
        }
    }

    Destroy(gameObject);
}


}
