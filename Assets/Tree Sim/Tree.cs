using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree
{
    // Variables to store tree properties
    private float height;
    private int maxSplits;
    private List<GameObject> branches = new List<GameObject>();
    private List<GameObject> leaves = new List<GameObject>();
    private GameObject gameObject;

    // Constructor to initialize a Tree object
    public Tree(int maxSplits, GameObject gameObject) {
        height = gameObject.transform.localScale.y;
        this.maxSplits = maxSplits;
        this.gameObject = gameObject;
    }

    // Getter methods for retrieving tree properties
    public float GetHeight() {
        return height;
    }
    public void AddHeight(float height) {
        gameObject.transform.localScale += new Vector3(0, height, 0);
        gameObject.transform.position += new Vector3(0, height/2, 0);
        this.height = gameObject.transform.localScale.y;
    }
    public List<GameObject> GetBranches() {
        return branches;
    }
    public List<GameObject> GetLeaves() {
        return leaves;
    }
    public int GetMaxSplits() {
        return maxSplits;
    }
    public GameObject GetGameObject() {
        return gameObject;
    }
}

