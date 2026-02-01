using UnityEngine;
    using UnityEngine.UI; 
    using TMPro; 


    public class DoubleDoorToggler : MonoBehaviour
    {
        [Header("Double Door Settings")]
        [Tooltip("Drag the Left Door GameObject (parent of handle/knob) here.")]
        public GameObject leftDoor; 
        [Tooltip("Drag the Right Door GameObject (parent of handle/knob) here.")]
        public GameObject rightDoor; 

        [Header("UI Button")]
        [Tooltip("Drag the UI Button that will toggle the doors visibility here.")]
        public Button toggleDoorsButton;

        [Tooltip("Optional: Text to display on the button when doors are visible (e.g., 'Hide Doors').")]
        public string hideButtonText = "Hide Doors";
        [Tooltip("Optional: Text to display on the button when doors are hidden (e.g., 'Show Doors').")]
        public string showButtonText = "Show Doors";
        [Tooltip("Optional: Reference to the TMP_Text component on the toggleDoorsButton.")]
        public TMP_Text toggleButtonText;


        private bool doorsAreVisible = true; 
        void Start()
        {
            
            if (leftDoor == null || rightDoor == null)
            {
                Debug.LogWarning("DoubleDoorToggler: One or both door GameObjects are not assigned! This script will not function.", this);
                enabled = false; 
                return;
            }

            if (toggleDoorsButton == null)
            {
                Debug.LogError("DoubleDoorToggler: Toggle Doors Button is not assigned! Please assign it in the Inspector.", this);
                enabled = false; 
                return;
            }

            
            toggleDoorsButton.onClick.RemoveAllListeners(); 
            toggleDoorsButton.onClick.AddListener(ToggleDoorsVisibility);

            
            SetDoorsVisibility(doorsAreVisible);
        }

        public void ToggleDoorsVisibility()
        {
            doorsAreVisible = !doorsAreVisible; 
            SetDoorsVisibility(doorsAreVisible);
        }

       
        private void SetDoorsVisibility(bool isVisible)
        {
            if (leftDoor != null)
            {
                leftDoor.SetActive(isVisible);
            }
            if (rightDoor != null)
            {
                rightDoor.SetActive(isVisible);
            }

            if (toggleButtonText != null)
            {
                toggleButtonText.text = isVisible ? hideButtonText : showButtonText;
            }
            Debug.Log($"Double Doors are now: {(isVisible ? "Visible" : "Hidden")}");
        }
    }