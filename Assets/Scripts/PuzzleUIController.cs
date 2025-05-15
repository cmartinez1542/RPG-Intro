using UnityEngine;

public class PuzzleUIController : MonoBehaviour
{
    public GameObject panel;
    private MonoBehaviour playerMovementScript;

    void Start()
    {
        // Obtener el script del jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovementScript = player.GetComponent<PlayerMovement2>(); // Cambia el nombre si usas otro
        }
    }

    void Update()
    {
        if (panel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            panel.SetActive(false);
            Time.timeScale = 1f; // Reanudar el juego

            if (playerMovementScript != null)
                playerMovementScript.enabled = true;
        }
    }
}
