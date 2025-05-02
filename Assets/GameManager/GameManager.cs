using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform[] spawnPoints;   // array of spawn points
    public Camera mainCamera; // Assign the temporary camera in Inspector
    public Sprite[] playerOutfits; // Array of outfit sprites
    public Animator [] animator; // Array of animators
    public PlayerMovement movement;
    public RuntimeAnimatorController[] playerAnimatorControllers;



        private void Update()
    {
        // Debug: Check if any keyboard input is detected

    }

    private void Start()
    {
        // Checks if a player has touch a button to join
        PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
        
        //PlayerInputManager.instance.JoinPlayer();
        //PlayerInputManager.instance.JoinPlayer();


        // Debug Arrays
        Debug.Log("Spawn Points Count: " + spawnPoints.Length);
        Debug.Log("Player Outfits Count: " + playerOutfits.Length);

        
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {

        Rigidbody2D rbCheck = playerInput.GetComponent<Rigidbody2D>();

        
        Debug.Log("Player joined: " + playerInput.playerIndex);


        // Assign a spawn point

        int index = (PlayerInputManager.instance.playerCount - 1) % spawnPoints.Length;

        // logic to initialize in player_mov script 
        Sprite outfit = (index < playerOutfits.Length) ? playerOutfits[index] : null;
        
        PlayerMovement movement = playerInput.GetComponent<PlayerMovement>();
        
        Debug.Log("Assigning player to spawn point: " + index);
        playerInput.transform.position = spawnPoints[index].position;

        // Assign different outfits
        SpriteRenderer spriteRenderer = playerInput.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null && playerOutfits.Length > index)
        {
            Debug.Log("Assigning sprite to player " + index + ": " + playerOutfits[index].name);
            spriteRenderer.sprite = playerOutfits[index];
        }
        else
        {
            Debug.LogWarning("No sprite found or index out of range for player " + index);
        }

        // Assign camara to a player
        if (mainCamera != null && PlayerInputManager.instance.playerCount == 1)
        {
            mainCamera.transform.SetParent(playerInput.transform);
            mainCamera.transform.localPosition = new Vector3(0, 0, -10);
        }

        // Assign Animator Controller based on player index
        Animator animator = playerInput.GetComponentInChildren<Animator>(true);
        if (animator != null && playerAnimatorControllers.Length > index)
        {
            animator.runtimeAnimatorController = playerAnimatorControllers[index];
            Debug.Log($"Animator Controller '{playerAnimatorControllers[index].name}' successfully assigned to player {index}.");
        }
        else
        {
            Debug.LogError($"Animator assignment failed. Animator found: {animator != null}, Animator Controllers array length: {playerAnimatorControllers.Length}, Index: {index}");
        }

        if (SceneManager.GetActiveScene().name == "Boss_Level")
        {
            Boss_Movement boss = Object.FindFirstObjectByType<Boss_Movement>();
            if (boss != null)
            {
                boss.m_player = playerInput.transform;
                boss.m_playerRb = playerInput.GetComponent<Rigidbody2D>();
                Debug.Log("Boss assigned player transform" + playerInput.transform.name);
            }
            else
            {
                Debug.LogWarning("Couldn't find the boss");
            }
        }
    }
}