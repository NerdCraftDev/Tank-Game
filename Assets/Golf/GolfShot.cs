using UnityEngine;

public class GolfShot : MonoBehaviour {
    [SerializeField] private Gradient powerGradient;  
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Rigidbody golfBall;
    [SerializeField] private Collider golfBallCollider;
    [SerializeField] private GolfCameraController golfCameraController;
    [SerializeField] private float maxPower = 10f;
    [SerializeField] private float powerScale = 1f;
    [SerializeField] private float minY;
    [SerializeField] private float minSpeed = 1f;
    private Vector3 force; 
    private bool shouldShoot = false;
    private Vector2 startPos;
    private Vector2 endPos;
  
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, golfCameraController.maxZoomDistance+5)) {
                if (hit.collider == golfBallCollider) {
                    startPos = Input.mousePosition;
                    shouldShoot = true;
                }
            }
        }
        if (Input.GetMouseButton(0) && shouldShoot) {
            endPos = Input.mousePosition;
            
            // Calculate direction and distance
            Vector2 direction = startPos - endPos; 
            float distance = Vector2.Distance(startPos, endPos);
            
            // Limit power
            distance = Mathf.Min(distance, maxPower);
            
            // Add force
            force = direction.normalized * distance;
            force = new Vector3(force.x, 0, force.y) * powerScale;

            // Update line
            // lineRenderer.SetPosition(0, transform.position);
            // lineRenderer.SetPosition(1, transform.position-force/(maxPower*powerScale/2)); 
            // lineRenderer.material.color = powerGradient.Evaluate(force.magnitude/(maxPower*powerScale));
            Debug.DrawLine(startPos, endPos, powerGradient.Evaluate(force.magnitude/(maxPower*powerScale)));
        }
        if (Input.GetMouseButtonUp(0) && shouldShoot) {
            golfBall.velocity = force;
            
            // Reset
            startPos = Vector3.zero; 
            endPos = Vector3.zero;
            shouldShoot = false;
        }
        if (transform.position.y < minY) {
            transform.position = Vector3.up*transform.localScale.y/2;
            golfBall.velocity = Vector3.zero;
            golfBall.angularVelocity = Vector3.zero;
        }
        if (golfBall.angularVelocity.magnitude < minSpeed) {
            golfBall.angularVelocity = Vector3.zero;
        }
        golfCameraController.UpdateCamPos(transform.position);
    }
}