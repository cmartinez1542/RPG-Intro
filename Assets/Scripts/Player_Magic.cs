using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Player_Magic : MonoBehaviour
{
    public int currentMagic;
    public int maxMagic;
    public Slider slider;
    public AudioManager audiomanager; // audiomanager.PlayDash();

    [Header("Passive Regeneration")]
    public float regenTime = 5f;
    public int regenAmount = 1;

    [Header("Charging")]
    public float chargingRate = 3f; // Magic per second
    private float magicBuffer = 0f;

    private bool isCharging = false;

    [Header("Charging VFX")]
    public GameObject ChargingMagicPrefab; // Drag your charging effect prefab here

    private GameObject chargingEffectInstance;
    private Animator chargingAnim;

    void Start()
    {
        currentMagic = maxMagic;
        slider.maxValue = maxMagic;
        slider.value = currentMagic;

        StartCoroutine(RegenMagic());
    }

    void Update()
    {
        slider.value = currentMagic; // UI sync fallback
    }

    public void StartCharging()
    {
        if (!isCharging)
        {
            isCharging = true;
            audiomanager.PlayCharge();
         
           

            // Instantiate the charging effect if not already present
            if (ChargingMagicPrefab != null && chargingEffectInstance == null)
            {
                chargingEffectInstance = Instantiate(
                    ChargingMagicPrefab,
                    transform.position + new Vector3(0f, 0f, 0f),
                    Quaternion.identity,
                    transform
                    
                );
                 
                chargingAnim = chargingEffectInstance.GetComponent<Animator>();
                 
            }

            // Trigger charging animation on the prefab
            if (chargingAnim != null)
            {
                chargingAnim.SetBool("isCharging", true);
                chargingAnim.SetTrigger("Charging");
                
            }

            StartCoroutine(ChargeMagic());
             audiomanager.PlayCharge();
        }
    }

    public void StopCharging()
    {
        isCharging = false;
        
        if (chargingAnim != null)
        {
            chargingAnim.SetBool("isCharging", false);
        }

        // Optionally destroy the effect when charging ends
        if (chargingEffectInstance != null)
        {
            Destroy(chargingEffectInstance);
            chargingEffectInstance = null;
            chargingAnim = null;
        }
    }

private IEnumerator ChargeMagic()
{
    while (isCharging && currentMagic < maxMagic)
    {
        magicBuffer += chargingRate * Time.deltaTime;

        if (magicBuffer >= 1f)
        {
            int amountToAdd = Mathf.FloorToInt(magicBuffer);
            ChangeMagic(amountToAdd);
            magicBuffer -= amountToAdd;
           
        }

        yield return null;
    }

    StopCharging();
    audiomanager.StopCharge();
}


    public void ChangeMagic(int amount)
    {
        currentMagic = Mathf.Clamp(currentMagic + amount, 0, maxMagic);
        slider.value = currentMagic;

        Debug.Log($"Magic: {currentMagic} / {maxMagic}");

        if (currentMagic <= 0)
        {
            Debug.Log("Magic ran out! Wait for regeneration.");
        }
    }

    private IEnumerator RegenMagic()
    {
        while (true)
        {
            yield return new WaitForSeconds(regenTime);

            if (!isCharging && currentMagic < maxMagic)
            {
                ChangeMagic(regenAmount);
            }
        }
    }
}