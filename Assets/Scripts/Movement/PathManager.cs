    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class PathManager : MonoBehaviour
{
    public GameObject pathPrefab; // Пустой префаб дороги
    public GameObject whiteLinePrefab; // Префаб белой полосы
    public GameObject whitePointPrefab; // Префаб белой точки
    public GameObject[] obstaclePrefabs; // Префабы препятствий
    public GameObject[] bonusPrefabs; // Префабы бонусов
    public int numberOfActivePaths = 5; // Количество активных участков дороги
    public float pathLength = 35f; // Длина каждого участка дороги
    public Transform playerTransform; // Трансформ игрока
    public float removalDelay = 2f; // Задержка перед удалением старых участков дороги

    private Queue<GameObject> activePaths = new Queue<GameObject>();
    private List<GameObject> activeObstaclesAndBonuses = new List<GameObject>(); // Список активных препятствий и бонусов
    private float spawnZ = 40f; // Позиция Z для спавна следующего участка дороги

    private void Start()
    {
        for (int i = 0; i < numberOfActivePaths; i++)
        {
            SpawnPath();
        }
    }

    private void Update()
    {
        if (playerTransform.position.z - 20 > (spawnZ - numberOfActivePaths * pathLength))
        {
            SpawnPath();
            RemoveOldPathAndObjects();
        }
    }

    private void SpawnPath()
    {
        GameObject go = Instantiate(pathPrefab, transform);
        go.transform.position = Vector3.forward * spawnZ;

        SpawnObstaclesAndBonuses(spawnZ);

        // Добавляем спавн белой полосы и точки
        if (ShouldSpawnWhiteLine())
        {
            SpawnWhiteLineAndPoint(spawnZ + pathLength / 2);
        }

        spawnZ += pathLength;
        activePaths.Enqueue(go);
    }

    private bool ShouldSpawnWhiteLine()
    {
        // Условие для спавна белой полосы
        return Random.value < 0.1f; // 20% шанс на спавн
    }

    private void SpawnWhiteLineAndPoint(float zPos)
    {
        GameObject whiteLine = Instantiate(whiteLinePrefab, new Vector3(0, 0.1f, zPos), Quaternion.identity);
        activeObstaclesAndBonuses.Add(whiteLine);
    }

    private void SpawnObstaclesAndBonuses(float zPos)
    {
        List<int> availableLines = new List<int> { -4, 0, 4 }; // Доступные линии: -2, 0, 2

        int obstacleCount = Random.Range(1, 3); // Генерация 1 или 2 препятствий
        for (int i = 0; i < obstacleCount; i++)
        {
            if (availableLines.Count > 0)
            {
                int randomIndex = Random.Range(0, availableLines.Count);
                int line = availableLines[randomIndex];
                availableLines.RemoveAt(randomIndex);

                GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
                GameObject obstacle = Instantiate(obstaclePrefab, new Vector3(line, 0.2f, zPos), Quaternion.Euler(0, 90, 0));
                activeObstaclesAndBonuses.Add(obstacle);
            }
        }

        // Спавн бонусов (если остались доступные линии)
        if (availableLines.Count > 0 && Random.value < 0.2f) // 50% шанс на спавн бонуса
        {
            int randomIndex = Random.Range(0, availableLines.Count);
            int line = availableLines[randomIndex];

            GameObject bonusPrefab = bonusPrefabs[Random.Range(0, bonusPrefabs.Length)];
            GameObject bonus = Instantiate(bonusPrefab, new Vector3(line, 0.5f, zPos), Quaternion.identity);
            activeObstaclesAndBonuses.Add(bonus);
        }
    }

    private void RemoveOldPathAndObjects()
    {
        if (activePaths.Count > 0)
        {
            GameObject oldPath = activePaths.Dequeue();
            if (oldPath != null)
            {
                Destroy(oldPath);
            }
        }

        activeObstaclesAndBonuses.RemoveAll(item =>
        {
            if (item == null)
            {
                return true;
            }

            if (item.transform.position.z < playerTransform.position.z - 10f)
            {
                Destroy(item);
                return true;
            }
            return false;
        });
    }
}

