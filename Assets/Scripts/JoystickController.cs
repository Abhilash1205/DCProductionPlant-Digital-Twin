using UnityEngine;

public class JoystickController : MonoBehaviour
{
    public DynamicJoystick joystick;  
    public Transform cameraRig;      
    public float moveSpeed = 5f;    
    public float rotationSpeed = 150f; 
    private float targetRotationY;

    private Vector2 lastTouchPosition;
    private bool isRotating = false;

    void Awake()
    {
        if (joystick == null)
        {
            joystick = Object.FindFirstObjectByType<DynamicJoystick>();
        }
    }

    void Update()
    {
        if (joystick == null || cameraRig == null)
            return;

        HandleJoystickMovement();  
        HandleRotationAndPan();    
    }

    void HandleJoystickMovement()
    {
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;

        Vector3 forward = cameraRig.forward;
        Vector3 right = cameraRig.right;

        forward.y = 0; 
        right.y = 0;    

        Vector3 moveDirection = forward * vertical + right * horizontal;

        cameraRig.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    void HandleRotationAndPan()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);  

            if (touch.position.x > Screen.width / 2)  
            {
                if (touch.phase == TouchPhase.Began)
                {
                    lastTouchPosition = touch.position;
                    isRotating = true;
                }

                if (touch.phase == TouchPhase.Moved && isRotating)
                {
                    float rotationDelta = touch.deltaPosition.x;

                    targetRotationY += rotationDelta * rotationSpeed * Time.deltaTime;

                    targetRotationY = Mathf.Repeat(targetRotationY, 360f);

                    cameraRig.rotation = Quaternion.Euler(0, targetRotationY, 0);

                    lastTouchPosition = touch.position;
                }

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    isRotating = false;
                }

            }
        }
    }
}