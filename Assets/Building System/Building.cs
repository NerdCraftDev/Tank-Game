using UnityEngine;

public class Building : MonoBehaviour
{
    public GameObject[] buildingPrefabs; // Array of building prefabs (beam, tall beam, short beam, wall, window wall, ceiling, door)
    private GameObject previewObject; // Transparent preview object
    private int selectedBuildingIndex = 0; // Index of the selected building type
    private const float gridSize = 1f; // Size of the grid for snapping
    private bool buildMode = false;
    public float reachDist = 5;
    public float checkGridDist = 2;
    public Material buildingPiecesMaterial;
    public int layerToCheck = 4;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) {buildMode = !buildMode;}
        if (buildMode) {
            for (int i = 1; i <= buildingPrefabs.Length; i++) {
                if (Input.GetKeyDown(i.ToString())) {
                    selectedBuildingIndex = i - 1;
                    Destroy(previewObject);
                    previewObject = Instantiate(buildingPrefabs[selectedBuildingIndex]);
                }
            }
            
            if (previewObject == null) {previewObject = Instantiate(buildingPrefabs[0]);}

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, reachDist)) {
                previewObject.SetActive(true);
                previewObject.transform.position = GetGridPosition(hit.point, previewObject);
            } else {
                previewObject.SetActive(false);
            }

            if (Input.GetMouseButtonDown(0)) {
                GameObject obj = Instantiate(buildingPrefabs[selectedBuildingIndex], previewObject.transform.position, previewObject.transform.rotation);
                obj.layer = LayerMask.NameToLayer("Building Pieces");
                obj.GetComponent<MeshRenderer>().material = buildingPiecesMaterial;
            }
        } else {
            if (previewObject != null) {Destroy(previewObject);}
        }
        if (Input.GetKeyDown(KeyCode.R)) {previewObject.transform.Rotate(0, 90, 0);}
    }

    Vector3 GetGridPosition(Vector3 loc, GameObject obj) {
        Collider collider = LayerInRadius(loc, checkGridDist, layerToCheck);
        if (collider != null) {
            Vector3 gridSnap = collider.gameObject.transform.position;
            Vector3 difference = loc - gridSnap;
            float x = Mathf.Round(difference.x);
            float y = gridSnap.y-(collider.transform.localScale.y-obj.transform.localScale.y)/2 - (collider.transform.localScale.y/2);
            y += Mathf.Round(loc.y-(gridSnap.y-collider.transform.localScale.y/2));
            float z = Mathf.Round(difference.z);
            return new Vector3(x, y, z) + gridSnap;
        } else {
            return loc + Vector3.up*obj.transform.localScale.y/2;
        }
        
    }

    public Collider LayerInRadius(Vector3 checkPos, float radius, int layerMask) {
        
        Collider[] colliders = Physics.OverlapBox(checkPos, Vector3.one * radius, Quaternion.identity, ~layerMask);
        foreach (Collider collider in colliders) {
            if (collider.gameObject.layer == layerToCheck) {
                Debug.Log(collider.name);
                return collider;
            }
        }

        return null;
    }
}
