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
