using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 3; // Cuánta vida regenera
    public AudioClip pickupSound; // (Opcional) Sonido de recogida

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica si el objeto que entró tiene el script Player_Health
        Player_Health playerHealth = other.GetComponent<Player_Health>();

        if (playerHealth != null)
        {
            playerHealth.ChangeHealth(healAmount); // Suma vida
            Debug.Log($"🍖 Jugador curado por {healAmount} puntos");

            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            Destroy(gameObject); // Elimina el objeto de comida después de curar
        }
    }
}

