using Mirror;
using UnityEngine;

public class TankController : NetworkBehaviour
{
    public float moveSpeed = 5.0f; // Adjust this to set the tank's movement speed.
    public float rotationSpeed = 90.0f; // Adjust this to set the tank's rotation speed.
    public CameraController cameraController;
    private Rigidbody tankRigidbody;
    private float targetRotation = 0.0f;
    public Color blueTeamColor;
    public Color redTeamColor;

    void Start()
    {
        // Get the Rigidbody component of the tank.
        tankRigidbody = GetComponent<Rigidbody>();
        cameraController = Camera.main.transform.GetComponent<CameraController>();
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        int playerCount = NetworkServer.connections.Count; // Get the number of connected players.

        Material tankMaterial = new Material(Shader.Find("Standard"));
        if (playerCount % 2 == 1)
        {
            gameObject.layer = LayerMask.NameToLayer("Blue Team Tank"); // Odd number of players, assign "Blue Team" layer.
            tankMaterial.color = blueTeamColor;
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Red Team Tank"); // Even number of players, assign "Red Team" layer.
            tankMaterial.color = redTeamColor;
        }
        
        if (renderers != null)
        {
            foreach (MeshRenderer renderer in renderers) {
                renderer.material = tankMaterial;
            }
        }
        string name = LayerMask.LayerToName(gameObject.layer);
        gameObject.transform.position = GameObject.Find(name.Substring(0,name.Length-5)).transform.Find("Spawner").position;
        gameObject.transform.LookAt(Vector3.zero);
        
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        cameraController.UpdateCamPos(transform.position);
        float verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // Calculate the forward movement.
        Vector3 moveDirection = transform.forward * moveSpeed;

        // Determine the target rotation angle based on the key combinations.
        if (verticalInput > 0 && horizontalInput < 0)
        {
            // W and A (Forward-left)
            targetRotation = -45.0f;
        }
        else if (verticalInput > 0 && horizontalInput > 0)
        {
            // W and D (Forward-right)
            targetRotation = 45.0f;
        }
        else if (verticalInput < 0 && horizontalInput < 0)
        {
            // S and A (Backward-left)
            targetRotation = -135.0f;
        }
        else if (verticalInput < 0 && horizontalInput > 0)
        {
            // S and D (Backward-right)
            targetRotation = 135.0f;
        }
        else if (verticalInput > 0)
        {
            // W (Forward)
            targetRotation = 0.0f;
        }
        else if (verticalInput < 0)
        {
            // S (Backward)
            targetRotation = 180.0f;
        }
        else if (horizontalInput < 0)
        {
            // A (Left)
            targetRotation = -90.0f;
        }
        else if (horizontalInput > 0)
        {
            // D (Right)
            targetRotation = 90.0f;
        }

        // Rotate the tank towards the target angle.
        if (verticalInput != 0 || horizontalInput != 0) {
            tankRigidbody.velocity = moveDirection;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, targetRotation, 0), rotationSpeed * Time.deltaTime);
        }
        else {
            tankRigidbody.velocity = Vector3.zero;
        }
    }
}
