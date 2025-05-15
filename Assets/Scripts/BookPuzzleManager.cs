using System.Collections;
using UnityEngine;

public class BookPuzzleManager : MonoBehaviour
{
    public MonoBehaviour cameraControlScript; 
    private MonoBehaviour playerMovementScript;

    public GameObject puzzleUIPanel;
    public GameObject exclamationIcon;
    public GameObject puzzleTriggerObject;
    public Transform enemySpawnPoint;
    public GameObject singleEnemy; 
    public GameObject thunderEffectPrefab; 
    public Camera mainCamera; 
    public float cameraFocusDuration = 2f; 
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
                Debug.Log(" Orden incorrecto");
                return;
            }
        }

        Debug.Log(" Puzzle resuelto");

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
    
    if (playerMovementScript == null)
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerMovementScript = playerObj.GetComponent<PlayerMovement2>(); 
    }

   
    if (playerMovementScript != null) playerMovementScript.enabled = false;
    if (cameraControlScript != null) cameraControlScript.enabled = false;


    originalCameraPos = mainCamera.transform.position;
    Vector3 focusPosition = new Vector3(enemySpawnPoint.position.x, enemySpawnPoint.position.y, originalCameraPos.z);
    yield return StartCoroutine(MoveCameraSmooth(mainCamera.transform, originalCameraPos, focusPosition, 1f));

    yield return new WaitForSeconds(3f);

    
    if (thunderEffectPrefab != null)
        Instantiate(thunderEffectPrefab, enemySpawnPoint.position, Quaternion.identity);

    yield return new WaitForSeconds(1f);

  
    if (singleEnemy != null)
        singleEnemy.SetActive(true);

   
    yield return new WaitForSeconds(cameraFocusDuration);

    
    yield return StartCoroutine(MoveCameraSmooth(mainCamera.transform, focusPosition, originalCameraPos, 1f));

  
    if (playerMovementScript != null) playerMovementScript.enabled = true;
    if (cameraControlScript != null) cameraControlScript.enabled = true;
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
    cam.position = to; 
}

}
