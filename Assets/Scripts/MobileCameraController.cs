using UnityEngine;

public class MobileCameraController : MonoBehaviour
{
    public Camera cam;
    public float zoomSpeed = 0.01f;
    public float minZoom = 2f;
    public float maxZoom = 15f;
    public float panSpeed = 0.005f;
    public float zoomSmoothSpeed = 5f;
    public float panSmoothSpeed = 10f;

    private Vector2 lastPanPosition;
    private int panFingerId;
    private bool isPanning;

    private Vector3 targetPosition;
    private float targetZoom;
    private Quaternion targetRotation;

    public bool allowControl = true;

    void Start()
    {
        targetPosition = cam.transform.position;
        targetZoom = cam.orthographic ? cam.orthographicSize : cam.fieldOfView;
        targetRotation = cam.transform.rotation;
    }

    void Update()
    {
        if (!allowControl) return;

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (!isPanning)
            {
                isPanning = true;
                panFingerId = touch.fingerId;
                lastPanPosition = touch.position;
            }
            else if (touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.position - lastPanPosition;
                lastPanPosition = touch.position;

                Vector3 move = new Vector3(-delta.x * panSpeed, -delta.y * panSpeed, 0f);
                targetPosition += cam.transform.TransformDirection(move);
            }

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                isPanning = false;
        }
        else if (Input.touchCount == 2)
        {
            isPanning = false;

            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);

            Vector2 prevT1 = t1.position - t1.deltaPosition;
            Vector2 prevT2 = t2.position - t2.deltaPosition;

            float prevDistance = (prevT1 - prevT2).magnitude;
            float currDistance = (t1.position - t2.position).magnitude;

            float delta = prevDistance - currDistance;

            if (cam.orthographic)
            {
                targetZoom += delta * zoomSpeed;
                targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            }
            else 
            {
                targetZoom += delta * zoomSpeed * 100f;
                targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            }
        }

        // Smooth Zoom
        if (cam.orthographic)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSmoothSpeed);
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetZoom, Time.deltaTime * zoomSmoothSpeed);
        }

        // Smooth Pan and Rotation
        cam.transform.position = Vector3.Lerp(cam.transform.position, targetPosition, Time.deltaTime * panSmoothSpeed);
        cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, targetRotation, Time.deltaTime * panSmoothSpeed);
    }

    public void SetCameraTarget(Vector3 position, Quaternion rotation)
    {
        allowControl = false; 
        targetPosition = position;
        targetRotation = rotation;
        cam.transform.position = position;
        cam.transform.rotation = rotation;
    }

    public void EnableControl(bool enable)
    {
        allowControl = enable;
    }
}
