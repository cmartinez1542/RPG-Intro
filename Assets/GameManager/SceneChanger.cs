using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneToLoad = "NombreDeTuEscena";

    private void OnTriggerEnter2D(Collider2D other)
    {
  
if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;

        {
            Debug.Log("🔁 Cambiando a escena: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
