using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    public GameObject[] buttons;
    public bool isOpen = false;
    public AudioManager audiomanager;
    private int seqCount = 0;
    private int buttonsPressed = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        Renderer renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update() {
        //If the door is opened then destroy it
        if(isOpen) {
            Destroy(gameObject);
        }
    }

    //Checks the buttons pressed by the the player, this is called by Buttons.cs
    public void checkButtons(int seq) {
        Debug.Log("Checked Door");
        if(buttons.Length != 0 && !isOpen) { //If the door has buttons listed AND is not open
            // If there are more than 2+ buttons
            if(buttons.Length > 1) {
                buttonsPressed++; //Inc the buttons pressed
                if(seq - 1 == seqCount) { //If the button that called this is of the right seq order
                    seqCount++;
                }
                if(buttonsPressed >= buttons.Length) { //If all the buttons are pressed
                    if(seqCount >= buttons.Length) { //AND pressed in the right sequence 
                        isOpen = true;
                    } else { //ELSE reset all the buttons
                        for(int i = 0; i < buttons.Length; i++) {
                            buttons[i].GetComponent<Button>().isPressed = false;
                        }
                    }
                    buttonsPressed = 0;
                    seqCount = 0;
                }
            } else {
            //ELSE if these is only 1 button to press, open the door
                if(buttons[0].GetComponent<Button>().isPressed) {
                    isOpen = true;
                }
            }
        }
    }
}
