using UnityEngine;

public class PuzzleTrigger : MonoBehaviour
{
    public GameObject exclamationIcon;
    public GameObject puzzleUIPanel;
    private MonoBehaviour playerMovementScript;
    private bool playerInRange = false;

    void Start()
    {
        exclamationIcon.SetActive(false);
        puzzleUIPanel.SetActive(false);
    }

void Update()
{
    if (playerInRange && Input.GetKeyDown(KeyCode.Space))
    {
        puzzleUIPanel.SetActive(true);

        // Buscar el jugador si aún no está asignado
        if (playerMovementScript == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerMovementScript = playerObj.GetComponent<PlayerMovement2>(); // ← Cambia esto si tiene otro nombre
        }

        // Desactivar movimiento del jugador
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;
    }
}


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            exclamationIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            exclamationIcon.SetActive(false);
        }
    }
}
