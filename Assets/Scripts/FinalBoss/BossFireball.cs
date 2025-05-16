using UnityEngine;

public class BossFireball : MonoBehaviour
{
    public float speed = 5f; // basic declarations
    public float lifetime = 5f;

    private Vector2 moveDirection;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // get rigid body of the fireball

        // Get direction to player at the time of spawn
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) // start fireball moving towards player
        {
            Vector2 toPlayer = (player.transform.position - transform.position).normalized;
            moveDirection = toPlayer;
        }
        else // if player doesn't exist
        {
            moveDirection = Vector2.right; // fallback
        }

        // Destroy after lifetime
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate() // update position
    {
        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignore boss or other fireballs
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Weapon"))
            return;
        if (collision.gameObject.CompareTag("Player"))
        {
            Player_Health playerHealth = collision.GetComponent<Player_Health>(); // get player health

            if (playerHealth != null) // if the player health exists then do damage
            {
                playerHealth.ChangeHealth(-5);
                Debug.Log("Fireball hit player");
            }

            PlayerMovement2 playerMovement = collision.GetComponent<PlayerMovement2>(); // get player movement
            if (playerMovement != null) // if the player movement exists then do knockback
            {
                playerMovement.Knockback(transform, 5f, 0.5f);
            }
        }
        // Destroy on any collision
        Destroy(gameObject);
    }
}
