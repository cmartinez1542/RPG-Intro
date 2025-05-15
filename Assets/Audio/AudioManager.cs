using UnityEngine;
using System.Collections;
public class AudioManager : MonoBehaviour
{
    [Header("-------- Audio Source --------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource chargeSource; 


    [Header("-------- Audio Clip --------")]
    public AudioClip background;
    public AudioClip attack1;
    public AudioClip attack2;

    public AudioClip WindAttack;
    public AudioClip dash;
    public AudioClip charge;
    public AudioClip death;

    public float minPitch = 0.9f;
    public float maxPitch = 0.9f;

    

    public void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

public void PlayAttackSound()
{
    if (attack1 != null)
    {
        SFXSource.pitch = Random.Range(minPitch, maxPitch);
        SFXSource.dopplerLevel = 0;

        float volume = 0.5f; 
        SFXSource.PlayOneShot(attack1, volume);
    }
}


    public void PlayAttackRock()
    {
        if (attack2 != null )
        {
            SFXSource.pitch = 2f;
        SFXSource.dopplerLevel = 0;
        SFXSource.pitch = Random.Range( minPitch, maxPitch );
        SFXSource.PlayOneShot(attack2);
        }
    }

   public void PlayDash()
    {
        if (dash != null )
        {
            SFXSource.pitch = 2f;
            SFXSource.dopplerLevel = 0;
            SFXSource.pitch = Random.Range( minPitch, maxPitch );
            SFXSource.PlayOneShot(dash);
        }
    }

public void PlayCharge()
{
    if (charge != null)
    {
        chargeSource.Stop(); 
        chargeSource.clip = charge;
        chargeSource.dopplerLevel = 0;
        chargeSource.pitch = Random.Range(minPitch, maxPitch);
        chargeSource.volume = 1f;
        chargeSource.loop = false;
        chargeSource.Play();
    }
}

public void StopCharge()
{
    if (chargeSource.isPlaying)
    {
        StartCoroutine(FastFadeOutAndStop(chargeSource, 0.1f));
    }
}

private IEnumerator FastFadeOutAndStop(AudioSource source, float duration)
{
    float startVolume = source.volume;
    float t = 0f;

    while (t < duration)
    {
        t += Time.deltaTime;
        source.volume = Mathf.Lerp(startVolume, 0f, t / duration);
        yield return null;
    }

    source.volume = 0f;
    source.Stop();
    source.clip = null;
}




public void PlayWindAttackSound()
{
    if (WindAttack != null)
    {
        SFXSource.pitch = Random.Range(minPitch, maxPitch);
        SFXSource.dopplerLevel = 0;
        SFXSource.PlayOneShot(WindAttack);
        StartCoroutine(StopSFXAfterSeconds(3f)); // Detener después de 2 segundos
    }
}
 private IEnumerator StopSFXAfterSeconds(float seconds)
{
    yield return new WaitForSeconds(seconds);
    SFXSource.Stop();
}
}


