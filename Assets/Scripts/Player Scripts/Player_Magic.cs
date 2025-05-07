using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Magic : MonoBehaviour
{
    
    public int currentMagic;
    public int maxMagic;
    public Slider slider;
    public float regenTime = 5f;
    public int regenAmount = 1;  // how much magic is restored per tick

    void Start()
    {
        currentMagic = maxMagic;
        slider.maxValue = maxMagic;
        slider.value = currentMagic;

        StartCoroutine(RegenMagic());
    }


    public void ChangeMagic(int amount)
    {
        currentMagic += amount;
        slider.value = currentMagic;

        Debug.Log($"Vida actual: {currentMagic} / {maxMagic}");

        if (currentMagic <= 0)
        {
            Debug.Log("Magic run out, wait for 10 Sec's");

        }
    }


        private IEnumerator RegenMagic()
    {
        // ONLY IF Currenthealth != maxHealth
        while (true)
        {
            yield return new WaitForSeconds(regenTime);

            if (currentMagic < maxMagic)
            {
                ChangeMagic(regenAmount);
            }
        }
    }

}

/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Health : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;
    public Slider slider;

    void Start()
    {
        currentHealth = maxHealth;
        slider.maxValue = maxHealth;
        slider.value = currentHealth;
    }

public void ChangeHealth(int amount)
{
    currentHealth += amount;
    slider.value = currentHealth;

    Debug.Log($"[💔] Vida actual: {currentHealth} / {maxHealth}");

    if (currentHealth <= 0)
    {
        Debug.Log("☠️ El jugador ha muerto.");
        gameObject.SetActive(false);
    }
}


}


*/