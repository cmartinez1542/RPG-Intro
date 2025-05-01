using UnityEngine;

public class BossFireball : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 5f;

    private Vector2 moveDirection;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Get direction to player at the time of spawn
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector2 toPlayer = (player.transform.position - transform.position).normalized;
            moveDirection = toPlayer;
        }
        else
        {
            moveDirection = Vector2.right; // fallback
        }

        // Destroy after lifetime
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignore boss or other fireballs
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject == gameObject) // TODO add "projectile tag so that fireball and firerain don't despawn when hitting each other"
            return;

        // Destroy on any other collision
        Destroy(gameObject);
    }
}
