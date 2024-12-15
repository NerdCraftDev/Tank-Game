using UnityEngine;
public class Placement : MonoBehaviour
{
    public Transform gridContainer; // Parent object for organizing grid elements
    public GameObject[] buildingPrefabs; // Array of building prefabs (beam, tall beam, short beam, wall, window wall, ceiling, door)

    private GameObject previewObject; // Transparent preview object
    private int selectedBuildingIndex = 0; // Index of the selected building type

    private const float gridSize = 1f; // Size of the grid for snapping
    private bool buildMode = false;
    public float reachDist = 5;

    void Update()
    {
        UpdatePreviewPosition();
        CheckUserInput();
    }

    void CreatePreviewObject()
    {
        previewObject = Instantiate(buildingPrefabs[selectedBuildingIndex], Vector3.zero, Quaternion.identity);
        previewObject.GetComponent<Collider>().enabled = false; // Disable collider for the preview object
    }

    void UpdatePreviewPosition()
    {
        if (!buildMode) {return;}
        // Raycast to the ground and update the position of the preview object
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, reachDist))
        {
            previewObject.SetActive(true);
            Vector3 gridPosition = GetNearestGridPosition(hit.point);
            previewObject.transform.position = gridPosition;
        } else {
            previewObject.SetActive(false);
        }
    }

    Vector3 GetNearestGridPosition(Vector3 position)
    {
        float x = Mathf.Floor(position.x / gridSize) * gridSize + gridSize / 2f;
        float z = Mathf.Floor(position.z / gridSize) * gridSize + gridSize / 2f;
        float y = 0;
        if (Physics.Raycast(new Vector3(x, 1000, z), Vector3.down, out RaycastHit hit)) {
            y = hit.point.y+previewObject.transform.localScale.y/2;
        }

        return new Vector3(x, y, z);
    }

    void CheckUserInput()
    {
        if (Input.GetKeyDown(KeyCode.B)) {buildMode = !buildMode;}
        if (!buildMode) {
            Destroy(previewObject);
        } else {
            if (previewObject == null) {CreatePreviewObject();}
            if (Input.GetKeyDown(KeyCode.R)) {previewObject.transform.Rotate(0, 90, 0);}
        }
        if (Input.GetMouseButtonDown(0) && buildMode)
        {
            PlaceBuildingObject();
        }

        // Handle number key input to select different building options
        for (int i = 1; i <= buildingPrefabs.Length; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                selectedBuildingIndex = i - 1;
                Destroy(previewObject);
                CreatePreviewObject();
            }
        }
    }

    void PlaceBuildingObject()
    {
        // Instantiate the selected building object at the preview position
        GameObject newBuilding = Instantiate(buildingPrefabs[selectedBuildingIndex], previewObject.transform.position, previewObject.transform.rotation);

        // Adjust the structure based on neighboring objects or conditions (e.g., slanting ceiling)
        AdjustStructure(newBuilding);

        // Update the preview object
        UpdatePreviewPosition();
    }

    void AdjustStructure(GameObject newBuilding)
    {
        // Implement logic to adjust the structure based on neighboring objects or conditions
        // For example, check neighboring beams and adjust the orientation of the ceiling
    }
}
