using System.Collections;
using UnityEngine;

public class BookPuzzleManager : MonoBehaviour
{
    public GameObject puzzleUIPanel;
    public GameObject exclamationIcon;
    public GameObject puzzleTriggerObject;
    public Transform enemySpawnPoint;
    public GameObject singleEnemy; // ← SOLO UN ENEMIGO
    public GameObject thunderEffectPrefab; // ← Prefab de trueno
    public Camera mainCamera; // ← Cámara principal
    public float cameraFocusDuration = 2f; // Cuánto tiempo la cámara enfoca al enemigo
    public BookVisual[] sceneBooks;

    public static BookPuzzleManager Instance;
    public BookPuzzleSlot[] slots;

    private Vector3 originalCameraPos;

    private void Awake()
    {
        Instance = this;
    }

    public void CheckSolution()
    {
        string[] correctOrder = { "yellow", "blue", "black", "green", "red" };

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].bookID != correctOrder[i])
            {
                Debug.Log("❌ Orden incorrecto");
                return;
            }
        }

        Debug.Log("✅ Puzzle resuelto");

        puzzleUIPanel.SetActive(false);
        Time.timeScale = 1f;
        exclamationIcon.SetActive(false);
        puzzleTriggerObject.SetActive(false);

        for (int i = 0; i < sceneBooks.Length; i++)
        {
            sceneBooks[i].SetBook(slots[i].bookID, slots[i].image.sprite);
        }

        StartCoroutine(EnemySpawnSequence());
    }

private IEnumerator EnemySpawnSequence()
{
    originalCameraPos = mainCamera.transform.position;
    Vector3 focusPosition = new Vector3(enemySpawnPoint.position.x, enemySpawnPoint.position.y, originalCameraPos.z);

    // 🔄 Mover suavemente la cámara al enemigo
    yield return StartCoroutine(MoveCameraSmooth(mainCamera.transform, originalCameraPos, focusPosition, 1f));

    // ⚡ Trueno
    Instantiate(thunderEffectPrefab, enemySpawnPoint.position, Quaternion.identity);
    yield return new WaitForSeconds(1f);

    // 👹 Activar enemigo
    singleEnemy.SetActive(true);

    // Esperar para que el jugador lo vea
    yield return new WaitForSeconds(cameraFocusDuration);

    // 🔄 Volver cámara a su posición original
    yield return StartCoroutine(MoveCameraSmooth(mainCamera.transform, focusPosition, originalCameraPos, 1f));
}
private IEnumerator MoveCameraSmooth(Transform cam, Vector3 from, Vector3 to, float duration)
{
    float elapsed = 0f;
    while (elapsed < duration)
    {
        cam.position = Vector3.Lerp(from, to, elapsed / duration);
        elapsed += Time.deltaTime;
        yield return null;
    }
    cam.position = to; // asegurar posición final exacta
}

}
