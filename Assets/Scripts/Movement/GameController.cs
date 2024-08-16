using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public float initialForwardSpeed = 10f;
    public float speedIncreaseRate = 0.1f; // Speed increase per second
    public TextMeshProUGUI countdownText; // UI Text for countdown display
    public Button startButton; // Button to start the game
    public Button restartButton; // Button to restart the game
    public CanvasGroup loseCanvas;
    public PlayerController player1;
    public PlayerController player2;
    
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI leaderboardText; // Text UI для отображения таблицы лидеров
    
    private float forwardSpeed;
    
    private GameData gameData;

    public bool isInMenu = true;
    public bool isGameOver = false;
    public bool isMultiplayer = true;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            gameData = SaveManager.LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        forwardSpeed = initialForwardSpeed;
        highScoreText.text = "High Score: " + SaveManager.LoadHighScore();

        // Setup start button
        startButton.onClick.AddListener(StartGame);
        
        
        // Setup countdown display
        countdownText.gameObject.SetActive(false);
        loseCanvas.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        Debug.Log("GameStarted");
        // Start countdown and game
        startButton.gameObject.SetActive(false); // Hide the start button
        Debug.Log("GameStarted");
        isInMenu = false;
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        countdownText.gameObject.SetActive(true); // Show countdown text

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "Go!";
        countdownText.transform.DOScale(3f, 1f);
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false); // Hide countdown text

        //gameStarted = true; // Start the game
        player1.StartGame(forwardSpeed);
        player2.StartGame(forwardSpeed);
    }

    public void GameOver()
    {
        // Save scores
        SaveScores();

        // Проверка активности второго игрока
        if (player2.isActiveAndEnabled)
        {
            // Если второй игрок активен, проверяем, завершили ли игру оба игрока
            if (player2.isGameOvered && player1.isGameOvered)
            {
                loseCanvas.gameObject.SetActive(true);
                restartButton.gameObject.SetActive(true);
                DisplayTop10();
                isInMenu = true;
                isGameOver = true;
            }
        }
        else
        {
            // Если второй игрок не активен, сразу показываем таблицу лидеров
            loseCanvas.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(true);
            DisplayTop10();
            isInMenu = true;
            isGameOver = true;
        }
    }
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    
    private void DisplayTop10()
    {
        // Загружаем список игроков из сохранённых данных
        List<PlayerData> players = gameData.players;

        // Сортируем по очкам в порядке убывания
        players.Sort((p1, p2) => p2.score.CompareTo(p1.score));

        // Берём топ-10
        List<PlayerData> top10 = players.GetRange(0, Mathf.Min(10, players.Count));

        // Формируем строку для отображения таблицы лидеров
        string leaderboard = "Top 10 Players:\n";
        for (int i = 0; i < top10.Count; i++)
        {
            leaderboard += $"{i + 1}. {top10[i].playerName} - {top10[i].score}\n";
        }

        // Отображаем на UI
        leaderboardText.text = leaderboard;
    }
    private void SaveScores()
    {
        gameData.players.Clear();

        gameData.players.Add(new PlayerData { playerName = "Player 1", score = Mathf.CeilToInt(player1.GetCoinCount() * 200 + player1.transform.position.z * 1.5f) });
        gameData.players.Add(new PlayerData { playerName = "Player 2", score = Mathf.CeilToInt(player2.GetCoinCount() * 200 + player2.transform.position.z * 1.5f) });

        SaveManager.SaveGame(gameData);

        int highScore = SaveManager.LoadHighScore();
        int currentScore1 = Mathf.CeilToInt(player1.GetCoinCount() * 200 + player1.transform.position.z * 1.5f); 
        int currentScore2 = Mathf.CeilToInt(player2.GetCoinCount() * 200 + player2.transform.position.z * 1.5f);
        
        int currentHighScore = Mathf.Max(currentScore1, currentScore2);
        
        if (currentHighScore > highScore)
        {
            SaveManager.SaveHighScore(currentHighScore);
            highScoreText.text = "High Score: " + currentHighScore;
        }
    }
}
