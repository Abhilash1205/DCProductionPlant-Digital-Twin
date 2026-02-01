using UnityEngine;


public class DoorAnimatorToggle : MonoBehaviour
{
    [Header("Door Components")]
    [Tooltip("Assign the Animator component of the door. This Animator will play the open/close animations.")]
    public Animator doorAnimator;

    [Header("Animation Parameters")]
    [Tooltip("The name of the boolean parameter in your Animator Controller that triggers the open/close animation (e.g., 'IsOpen').")]
    public string openParameterName = "IsOpen";

    private bool isOpen = false; 

    void Start()
    {
        if (doorAnimator == null)
        {
            Debug.LogError("DoorAnimatorToggle: Door Animator not assigned on " + gameObject.name + ". Please assign it in the Inspector.", this);
            enabled = false; 
            return;
        }

        
        doorAnimator.SetBool(openParameterName, false);
    }

    public void ToggleDoorState()
    {
        isOpen = !isOpen;
        doorAnimator.SetBool(openParameterName, isOpen); 
        Debug.Log(gameObject.name + " door toggled to " + (isOpen ? "Open" : "Closed"));
    }

    public bool IsOpen()
    {
        return isOpen;
    }
}