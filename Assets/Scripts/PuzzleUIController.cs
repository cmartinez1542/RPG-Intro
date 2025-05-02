using UnityEngine;

public class PuzzleUIController : MonoBehaviour
{
    public GameObject panel;

    void Update()
    {
        if (panel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            panel.SetActive(false);
            Time.timeScale = 1f; // Reanudar el juego
        }
    }
}
