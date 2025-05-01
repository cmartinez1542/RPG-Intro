using UnityEngine;
using System.Collections;

public class FireyRain : MonoBehaviour
{
    public float duration = 3f;
    public float damageInterval = 0.5f;

    private void Start()
    {
        StartCoroutine(Lifetime());
    }

    IEnumerator Lifetime()
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            DealDamageToPlayersInZone();
            yield return new WaitForSeconds(damageInterval);
            elapsed += damageInterval;
        }

        Destroy(gameObject);
    }

    void DealDamageToPlayersInZone()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, GetComponent<CircleCollider2D>().radius, LayerMask.GetMask("Player"));

        foreach (Collider2D hit in hits)
        {
        }
    }
}
