using UnityEngine;
using System.Collections;


public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.1f;
    private Vector3 initialPosition;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void OnEnable()
    {
        initialPosition = transform.localPosition;
    }

    public void Shake()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            Vector3 randomPoint = initialPosition + Random.insideUnitSphere * shakeMagnitude;
            randomPoint.z = initialPosition.z;
            transform.localPosition = randomPoint;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = initialPosition;
    }
}
