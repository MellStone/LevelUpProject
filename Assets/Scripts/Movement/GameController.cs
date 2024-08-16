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
    public GameObject startButton2;
    public Button restartButton;
    public GameObject ipText;
    public GameObject guideNameText;
    public CanvasGroup loseCanvas;
    public CameraFollow _cameraFollow;
    public PlayerController player1;
    public PlayerController player2;
    public SpriteRenderer expIMG;
    
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI leaderboardText;

    // Поля для ввода имени игроков
    public GameObject playerNamesMenu;
    public TMP_InputField player1NameInput;
    public TMP_InputField player2NameInput;
    public Button submitNamesButton; // Новая кнопка для подтверждения имени

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
        startButton2.GetComponent<Button>().onClick.AddListener(StartGame);

        countdownText.gameObject.SetActive(false);
        loseCanvas.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);

        player1.enabled = false;
        player2.enabled = false;

        // Подписываем кнопку подтверждения имен на метод SubmitNames
        submitNamesButton.onClick.AddListener(SubmitNames);
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

        startButton2.gameObject.SetActive(false);
        
        
        player1.enabled = true;
        player2.enabled = true;
        
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
        // Скрываем кнопки рестарта и проигрыша, показываем меню ввода имен игроков
        loseCanvas.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        expIMG.gameObject.SetActive(true);

        HidePlayerNames(false);

        // Ожидаем, пока игроки введут свои имена и нажмут кнопку подтверждения
    }

    public void SubmitNames()
    {
        // Получаем имена игроков из InputField
        player1Name = string.IsNullOrEmpty(player1NameInput.text) ? "Player 1" : player1NameInput.text;
        player2Name = string.IsNullOrEmpty(player2NameInput.text) ? "Player 2" : player2NameInput.text;

        // Сохраняем результаты и показываем лидерборд
        SaveScores();
        DisplayTop10();

        // Скрываем меню ввода имен
        HidePlayerNames(true);

        // Показываем интерфейс проигрыша и кнопку рестарта
        loseCanvas.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);

        isInMenu = true;
        isGameOver = true;
    }

    public void HidePlayerNames(bool state)
    {
        playerNamesMenu.gameObject.SetActive(!state);
        submitNamesButton.gameObject.SetActive(!state);
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
        if (isMultiplayer)
        {
            gameData.players.Add(new PlayerData { playerName = player1Name, score = player1.GetScore()});
            gameData.players.Add(new PlayerData { playerName = player2Name, score = player2.GetScore()});
        }
        else
        {
            gameData.players.Add(new PlayerData { playerName = player1Name, score = player1.GetScore()});
        }

        SaveManager.SaveGame(gameData);

        int highScore = SaveManager.LoadHighScore();
        int currentScore1 = player1.GetScore();
        int currentScore2 = player2.GetScore();

        int currentHighScore = Mathf.Max(currentScore1, currentScore2);

        if (currentHighScore > highScore)
        {
            SaveManager.SaveHighScore(currentHighScore);
            highScoreText.text = "High Score: " + currentHighScore;
        }
    }
}
