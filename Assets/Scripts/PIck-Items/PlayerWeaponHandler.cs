using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerWeaponHandler : MonoBehaviour
{
    public SpriteRenderer heldWeaponSpriteRenderer; // assign this via inspector

    public void SetHeldWeaponSprite(Sprite newSprite)
    {
        heldWeaponSpriteRenderer.sprite = newSprite;
    }
}

