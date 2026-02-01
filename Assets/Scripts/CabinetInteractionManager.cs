using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic; 


public class CabinetInteractionManager : MonoBehaviour
{
    [Header("Door References")]
    [Tooltip("Assign all DoorAnimatorToggle scripts that belong to this cabinet.")]
    public List<DoorAnimatorToggle> controlledDoors; 

    [Header("Global UI Reference")]
    [Tooltip("Assign the SINGLE, GLOBAL UI Button that appears for all interactions.")]
    public Button globalInteractionButton; 

    [Header("UI Text Update")]
    [Tooltip("Text to display on the button when doors are closed.")]
    public string openButtonText = "Open Cabinet";
    [Tooltip("Text to display on the button when doors are open.")]
    public string closeButtonText = "Close Cabinet";

    [Header("Player Detection")]
    [Tooltip("The Layer that your player GameObject (Main Camera) is on.")]
    public LayerMask playerLayer; 


    void Start()
    {
        
        if (globalInteractionButton == null)
        {
            Debug.LogError("CabinetInteractionManager: Global Interaction Button not assigned on " + gameObject.name + ". Please assign the single UI Button from your Canvas in the Inspector.", this);
            enabled = false;
            return;
        }

        if (controlledDoors == null || controlledDoors.Count == 0)
        {
            Debug.LogWarning("CabinetInteractionManager: No doors assigned to be controlled by " + gameObject.name + ". Please assign DoorAnimatorToggle scripts to the 'Controlled Doors' list.", this);
        }

        
        Collider proximityCollider = GetComponent<Collider>();
        if (proximityCollider == null)
        {
            Debug.LogError("CabinetInteractionManager: This GameObject (" + gameObject.name + ") is missing a Collider for proximity detection. Please add one and set it to 'Is Trigger'.", this);
            enabled = false;
            return;
        }
        if (!proximityCollider.isTrigger)
        {
            Debug.LogWarning("CabinetInteractionManager: The Collider on this GameObject (" + gameObject.name + ") is not set to 'Is Trigger'. Proximity detection will not work correctly.", this);
        }

        
        if (Camera.main == null)
        {
            Debug.LogError("CabinetInteractionManager: No camera found with 'MainCamera' tag! Interaction will not work. Please ensure your main camera is tagged 'MainCamera'.", this);
        }

        
        globalInteractionButton.gameObject.SetActive(false);
    }

    
    public void ToggleAllDoors()
    {
        bool targetState = !AreAllDoorsOpen(); 

        foreach (DoorAnimatorToggle door in controlledDoors)
        {
            if (door != null) 
            {
                if (door.IsOpen() != targetState)
                {
                    door.ToggleDoorState();
                }
            }
        }
        UpdateButtonText(); 
    }

    
    private bool AreAllDoorsOpen()
    {
        if (controlledDoors == null || controlledDoors.Count == 0) return false; 

        foreach (DoorAnimatorToggle door in controlledDoors)
        {
            if (door != null && !door.IsOpen())
            {
                return false; 
            }
        }
        return true; 
    }


    private void UpdateButtonText()
    {
        if (globalInteractionButton == null) return;

        string newText = AreAllDoorsOpen() ? closeButtonText : openButtonText;

        if (globalInteractionButton.GetComponentInChildren<Text>() != null) 
        {
            globalInteractionButton.GetComponentInChildren<Text>().text = newText;
        }
        else if (globalInteractionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>() != null)
        {
            globalInteractionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = newText;
        }
    }


    
    void OnTriggerEnter(Collider other)
    {
        
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            globalInteractionButton.gameObject.SetActive(true); 

            globalInteractionButton.onClick.RemoveAllListeners();
            globalInteractionButton.onClick.AddListener(ToggleAllDoors);

            UpdateButtonText(); 

            Debug.Log("Player entered range of cabinet: " + gameObject.name + ". UI shown and linked.", this);
        }
    }

    
    void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            globalInteractionButton.gameObject.SetActive(false); 

            
            globalInteractionButton.onClick.RemoveAllListeners();

            Debug.Log("Player exited range of cabinet: " + gameObject.name + ". UI hidden and unlinked.", this);
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