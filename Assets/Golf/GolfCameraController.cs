using UnityEngine;

public class GolfCameraController : MonoBehaviour
{
    public float zoomSpeed = 2.0f; // Adjust this to control camera zoom speed.
    public float minZoomDistance = 2.0f; // Minimum zoom distance from the tank.
    public float maxZoomDistance = 10.0f; // Maximum zoom distance from the tank.
    public float startZoomDistance = 7.5f;
    public float minWorldSpaceFOV = 30.0f; // Minimum FOV for a standard resolution (e.g., 1080p).
    private float currentZoomDistance;

    void Start()
    {
        currentZoomDistance = startZoomDistance;
    }

    void Update()
    {
        // Zoom in and out using the mouse scroll wheel.
        float zoomInput = Input.GetAxis("Mouse ScrollWheel");
        currentZoomDistance -= zoomInput * zoomSpeed;
        currentZoomDistance = Mathf.Clamp(currentZoomDistance, minZoomDistance, maxZoomDistance);
    }

    public void UpdateCamPos(Vector3 newPos)
    {
        newPos.y = currentZoomDistance;
        newPos.z -= currentZoomDistance;
        transform.position = newPos;
    }
}
