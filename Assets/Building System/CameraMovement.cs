using UnityEngine;

public class CameraMovement : MonoBehaviour {
    public Transform target;
    public Vector3 offset;
    public float sensitivity = 5f;
    public float minAngle = -90f;
    public float maxAngle = 90f;
    public bool thirdPerson = false;
    private float yaw = 0f;
    private float pitch = 0f;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate() {
        PlayerMovement playerMovement = target.GetComponent<PlayerMovement>();
        if (playerMovement.isPaused || playerMovement.guiOpen) {return;}
        transform.position = target.position + offset;
        // Rotate camera based on mouse movement
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;

        // Clamp pitch between min and max angles
        pitch = Mathf.Clamp(pitch, minAngle, maxAngle);

        // Apply rotation
        transform.eulerAngles = new Vector3(pitch, yaw, 0f);

        // Follow target 
        transform.position = target.position+offset;
    }
}
