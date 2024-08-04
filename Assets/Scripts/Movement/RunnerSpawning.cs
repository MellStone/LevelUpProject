using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathManager : MonoBehaviour
{
    public GameObject[] pathPrefabs; // Array of prefabs for different path sections
    public GameObject[] initialSafePathPrefabs; // Array of safe prefabs for the initial path sections
    public int numberOfActivePaths = 2; // Number of path sections to be active at a time
    public float pathLength = 40f; // Length of each path section
    public Transform playerTransform; // Reference to the player's transform
    public float removalDelay = 2f; // Delay before removing old path sections

    private Queue<GameObject> activePaths = new Queue<GameObject>();
    private float spawnZ = 0f; // Z position to spawn the next path section
    private int initialSafePathsCount = 2; // Number of initial safe paths to spawn

    private void Start()
    {
        // Spawn initial safe paths
        for (int i = 0; i < initialSafePathsCount; i++)
        {
            SpawnInitialSafePath();
        }

        // Spawn remaining paths to meet the number of active paths
        for (int i = initialSafePathsCount; i < numberOfActivePaths; i++)
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

    private void SpawnInitialSafePath()
    {
        GameObject go = Instantiate(initialSafePathPrefabs[Random.Range(0, initialSafePathPrefabs.Length)], transform);
        go.transform.position = Vector3.forward * spawnZ;
        spawnZ += pathLength;
        activePaths.Enqueue(go);
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
