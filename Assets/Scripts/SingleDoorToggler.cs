using UnityEngine;
    using UnityEngine.UI; 
    using TMPro; 


    public class SingleDoorToggler : MonoBehaviour
    {
        [Header("Door Settings")]
        [Tooltip("Drag the single DOOR GameObject (parent of handle/knob) here.")]
        public GameObject cabinetDoor; 

        [Header("UI Button")]
        [Tooltip("Drag the UI Button that will toggle the door visibility here.")]
        public Button toggleDoorButton; 

        [Tooltip("Optional: Text to display on the button when the door is visible (e.g., 'Hide Door').")]
        public string hideButtonText = "Hide Door";
        [Tooltip("Optional: Text to display on the button when the door is hidden (e.g., 'Show Door').")]
        public string showButtonText = "Show Door";
        [Tooltip("Optional: Reference to the TMP_Text component on the toggleDoorButton.")]
        public TMP_Text toggleButtonText;


        private bool doorIsVisible = true; 

        void Start()
        {
            
            if (cabinetDoor == null)
            {
                Debug.LogWarning("SingleDoorToggler: No cabinet door GameObject assigned! This script will not function.", this);
                enabled = false; 
                return;
            }

            if (toggleDoorButton == null)
            {
                Debug.LogError("SingleDoorToggler: Toggle Door Button is not assigned! Please assign it in the Inspector.", this);
                enabled = false; 
                return;
            }

            toggleDoorButton.onClick.RemoveAllListeners(); 
            toggleDoorButton.onClick.AddListener(ToggleDoorVisibility);

            SetDoorVisibility(doorIsVisible);
        }


        public void ToggleDoorVisibility()
        {
            doorIsVisible = !doorIsVisible; 
            SetDoorVisibility(doorIsVisible); 
        }


        private void SetDoorVisibility(bool isVisible)
        {
            if (cabinetDoor != null)
            {
                cabinetDoor.SetActive(isVisible);
            }

            if (toggleButtonText != null)
            {
                toggleButtonText.text = isVisible ? hideButtonText : showButtonText;
            }
            Debug.Log($"Single Door is now: {(isVisible ? "Visible" : "Hidden")}");
        }
    }