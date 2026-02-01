using UnityEngine;
    using UnityEngine.UI; 
    using System.Collections.Generic; 

    
    public class CabinetDoorToggler : MonoBehaviour
    {
        [Header("Cabinet Door Settings")]
        [Tooltip("Drag all 5 control cabinet DOOR GameObjects (the parents of handles/knobs) here.")]
        public List<GameObject> cabinetDoors; 

        [Header("UI Button")]
        [Tooltip("Drag the UI Button that will toggle the cabinet door visibility here.")]
        public Button toggleDoorsButton; 

        [Tooltip("Optional: Text to display on the button when doors are visible (e.g., 'Hide Doors').")]
        public string hideButtonText = "Hide Doors";
        [Tooltip("Optional: Text to display on the button when doors are hidden (e.g., 'Show Doors').")]
        public string showButtonText = "Show Doors";
        [Tooltip("Optional: Reference to the TMP_Text component on the toggleDoorsButton.")]
        public TMPro.TMP_Text toggleButtonText;


        private bool doorsAreVisible = true; 

        void Start()
        {
            
            if (cabinetDoors == null || cabinetDoors.Count == 0)
            {
                Debug.LogWarning("CabinetDoorToggler: No cabinet door GameObjects assigned! This script will not function.", this);
                enabled = false; 
                return;
            }

            if (toggleDoorsButton == null)
            {
                Debug.LogError("CabinetDoorToggler: Toggle Doors Button is not assigned! Please assign it in the Inspector.", this);
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
            foreach (GameObject door in cabinetDoors)
            {
                if (door != null)
                {
                    door.SetActive(isVisible);
                }
            }

            if (toggleButtonText != null)
            {
                toggleButtonText.text = isVisible ? hideButtonText : showButtonText;
            }
            Debug.Log($"Control Cabinet Doors are now: {(isVisible ? "Visible" : "Hidden")}");
        }
    }