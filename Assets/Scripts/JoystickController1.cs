using UnityEngine;


public class JoystickController1 : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Assign the DynamicJoystick component here.")]
    public DynamicJoystick joystick;    
    [Tooltip("The CameraRig (parent of the camera) that will be moved and rotated.")]
    public Transform cameraRig;         
    [Tooltip("Speed of linear movement.")]
    public float moveSpeed = 5f;        

    [Header("Rotation Settings")]
    [Tooltip("Speed of camera rotation (for panning and rotating).")]
    public float rotationSpeed = 150f;  

    private int rotationTouchId = -1; 
    void Awake()
    {
        if (joystick == null)
        {
            joystick = FindFirstObjectByType<DynamicJoystick>(); 
        }
    }

    void Update()
    {
        if (cameraRig == null)
        {
            Debug.LogWarning("JoystickController1: CameraRig is not assigned! Movement and rotation will not work.", this);
            return;
        }

        if (joystick != null)
        {
            HandleJoystickMovement();
        }
        else
        {
        }
        
        HandleRotationAndPan();
    }

    void HandleJoystickMovement()
    {
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            Vector3 forward = cameraRig.forward;
            Vector3 right = cameraRig.right;

            forward.y = 0;  
            right.y = 0;    

            forward.Normalize(); 
            right.Normalize();

            Vector3 moveDirection = forward * vertical + right * horizontal;

            cameraRig.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }


    void HandleRotationAndPan()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId == rotationTouchId || (rotationTouchId == -1 && touch.position.x > Screen.width / 2))
            {
                if (rotationTouchId == -1 && touch.phase == TouchPhase.Began)
                {
                    rotationTouchId = touch.fingerId;
                }

                if (touch.fingerId == rotationTouchId)
                {
                    if (touch.phase == TouchPhase.Moved)
                    {
                        float rotationDeltaX = touch.deltaPosition.x;

                        cameraRig.Rotate(Vector3.up, rotationDeltaX * rotationSpeed * Time.deltaTime, Space.World);
                    }
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        rotationTouchId = -1;
                    }
                }
            }
        }
    }
}