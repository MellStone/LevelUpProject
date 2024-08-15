using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathManager : MonoBehaviour
{
    public GameObject pathPrefab; // Пустой префаб дороги
    public GameObject[] obstaclePrefabs; // Префабы препятствий
    public GameObject[] bonusPrefabs; // Префабы бонусов
    public int numberOfActivePaths = 5; // Количество активных участков дороги
    public float pathLength = 40f; // Длина каждого участка дороги
    public Transform playerTransform; // Трансформ игрока
    public float removalDelay = 2f; // Задержка перед удалением старых участков дороги

    private Queue<GameObject> activePaths = new Queue<GameObject>();
    private List<GameObject> activeObstaclesAndBonuses = new List<GameObject>(); // Список активных препятствий и бонусов
    private float spawnZ = 0f; // Позиция Z для спавна следующего участка дороги

    private void Start()
    {
        // Спавним начальные участки дороги
        for (int i = 0; i < numberOfActivePaths; i++)
        {
            SpawnPath();
        }
    }

    private void Update()
    {
        // Проверяем, нужно ли спавнить новый участок дороги
        if (playerTransform.position.z - 20 > (spawnZ - numberOfActivePaths * pathLength))
        {
            SpawnPath();
            RemoveOldPathAndObjects();
        }
    }

    private void SpawnPath()
    {
        // Спавним дорогу
        GameObject path = Instantiate(pathPrefab, transform);
        path.transform.position = Vector3.forward * spawnZ;
        activePaths.Enqueue(path);

        // Спавним препятствия и бонусы
        SpawnObstaclesAndBonuses();

        spawnZ += pathLength;
    }

    private void SpawnObstaclesAndBonuses()
    {
        int lanes = 3; // Количество линий

        // Проходим по каждой линии и спавним на ней объект
        for (int i = 0; i < lanes; i++)
        {
            float lanePosition = (i - 1) * 4f; // Расположение линий (-4, 0, 4)

            // Случайно выбираем, что спавнить на этой линии
            float spawnHeight = 0.25f;
            if (Random.value < 0.5f)
            {
                // Спавним препятствие
                GameObject obstacle = Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)]);
                obstacle.transform.position = new Vector3(lanePosition, spawnHeight, spawnZ + Random.Range(0f, pathLength));
                activeObstaclesAndBonuses.Add(obstacle);
            }
            else
            {
                // Спавним бонус
                GameObject bonus = Instantiate(bonusPrefabs[Random.Range(0, bonusPrefabs.Length)]);
                bonus.transform.position = new Vector3(lanePosition, spawnHeight * 2, spawnZ + Random.Range(0f, pathLength));
                activeObstaclesAndBonuses.Add(bonus);
            }
        }
    }

    private void RemoveOldPathAndObjects()
    {
        // Удаляем старые участки дороги
        if (activePaths.Count > 0)
        {
            GameObject oldPath = activePaths.Dequeue();
            if (oldPath != null)
            {
                Destroy(oldPath);
            }
        }

        // Удаляем препятствия и бонусы, которые находятся позади игрока
        activeObstaclesAndBonuses.RemoveAll(item =>
        {
            // Проверяем, что объект всё ещё существует, перед попыткой его удалить
            if (item == null)
            {
                return true;
            }

            // Удаляем объекты, которые находятся позади игрока
            if (item.transform.position.z < playerTransform.position.z - 10f)
            {
                Destroy(item);
                return true;
            }
            return false;
        });
    }
}
