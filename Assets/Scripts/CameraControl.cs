using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private InputActionReference mouseScroll;
    [SerializeField] private Camera cam;
    private float zoom;
    private float zoomMultiplier = 1f;
    private float minZoom = 4f;
    private float maxZoom = 6f;
    private float velocity = 0f;
    private float smoothTime = 0.25f;

    void Awake()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
        zoom = cam.orthographicSize;
    }

    void Update()
    {
        float scroll = mouseScroll.action.ReadValue<float>();
        zoom -= scroll * zoomMultiplier;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoom, ref velocity, smoothTime);
    }
}
