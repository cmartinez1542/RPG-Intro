using UnityEngine;

public class BossAudioManager : MonoBehaviour
{
    [SerializeField] AudioSource SFXSource; // declaring all the audio clips/source
    public AudioClip boss_wakes_up;
    public AudioClip boss_shoots_fireball;
    public AudioClip fire_rain_sound;
    public AudioClip boss_spawns_minion;
    public AudioClip boss_dies;


    public void Start()
    {
    }

    public void PlayBossWakesUp() // various functions to play different sounds related to the boss, they're all the same except for different sound and volume
    {
        if (boss_wakes_up != null)
        {
            SFXSource.volume = 1f;
            SFXSource.clip = boss_wakes_up;
            SFXSource.Play();
        }
    }

    public void PlayBossShootsFireball()
    {
        if (boss_shoots_fireball != null)
        {
            SFXSource.volume = 1f;
            SFXSource.clip = boss_shoots_fireball;
            SFXSource.Play();
        }
    }
    public void PlayFireRain()
    {
        if (fire_rain_sound != null)
        {
            SFXSource.volume = 0.5f;
            SFXSource.clip = fire_rain_sound;
            SFXSource.Play();
        }
    }
    public void PlayBossSpawnsMinion()
    {
        if (boss_spawns_minion != null)
        {
            SFXSource.volume = 0.5f;
            SFXSource.clip = boss_spawns_minion;
            SFXSource.Play();
        }
    }
    public void PlayBossDies()
    {
        if (boss_dies != null)
        {
            SFXSource.volume = 0.25f;
            SFXSource.clip = boss_dies;
            SFXSource.Play();
        }
    }

}


// Fire Burst.wav by SilverIllusionist -- https://freesound.org/s/472688/ -- License: Attribution 4.0 (used for fireball)
// Fire - Effects - Textures - Sizzling, crackling, blowing by GregorQuendel -- https://freesound.org/s/421878/ -- License: Attribution 4.0 (used for fire rain)
