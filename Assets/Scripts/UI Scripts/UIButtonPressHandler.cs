using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonPressHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // Flag to indicate whether the button is currently pressed.
    public bool isPressed = false;
    public float pressStartTime = 0f;
    public float pressDuration = 0f;
    public float currentDuration = 0f;
    

    // This method is called when the pointer presses down on the button.
    public void OnPointerDown(PointerEventData eventData)
    {
        
        isPressed = true;
        pressStartTime = Time.time;
        Debug.Log("Button Pressed");
        // You can call other functions or trigger logic here.
    }

    // This method is called when the pointer releases from the button.
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        pressDuration = Time.time - pressStartTime;
        Debug.Log("Button Released after " + pressDuration + "seconds");
        
        // You can call other functions or trigger logic here.
    }

    public void Update()
    {
        if (isPressed)
        {
            float currentDuration = Time.time - pressStartTime;
            // For example, you might want to update a UI element or execute some logic continuously.
            // Debug.Log("Button held for " + currentDuration + " seconds so far.");
        }
    }
}

