using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    public GameObject branchPrefab;  // Prefab for branches
    public GameObject leafPrefab;    // Prefab for leaves
    public float maxDist = 5.0f;
    public float maxHeight = 10.0f;
    public float growRate = 1.0f;
    public float growAmount = 0.25f;
    private List<Tree> trees = new List<Tree>();
    private bool allTreesGrown = false;

    void Start()
    {
        // Start with a single trunk
        AddTrees(1);
        StartCoroutine(GrowTrees());
    }

    void AddTrees(int numTrees)
    {
        for (int i = 0; i < numTrees; i++) {
            GameObject startObj = Instantiate(branchPrefab);
            startObj.name = "Tree " + (i+1);
            startObj.transform.localScale += new Vector3(0.4f, 0, 0.4f);
            startObj.transform.position = new Vector3(Random.Range(-maxDist, maxDist), startObj.transform.localScale.y/2, Random.Range(-maxDist, maxDist));
            Tree tree = new Tree(1, startObj);
            trees.Add(tree);
        }
    }

    IEnumerator GrowTrees() {
        while (!allTreesGrown) {
            allTreesGrown = true;
            foreach (Tree tree in trees) {
                float height = tree.GetHeight();
                if (height < maxHeight) {
                    tree.AddHeight(growAmount);
                    if (Random.Range(0f,1f) < 0.5f) {
                        AddBranch(tree);
                    }
                    allTreesGrown = false;
                }
            }
            yield return new WaitForSeconds(growRate);
        }
    }

    void AddBranch(Tree tree) {
        float actualHeight = tree.GetHeight() * 1.5f;
        GameObject branch = Instantiate(branchPrefab);
        Vector3 treePos = tree.GetGameObject().transform.position;
        branch.transform.position = new Vector3(treePos.x + (Random.Range(0,1)*2-1)*.75f, actualHeight-.5f, treePos.z + (Random.Range(0,1)*2-1)*.75f);
        branch.transform.LookAt(treePos);
    }
}
