using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public enum Direction { None, Up, Down, Left, Right }
    public Direction lastDirection = Direction.None;
    public string lastDoorID = "";

    private float roomSwitchCooldown = 0.5f;
    private float lastSwitchTime = -10f;

    public static RoomManager Instance;
    public GameObject currentRoom;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetCurrentRoom(GameObject newRoom)
    {
        if (Time.time - lastSwitchTime < roomSwitchCooldown)
            return;

        lastSwitchTime = Time.time;

        if (currentRoom != null)
            currentRoom.SetActive(false);

        currentRoom = newRoom;
        currentRoom.SetActive(true);

        Debug.Log($"📦 Activando sala: {currentRoom.name} desde puerta '{lastDoorID}' y dirección {lastDirection}");
    }
}

