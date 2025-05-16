using UnityEngine;
using System.Collections;

public class BossFireRain : MonoBehaviour
{ // basic declaration
    public float duration = 5f;
    public float damageInterval = 1.0f;

    private void Start() // start the lifetime coroutine
    {
        StartCoroutine(Lifetime());
    }

    IEnumerator Lifetime() // lifetime coroutine
    {
        float elapsed = 0f; // while time less than 10s then do damage every second
        while (elapsed < duration)
        {
            yield return new WaitForSeconds(damageInterval);
            DealDamageToPlayersInZone();
            elapsed += damageInterval;
        }

        Destroy(gameObject);
    }

    void DealDamageToPlayersInZone() // damage function, hurt player colisions only
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, GetComponent<CircleCollider2D>().radius, LayerMask.GetMask("Player"));

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.CompareTag("Player"))
            {
                Player_Health playerHealth = hit.GetComponent<Player_Health>();

                if (playerHealth != null)
                {
                    playerHealth.ChangeHealth(-5);
                    Debug.Log("Fire rain hit player");
                }
            }
        }
    }
}
