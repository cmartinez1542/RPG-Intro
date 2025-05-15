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
        if(isOpen) {
            Destroy(gameObject);
        } else {

        }
    }

    public void checkButtons(int seq) {
        Debug.Log("Checked Door");
        if(buttons.Length != 0 && !isOpen) {
            if(buttons.Length > 1) {
                buttonsPressed++;
                if(seq - 1 == seqCount) {
                    seqCount++;
                }
                if(buttonsPressed >= buttons.Length) {
                    if(seqCount >= buttons.Length) {
                        isOpen = true;
                    } else {
                        for(int i = 0; i < buttons.Length; i++) {
                            buttons[i].GetComponent<Button>().isPressed = false;
                        }
                    }
                    buttonsPressed = 0;
                    seqCount = 0;
                }
            } else {
                if(buttons[0].GetComponent<Button>().isPressed) {
                    isOpen = true;
                }
            }
        }
    }
}
