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
    public float speedIncreaseRate = 0.1f;
    public TextMeshProUGUI countdownText;
    public Button startButton;
    public Button restartButton;
    public GameObject ipText;
    public GameObject guideNameText;
    public CanvasGroup loseCanvas;
    public CameraFollow _cameraFollow;
    public PlayerController player1;
    public PlayerController player2;
    
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI leaderboardText;

    // Новые поля для ввода имени игроков
    public GameObject playerNamesMenu;
    public TMP_InputField player1NameInput;
    public TMP_InputField player2NameInput;

    private string player1Name = "Player 1";
    private string player2Name = "Player 2";

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
        HidePlayerNames(true);
        forwardSpeed = initialForwardSpeed;
        highScoreText.text = "High Score: " + SaveManager.LoadHighScore();

        // Подписываем кнопку старта на метод StartGame
        startButton.onClick.AddListener(StartGame);

        countdownText.gameObject.SetActive(false);
        loseCanvas.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        // Получаем имена игроков из InputField
        player1Name = string.IsNullOrEmpty(player1NameInput.text) ? "Player 1" : player1NameInput.text;
        player2Name = string.IsNullOrEmpty(player2NameInput.text) ? "Player 2" : player2NameInput.text;

        // Скрываем элементы интерфейса, которые не нужны во время игры
        startButton.gameObject.SetActive(false);
        ipText.gameObject.SetActive(false);
        guideNameText.gameObject.SetActive(false);
        HidePlayerNames(true);
        
        isInMenu = false;
        _cameraFollow.CameraIntro();
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "Go!";
        countdownText.transform.DOScale(3f, 1f);
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);

        player1.StartGame(forwardSpeed);
        player2.StartGame(forwardSpeed);
    }

    public void GameOver()
    {
        SaveScores();

        if (player2.isActiveAndEnabled)
        {
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
            loseCanvas.gameObject.SetActive(true);
            restartButton.gameObject.SetActive(true);
            DisplayTop10();
            isInMenu = true;
            isGameOver = true;
        }
    }

    public void HidePlayerNames(bool state)
    {
        if(isInMenu)
            playerNamesMenu.gameObject.SetActive(!state);
    }
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void ResetHighScore()
    {
        gameData.players.Clear();
    }
    private void DisplayTop10()
    {
        List<PlayerData> players = gameData.players;
        players.Sort((p1, p2) => p2.score.CompareTo(p1.score));
        List<PlayerData> top10 = players.GetRange(0, Mathf.Min(10, players.Count));

        string leaderboard = "Top 10 Players:\n";
        for (int i = 0; i < top10.Count; i++)
        {
            leaderboard += $"{i + 1}. {top10[i].playerName} - {top10[i].score}\n";
        }

        leaderboardText.text = leaderboard;
    }

    private void SaveScores()
    {
        gameData.players.Add(new PlayerData { playerName = player1Name, score = Mathf.CeilToInt(player1.GetCoinCount() * 200 + player1.transform.position.z * 1.5f) });
        gameData.players.Add(new PlayerData { playerName = player2Name, score = Mathf.CeilToInt(player2.GetCoinCount() * 200 + player2.transform.position.z * 1.5f) });

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
