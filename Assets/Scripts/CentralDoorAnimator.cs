using System.Collections;
    using UnityEngine;
    using UnityEngine.UI; 
    using TMPro; 

    public class CentralDoorAnimator : MonoBehaviour
    {
        [Header("Door References")]
        [Tooltip("Drag the Left Door GameObject here. Ensure its pivot is at the hinge point.")]
        public GameObject leftDoor; 
        [Tooltip("Drag the Right Door GameObject here. Ensure its pivot is at the hinge point.")]
        public GameObject rightDoor; 

        [Header("Door Animation Settings")]
        [Tooltip("The rotation (Euler angles) of the left door when fully OPENED.")]
        public Vector3 leftDoorOpenRotationEuler = new Vector3(0, -90, 0); 
        [Tooltip("The rotation (Euler angles) of the right door when fully OPENED.")]
        public Vector3 rightDoorOpenRotationEuler = new Vector3(0, 90, 0); 
        [Tooltip("Duration of the door opening/closing animation in seconds.")]
        public float animationDuration = 1.0f; 

        [Header("UI Interaction Button")]
        [Tooltip("Drag the SINGLE UI Button that will appear for door interaction.")]
        public Button interactionButton;
        [Tooltip("Optional: Reference to the TMP_Text component on the interactionButton.")]
        public TMP_Text buttonText; 

        [Tooltip("Text to display on the button when doors are closed.")]
        public string openButtonText = "Open Doors";
        [Tooltip("Text to display on the button when doors are open.")]
        public string closeButtonText = "Close Doors";

        [Header("Player Detection")]
        [Tooltip("The Layer(s) that your player GameObject (or CameraRig) is on.")]
        public LayerMask playerLayer; 

        
        private Quaternion leftDoorClosedRotation;
        private Quaternion rightDoorClosedRotation;
        private Quaternion leftDoorTargetRotation;
        private Quaternion rightDoorTargetRotation;
        private bool doorsAreOpen = false;
        private Coroutine animationCoroutine; 


        void Awake()
        {
            if (leftDoor == null || rightDoor == null)
            {
                Debug.LogError("CentralDoorAnimator: LeftDoor or RightDoor is not assigned! This script will not function.", this);
                enabled = false;
                return;
            }

            
            leftDoorClosedRotation = leftDoor.transform.localRotation;
            rightDoorClosedRotation = rightDoor.transform.localRotation;

            if (interactionButton != null)
            {
                interactionButton.gameObject.SetActive(false);
                if (buttonText != null)
                {
                    buttonText.text = openButtonText;
                }
            }
            else
            {
                Debug.LogError("CentralDoorAnimator: Interaction Button is not assigned! Please assign it in the Inspector.", this);
                enabled = false;
                return;
            }
        }


        void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & playerLayer) != 0)
            {
                Debug.Log($"CentralDoorAnimator: Player ({other.gameObject.name}) entered door range.", this);
                if (interactionButton != null)
                {
                    interactionButton.gameObject.SetActive(true);
                    interactionButton.onClick.RemoveAllListeners();
                    interactionButton.onClick.AddListener(ToggleDoors); 
                    UpdateButtonText(); 
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (((1 << other.gameObject.layer) & playerLayer) != 0)
            {
                Debug.Log($"CentralDoorAnimator: Player ({other.gameObject.name}) exited door range.", this);
                if (interactionButton != null)
                {
                    interactionButton.gameObject.SetActive(false); 
                    interactionButton.onClick.RemoveAllListeners(); 
                }
            }
        }

        public void ToggleDoors()
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
            }

            doorsAreOpen = !doorsAreOpen; 
            UpdateButtonText();           

            leftDoorTargetRotation = doorsAreOpen ? Quaternion.Euler(leftDoorOpenRotationEuler) : leftDoorClosedRotation;
            rightDoorTargetRotation = doorsAreOpen ? Quaternion.Euler(rightDoorOpenRotationEuler) : rightDoorClosedRotation;

            animationCoroutine = StartCoroutine(AnimateDoors(leftDoorTargetRotation, rightDoorTargetRotation));
        }

        IEnumerator AnimateDoors(Quaternion targetLeftRotation, Quaternion targetRightRotation)
        {
            float elapsedTime = 0f;
            Quaternion startLeftRotation = leftDoor.transform.localRotation;
            Quaternion startRightRotation = rightDoor.transform.localRotation;

            while (elapsedTime < animationDuration)
            {
                
                float t = elapsedTime / animationDuration;
                t = t * t * (3f - 2f * t);

                leftDoor.transform.localRotation = Quaternion.Slerp(startLeftRotation, targetLeftRotation, t);
                rightDoor.transform.localRotation = Quaternion.Slerp(startRightRotation, targetRightRotation, t);

                elapsedTime += Time.deltaTime;
                yield return null; 
            }

            leftDoor.transform.localRotation = targetLeftRotation;
            rightDoor.transform.localRotation = targetRightRotation;

            animationCoroutine = null; 
            Debug.Log($"CentralDoorAnimator: Doors {(doorsAreOpen ? "opened" : "closed")}.");
        }

        private void UpdateButtonText()
        {
            if (buttonText != null)
            {
                buttonText.text = doorsAreOpen ? closeButtonText : openButtonText;
            }
        }
    }