using UnityEngine;
using System.Collections;

public class BossFireRain : MonoBehaviour
{
    public float duration = 5f;
    public float damageInterval = 1.0f;

    private void Start()
    {
        StartCoroutine(Lifetime());
    }

    IEnumerator Lifetime()
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            yield return new WaitForSeconds(damageInterval);
            DealDamageToPlayersInZone();
            elapsed += damageInterval;
        }

        Destroy(gameObject);
    }

    void DealDamageToPlayersInZone()
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
