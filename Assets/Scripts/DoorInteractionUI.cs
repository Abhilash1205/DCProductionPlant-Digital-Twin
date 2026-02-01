using UnityEngine;
using UnityEngine.UI; 


public class DoorInteractionUI : MonoBehaviour
{
    [Header("Door Components")]
    [Tooltip("Assign the Animator component of the door. This Animator will play the open/close animations.")]
    public Animator doorAnimator;

    [Header("UI Components")]
    [Tooltip("Assign the GameObject of the UI button/text that appears when the player is in range.")]
    public GameObject interactionUIPanel; 

    [Header("Animation Parameters")]
    [Tooltip("The name of the boolean parameter in your Animator Controller that triggers the open/close animation (e.g., 'IsOpen').")]
    public string openParameterName = "IsOpen"; 

    private bool isOpen = false; 

    void Start()
    {
        
        if (doorAnimator == null)
        {
            Debug.LogError("DoorInteractionUI: Door Animator not assigned on " + gameObject.name + ". Please assign it in the Inspector.", this);
            enabled = false; 
            return;
        }

        if (interactionUIPanel == null)
        {
            Debug.LogError("DoorInteractionUI: Interaction UI Panel not assigned on " + gameObject.name + ". Please assign the UI GameObject in the Inspector.", this);
            enabled = false;
            return;
        }

        Collider proximityCollider = GetComponent<Collider>();
        if (proximityCollider == null)
        {
            Debug.LogError("DoorInteractionUI: This GameObject (" + gameObject.name + ") is missing a Collider for proximity detection. Please add one and set it to 'Is Trigger'.", this);
            enabled = false;
            return;
        }
        if (!proximityCollider.isTrigger)
        {
            Debug.LogWarning("DoorInteractionUI: The Collider on this GameObject (" + gameObject.name + ") is not set to 'Is Trigger'. Proximity detection will not work correctly.", this);
        }

        interactionUIPanel.SetActive(false);

        doorAnimator.SetBool(openParameterName, false);
    }


    public void OnInteractButtonClicked()
    {
        
        if (interactionUIPanel.activeSelf) 
        {
            isOpen = !isOpen; 
            doorAnimator.SetBool(openParameterName, isOpen); 
            Debug.Log(gameObject.name + " door toggled to " + (isOpen ? "Open" : "Closed"));
        }
    }

    void OnTriggerEnter(Collider other)
    {
      
        if (other.CompareTag("Player"))
        {
            interactionUIPanel.SetActive(true); 
            Debug.Log("Player entered interaction range for door: " + gameObject.name + ". UI shown.", this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactionUIPanel.SetActive(false); 
            Debug.Log("Player exited interaction range for door: " + gameObject.name + ". UI hidden.", this);
        }
    }
}