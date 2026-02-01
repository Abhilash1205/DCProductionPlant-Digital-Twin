using UnityEngine;


public class NonLinearCameraNavigator : MonoBehaviour
{
    [Header("Non-Linear Navigation Targets")]
    [Tooltip("Drag the Transforms representing your non-linear jump destinations here.")]
    public Transform[] nonLinearTargets;

    [Header("Camera Reference")]
    [Tooltip("Drag your CameraRig (parent of Main Camera) here.")]
    public Transform cameraRig; 

    [Header("Linear Navigator Reference")]
    [Tooltip("Drag the CameraStepNavigator script instance here to synchronize linear step tracking.")]
    public CameraStepNavigator cameraStepNavigator; 



    void Start()
    {
        if (cameraRig == null)
        {
            Debug.LogWarning("NonLinearCameraNavigator: CameraRig is not assigned! Non-linear navigation will not work.", this);
        }
        if (cameraStepNavigator == null)
        {
            Debug.LogWarning("NonLinearCameraNavigator: CameraStepNavigator is not assigned! Linear navigation will not sync after jumps.", this);
        }
    }

    
    public void JumpToTarget(int targetIndex)
    {
        if (nonLinearTargets == null || targetIndex < 0 || targetIndex >= nonLinearTargets.Length)
        {
            Debug.LogWarning($"NonLinearCameraNavigator: Invalid target index: {targetIndex}. Array size: {(nonLinearTargets != null ? nonLinearTargets.Length.ToString() : "0")}", this);
            return;
        }

        Transform targetTransform = nonLinearTargets[targetIndex];
        if (targetTransform == null)
        {
            Debug.LogWarning($"NonLinearCameraNavigator: Target Transform at index {targetIndex} is null. Cannot jump.", this);
            return;
        }

        Debug.Log($"NonLinearCameraNavigator: Jumping to target: {targetTransform.name} (Index: {targetIndex})");

        if (cameraRig != null)
        {
            cameraRig.position = targetTransform.position;
            cameraRig.rotation = targetTransform.rotation;

            
            if (cameraStepNavigator != null)
            {
                cameraStepNavigator.SetCurrentStepIndex(targetIndex);
            }
        }
        else
        {
            Debug.LogError("NonLinearCameraNavigator: CameraRig is null. Cannot move camera.");
        }
    }
}