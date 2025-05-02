using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public GameObject roomToActivate;
    public RoomManager.Direction requiredDirection = RoomManager.Direction.None;
    public string expectedDoorID = "";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        bool directionOK = RoomManager.Instance.lastDirection == OppositeDirection(requiredDirection);
        bool doorOK = RoomManager.Instance.lastDoorID == expectedDoorID;

        if (directionOK && doorOK)
        {
            RoomManager.Instance.SetCurrentRoom(roomToActivate);
        }
    }

    private RoomManager.Direction OppositeDirection(RoomManager.Direction dir)
    {
        switch (dir)
        {
            case RoomManager.Direction.Left: return RoomManager.Direction.Right;
            case RoomManager.Direction.Right: return RoomManager.Direction.Left;
            case RoomManager.Direction.Up: return RoomManager.Direction.Down;
            case RoomManager.Direction.Down: return RoomManager.Direction.Up;
            default: return RoomManager.Direction.None;
        }
    }
}
