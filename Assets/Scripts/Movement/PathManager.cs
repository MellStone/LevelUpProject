    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class PathManager : MonoBehaviour
    {
        public GameObject pathPrefab; // Пустой префаб дороги
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
            GameObject go = Instantiate(pathPrefab, transform);
            go.transform.position = Vector3.forward * spawnZ;

            // Спавн препятствий и бонусов на этом участке пути
            SpawnObstaclesAndBonuses(spawnZ);

            spawnZ += pathLength;
            activePaths.Enqueue(go);
        }


        private void SpawnObstaclesAndBonuses(float zPos)
        {
            List<int> availableLines = new List<int> { -4, 0, 4 }; // Доступные линии: -2, 0, 2

            // Спавн препятствий
            int obstacleCount = Random.Range(1, 3); // Генерация 1 или 2 препятствий
            for (int i = 0; i < obstacleCount; i++)
            {
                if (availableLines.Count > 0)
                {
                    int randomIndex = Random.Range(0, availableLines.Count);
                    int line = availableLines[randomIndex];
                    availableLines.RemoveAt(randomIndex); // Удаляем линию из доступных, чтобы избежать спавна на всех трех линиях

                    GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
                    GameObject obstacle = Instantiate(obstaclePrefab, new Vector3(line, 0.2f, zPos), Quaternion.Euler(0, 90, 0));
                    activeObstaclesAndBonuses.Add(obstacle);
                }
            }

            // Спавн бонусов (если остались доступные линии)
            if (availableLines.Count > 0 && Random.value < 0.5f) // 50% шанс на спавн бонуса
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
