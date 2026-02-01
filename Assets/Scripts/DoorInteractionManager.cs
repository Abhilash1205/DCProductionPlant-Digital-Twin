using UnityEngine;
using UnityEngine.UI; 


public class DoorInteractionManager : MonoBehaviour
{
    [Header("Door Components")]
    [Tooltip("Assign the Animator component of the door. This Animator will play the open/close animations.")]
    public Animator doorAnimator;

    [Header("Global UI Reference")]
    [Tooltip("Assign the SINGLE, GLOBAL UI Button that appears for all interactions.")]
    public Button globalInteractionButton; 

    [Header("Animation Parameters")]
    [Tooltip("The name of the boolean parameter in your Animator Controller that triggers the open/close animation (e.g., 'IsOpen').")]
    public string openParameterName = "IsOpen"; 

    private bool isOpen = false; 

    void Start()
    {
        if (doorAnimator == null)
        {
            Debug.LogError("DoorInteractionManager: Door Animator not assigned on " + gameObject.name + ". Please assign it in the Inspector.", this);
            enabled = false;
            return;
        }

        if (globalInteractionButton == null)
        {
            Debug.LogError("DoorInteractionManager: Global Interaction Button not assigned on " + gameObject.name + ". Please assign the single UI Button from your Canvas in the Inspector.", this);
            enabled = false;
            return;
        }

        Collider proximityCollider = GetComponent<Collider>();
        if (proximityCollider == null)
        {
            Debug.LogError("DoorInteractionManager: This GameObject (" + gameObject.name + ") is missing a Collider for proximity detection. Please add one and set it to 'Is Trigger'.", this);
            enabled = false;
            return;
        }
        if (!proximityCollider.isTrigger)
        {
            Debug.LogWarning("DoorInteractionManager: The Collider on this GameObject (" + gameObject.name + ") is not set to 'Is Trigger'. Proximity detection will not work correctly.", this);
        }

        if (Camera.main == null)
        {
            Debug.LogError("DoorInteractionManager: No camera found with 'MainCamera' tag! Interaction will not work. Please ensure your main camera is tagged 'MainCamera'.", this);
        }

        
        doorAnimator.SetBool(openParameterName, false);
    }


    public void ToggleDoorState()
    {
        isOpen = !isOpen; 
        doorAnimator.SetBool(openParameterName, isOpen); 
        Debug.Log(gameObject.name + " door toggled to " + (isOpen ? "Open" : "Closed"));

        if (globalInteractionButton.GetComponentInChildren<Text>() != null) 
        {
            globalInteractionButton.GetComponentInChildren<Text>().text = isOpen ? "Close Door" : "Open Door";
        }
        else if (globalInteractionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>() != null) 
        {
            globalInteractionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = isOpen ? "Close Door" : "Open Door";
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            globalInteractionButton.gameObject.SetActive(true);

            globalInteractionButton.onClick.RemoveAllListeners();
            globalInteractionButton.onClick.AddListener(ToggleDoorState);

            if (globalInteractionButton.GetComponentInChildren<Text>() != null)
            {
                globalInteractionButton.GetComponentInChildren<Text>().text = isOpen ? "Close Door" : "Open Door";
            }
            else if (globalInteractionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>() != null)
            {
                globalInteractionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = isOpen ? "Close Door" : "Open Door";
            }

            Debug.Log("Player entered range of " + gameObject.name + ". UI shown and linked.", this);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            globalInteractionButton.gameObject.SetActive(false);


            globalInteractionButton.onClick.RemoveAllListeners();

            Debug.Log("Player exited range of " + gameObject.name + ". UI hidden and unlinked.", this);
        }
    }
    void OnDisable()
    {
        if (globalInteractionButton != null)
        {
            globalInteractionButton.gameObject.SetActive(false);
            globalInteractionButton.onClick.RemoveAllListeners();
        }
    }
}