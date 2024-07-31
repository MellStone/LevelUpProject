using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathManager : MonoBehaviour
{
    public GameObject[] pathPrefabs; // Array of prefabs for different path sections
    public int numberOfActivePaths = 2; // Number of path sections to be active at a time
    public float pathLength = 40f; // Length of each path section
    public Transform playerTransform; // Reference to the player's transform
    public float removalDelay = 2f; // Delay before removing old path sections

    private Queue<GameObject> activePaths = new Queue<GameObject>();
    private float spawnZ = 0f; // Z position to spawn the next path section

    private void Start()
    {
        for (int i = 0; i < numberOfActivePaths; i++)
        {
            SpawnPath();
        }
    }

    private void Update()
    {
        // Check if a new path section needs to be spawned
        if (playerTransform.position.z - 20 > (spawnZ - numberOfActivePaths * pathLength))
        {
            SpawnPath();
            StartCoroutine(RemoveOldPath());
        }
    }

    private void SpawnPath()
    {
        GameObject go = Instantiate(pathPrefabs[Random.Range(0, pathPrefabs.Length)], transform);
        go.transform.position = Vector3.forward * spawnZ;
        spawnZ += pathLength;
        activePaths.Enqueue(go);
    }

    private IEnumerator RemoveOldPath()
    {
        yield return new WaitForSeconds(removalDelay);

        if (activePaths.Count > 0)
        {
            GameObject oldPath = activePaths.Dequeue();
            Destroy(oldPath);
        }
    }
}
