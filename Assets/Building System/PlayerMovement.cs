using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float crouchSpeed = 3f;
    public float jumpForce = 10f;
    public float gravity = -9.81f;
    public float standingHeight = 1f;
    public float crouchHeight = 0.8f;
    public float crouchTransitionSpeed = 2.0f;
    public float raycastDist = 1.5f;
    public bool isPaused = false;
    public bool guiOpen = false;
    CharacterController controller;
    Vector3 velocity;
    public string state;

    void Start() {
        controller = GetComponent<CharacterController>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && !guiOpen) {
            isPaused = !isPaused; 
        }

        if (isPaused || guiOpen) {
            return;
        }
        // Check if jumping
        if(Input.GetKey(KeyCode.Space) && state == "OnGround") {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
        
        // Calculate movement direction
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = forward.normalized;
        Vector3 right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;

        Vector3 direction = forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal");
        
        // Determine speed
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        if (state == "InAir") {speed = walkSpeed;}

        bool isCrouching = Input.GetKey(KeyCode.LeftControl) && state == "OnGround";
        if (isCrouching) {
            float crouchAmount = Mathf.Clamp(crouchTransitionSpeed*Time.deltaTime, 0, transform.localScale.y-crouchHeight);
            transform.localScale = new Vector3(transform.localScale.x, Mathf.Clamp(transform.localScale.y-crouchAmount, crouchHeight, standingHeight), transform.localScale.z);
            controller.Move(-new Vector3(0, crouchAmount, 0));
            speed = crouchSpeed;
        } else {
            float crouchAmount = Mathf.Clamp(crouchTransitionSpeed*Time.deltaTime, 0, standingHeight-transform.localScale.y);
            transform.localScale = new Vector3(transform.localScale.x, Mathf.Clamp(transform.localScale.y+crouchAmount, crouchHeight, standingHeight), transform.localScale.z);
            controller.Move(new Vector3(0, crouchAmount, 0));
        }

        // Apply gravity
            velocity.y += gravity * Time.deltaTime;

        // Move controller
        controller.Move((direction * speed + velocity) * Time.deltaTime);

        // Set state
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, raycastDist)) {
              state = "OnGround"; velocity.y = 0;
        } else {
            state = "InAir";
        }
    }
}