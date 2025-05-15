using UnityEngine;

public class Button : MonoBehaviour
{
    public bool isPressed = false;
    public GameObject triggerObj; //Object to trigger when button is pressed
    public int seqNum;

    public AudioManager audiomanager;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        animator.SetBool("isPressed", isPressed);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(!isPressed) {
            isPressed = true;
            triggerObj.GetComponent<LockedDoor>().checkButtons(seqNum);
        }
    }
}
