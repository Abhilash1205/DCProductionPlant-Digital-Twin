using UnityEngine;


public class CameraStepNavigator : MonoBehaviour
{
    [Tooltip("Drag all Transform targets here for linear camera steps. Index 0 is start, last index is end.")]
    public Transform[] cameraSteps; 

    [Tooltip("Drag your CameraRig (parent of the camera) here.")]
    public Transform cameraRig; 

    private int currentStep = 0;

    void Start()
    {
        if (cameraSteps == null || cameraSteps.Length == 0)
        {
            Debug.LogWarning("CameraStepNavigator: No camera steps assigned! Linear navigation will not work.", this);
            enabled = false;
            return;
        }

        currentStep = Mathf.Clamp(currentStep, 0, cameraSteps.Length - 1);
        MoveToStep(currentStep); 
    }


    public void NextStep()
    {
        if (currentStep < cameraSteps.Length - 1)
        {
            currentStep++;
            MoveToStep(currentStep);
            Debug.Log($"CameraStepNavigator: Moved to next step: {currentStep} ({cameraSteps[currentStep].name})");
        }
        else
        {
            Debug.Log("CameraStepNavigator: Already at the last step. Cannot go further.");
        }
    }


    public void PreviousStep()
    {
        if (currentStep > 0)
        {
            currentStep--;
            MoveToStep(currentStep);
            Debug.Log($"CameraStepNavigator: Moved to previous step: {currentStep} ({cameraSteps[currentStep].name})");
        }
        else
        {
            Debug.Log("CameraStepNavigator: Already at the first step. Cannot go back.");
        }
    }

    void MoveToStep(int index)
    {
        if (cameraSteps == null || index < 0 || index >= cameraSteps.Length)
        {
            Debug.LogWarning($"CameraStepNavigator: Invalid step index provided: {index}. Cannot move camera.", this);
            return;
        }

        currentStep = index; 

        Vector3 targetPos = cameraSteps[index].position;
        Quaternion targetRot = cameraSteps[index].rotation;

        if (cameraRig != null && cameraRig.TryGetComponent(out MobileCameraController controller))
        {
            controller.SetCameraTarget(targetPos, targetRot);
        }
        else if (cameraRig != null)
        {
            cameraRig.position = targetPos;
            cameraRig.rotation = targetRot;
        }
        else
        {
            Debug.LogError("CameraStepNavigator: CameraRig is not assigned! Cannot move camera.", this);
        }
    }

    public void TeleportToStep(int index)
    {
        if (index >= 0 && cameraSteps != null && index < cameraSteps.Length)
        {
            MoveToStep(index);
            Debug.Log($"CameraStepNavigator: Teleported to step: {index} ({cameraSteps[index].name})");
        }
        else
        {
            Debug.LogWarning($"CameraStepNavigator: Attempted to teleport to an invalid step index: {index}", this);
        }
    }

    public void SetCurrentStepIndex(int newStepIndex)
    {
        currentStep = Mathf.Clamp(newStepIndex, 0, cameraSteps.Length - 1);
        Debug.Log($"CameraStepNavigator: Internal currentStep updated to: {currentStep} (via external call)");
    }
}