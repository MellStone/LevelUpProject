using System.Collections.Generic;
using UnityEngine;

public class RandomSequenceGenerator : MonoBehaviour
{
    public int sequenceLength = 5; // Длина генерируемой последовательности
    public float timeBetweenSteps = 0.5f; // Время между шагами (можно использовать для анимации или подсказок)

    private List<int> sequence = new List<int>(); // Список для хранения сгенерированной последовательности
    private int currentStep = 0; // Текущий шаг игрока в последовательности

    void Start()
    {
        GenerateSequence();
        StartCoroutine(DisplaySequence());
    }

    void GenerateSequence()
    {
        sequence.Clear();

        for (int i = 0; i < sequenceLength; i++)
        {
            // Генерация случайного KeyCode (например, A, S, D, W)
            int randomKey = GetRandomKey();
            sequence.Add(randomKey);
        }
    }

    int GetRandomKey()
    {
        // Пример для кнопок A, S, D, W
        
        int[] possibleKeys = { -1, 1 };
        int randomIndex = Random.Range(0, possibleKeys.Length);
        return possibleKeys[randomIndex];
    }

    System.Collections.IEnumerator DisplaySequence()
    {
        foreach (KeyCode key in sequence)
        {
            // Здесь можно реализовать логику визуальной индикации последовательности (например, подсветка UI)
            Debug.Log("Press: " + key);

            // Подсказка, ожидание между шагами
            yield return new WaitForSeconds(timeBetweenSteps);
        }

        // После отображения последовательности, игрок должен её повторить
        currentStep = 0;
    }
    

    public void PlaySequence(int direction)
    {
        // Проверка ввода игрока
        if (currentStep < sequence.Count)
        {
            if (direction == sequence[currentStep])
            {
                // Если игрок нажал правильную кнопку
                currentStep++;

                if (currentStep >= sequence.Count)
                {
                    // Игрок правильно ввел всю последовательность
                    Debug.Log("Sequence completed successfully!");
                    // Можно начать новую последовательность или наградить игрока
                    StartNewRound();
                }
            }
            else if (Input.anyKeyDown)
            {
                // Если игрок нажал неправильную кнопку
                Debug.Log("Wrong button! Start over.");
                // Можно сбросить последовательность или уменьшить здоровье игрока
                StartNewRound();
            }
        }
    }

    void StartNewRound()
    {
        // Запуск новой последовательности
        GenerateSequence();
        StartCoroutine(DisplaySequence());
    }
}
