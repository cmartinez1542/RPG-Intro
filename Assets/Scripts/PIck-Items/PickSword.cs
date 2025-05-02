using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickSword : MonoBehaviour
{
    public string weaponName;
    public Sprite weaponSprite; // assign in inspector
    private bool playerInRange;
    private GameObject player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            player = collision.gameObject;
            Debug.Log($"Player near {weaponName}. Press E to pick up.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            PlayerWeaponHandler weaponHandler = player.GetComponent<PlayerWeaponHandler>();
            if (weaponHandler != null)
            {
                weaponHandler.SetHeldWeaponSprite(weaponSprite);
                Destroy(gameObject); // remove weapon from scene
                Debug.Log($"Picked up {weaponName}");
            }
        }
    }
}

