using UnityEngine;

public class PuzzleTrigger : MonoBehaviour
{
    public GameObject exclamationIcon;
    public GameObject puzzleUIPanel;

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
            Time.timeScale = 0f; // Pausar el juego
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
