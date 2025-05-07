using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    private Transform target;
    private Vector3 offset;
    private bool following = true;

    void Start()
    {
        offset = new Vector3(0, 0, -10);
    }

void LateUpdate()
{
    if (following && target != null)
    {
        Vector3 followPos = target.position + offset;
        followPos.x = Mathf.Round(followPos.x * 100) / 100f;
        followPos.y = Mathf.Round(followPos.y * 100) / 100f;

        transform.position = followPos;
    }
}



    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        following = true;
    }

    public void FocusOn(Vector3 position)
    {
        following = false;
        transform.position = position + offset;
    }

    public void Shake(float magnitude, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeCoroutine(magnitude, duration));
    }

    private IEnumerator ShakeCoroutine(float magnitude, float duration)
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.position = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
    }
}
