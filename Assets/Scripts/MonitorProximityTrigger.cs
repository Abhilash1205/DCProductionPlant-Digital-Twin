using UnityEngine;
using UnityEngine.UI;


public class MonitorProximityTrigger : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Drag the 'Open Monitor' Button (BtnOpenMonitor) here from the Canvas.")]
    public Button btnOpenMonitor; 

    [Header("Player Settings")]
    [Tooltip("The tag assigned to your player/camera GameObject. Ensure your player has this tag.")]
    public string playerTag = "Player"; 
    [Tooltip("Drag your Player/Camera GameObject here so we can get its transform.")]
    public Transform playerTransform; 

    [Header("Facing Detection Settings")]
    [Tooltip("The maximum angle (in degrees) between the player's forward direction and the monitor's forward direction for interaction to be allowed.")]
    [Range(0, 180)]
    public float maxFacingAngle = 45f;
    [Tooltip("The transform representing the front direction of the monitor. Usually the monitor's own transform.")]
    public Transform monitorFacingTransform;

    
    private bool isInRange = false;

    private bool CanInteract
    {
        get
        {
            if (!isInRange || playerTransform == null || monitorFacingTransform == null)
            {
                return false;
            }

            Vector3 playerForward = playerTransform.forward;
            Vector3 monitorForward = monitorFacingTransform.forward;

            playerForward.Normalize();
            monitorForward.Normalize();

            float dotProduct = Vector3.Dot(playerForward, monitorForward);
            float cosMaxAngle = Mathf.Cos(maxFacingAngle * Mathf.Deg2Rad);

            bool isFacing = dotProduct >= cosMaxAngle;
            
            return isInRange && isFacing;
        }
    }


    private void Start()
    {
        
        if (btnOpenMonitor != null)
        {
            btnOpenMonitor.gameObject.SetActive(false);
            btnOpenMonitor.interactable = false;
            Debug.Log("MonitorProximityTrigger: BtnOpenMonitor forced to inactive state at Start.");
        }
        else
        {
            Debug.LogWarning("MonitorProximityTrigger: btnOpenMonitor is not assigned in the Inspector at Start!", this);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isInRange = true;
            Debug.Log("MonitorProximityTrigger: Player entered monitor range.");
            UpdateOpenMonitorButtonState(); 
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isInRange = false;
            Debug.Log("MonitorProximityTrigger: Player exited monitor range.");
            UpdateOpenMonitorButtonState(); 
        }
    }


    private void Update()
    {
        if (isInRange)
        {
            UpdateOpenMonitorButtonState();
        }
    }


    private void UpdateOpenMonitorButtonState()
    {
        bool shouldBeActive = CanInteract; 
        if (btnOpenMonitor != null)
        {
            btnOpenMonitor.gameObject.SetActive(shouldBeActive);
            btnOpenMonitor.interactable = shouldBeActive;
        }
        else
        {
            Debug.LogWarning("MonitorProximityTrigger: btnOpenMonitor is not assigned in the Inspector!", this);
        }

        if (playerTransform == null)
        {
            Debug.LogWarning("MonitorProximityTrigger: Player Transform is not assigned!", this);
        }
        if (monitorFacingTransform == null)
        {
            Debug.LogWarning("MonitorProximityTrigger: Monitor Facing Transform is not assigned!", this);
        }
    }
}