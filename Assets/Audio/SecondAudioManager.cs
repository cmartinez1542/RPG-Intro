using UnityEngine;
using System.Collections;
public class SecondAudioManager : MonoBehaviour
{
    [Header("-------- Audio Source --------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource chargeSource;


    [Header("-------- Audio Clip --------")]

    public AudioClip spikes;
    public AudioClip Break;

    public AudioClip ButtonPressed;
    public AudioClip FailedButtons;
    public AudioClip openGates;

    public float minPitch = 0.9f;
    public float maxPitch = 0.9f;



    public void SpikesSound()
    {
        if (spikes != null)
        {
            SFXSource.pitch = Random.Range(minPitch, maxPitch);
            SFXSource.dopplerLevel = 0;

            float volume = 0.5f;
            SFXSource.PlayOneShot(spikes, volume);
        }
    }


    public void BreakSounds()
    {
        if (Break != null)
        {
            SFXSource.pitch = 2f;
            SFXSource.dopplerLevel = 0;
            SFXSource.pitch = Random.Range(minPitch, maxPitch);
            SFXSource.PlayOneShot(Break);
        }
    }

    public void PressButtonSound()
    {
        if (ButtonPressed != null)
        {
            SFXSource.pitch = 2f;
            SFXSource.dopplerLevel = 0;
            SFXSource.pitch = Random.Range(minPitch, maxPitch);
            SFXSource.PlayOneShot(ButtonPressed);
        }
    }

    public void FailedButtonSound()
    {
        if (FailedButtons != null)
        {
            SFXSource.pitch = 2f;
            SFXSource.dopplerLevel = 0;
            SFXSource.pitch = Random.Range(minPitch, maxPitch);
            SFXSource.PlayOneShot(FailedButtons);
        }
    }


    public void OpenGates()
    {
        if (openGates != null)
        {
            SFXSource.pitch = 2f;
            SFXSource.dopplerLevel = 0;
            SFXSource.pitch = Random.Range(minPitch, maxPitch);
            SFXSource.PlayOneShot(openGates);
        }
    }
}
