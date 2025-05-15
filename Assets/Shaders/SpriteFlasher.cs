using UnityEngine;
using System.Collections;

using UnityEngine;

public class SpriteFlasher : MonoBehaviour
{
    public Material flashMaterial;
    private Material defaultMaterial;
    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        defaultMaterial = sr.material;
    }

    public void Flash(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FlashCoroutine(duration));
    }

    private IEnumerator FlashCoroutine(float duration)
    {
        sr.material = flashMaterial;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = Mathf.PingPong(elapsed * 10f, 1f); // flash rápido
            sr.material.SetFloat("_FlashAmount", t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        sr.material.SetFloat("_FlashAmount", 0);
        sr.material = defaultMaterial;
    }
}
