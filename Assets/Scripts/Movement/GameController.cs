using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public float initialForwardSpeed = 10f;
    public float speedIncreaseRate = 0.1f; // Speed increase per second
    public TextMeshProUGUI countdownText; // UI Text for countdown display
    public Button startButton; // Button to start the game
    public CanvasGroup loseCanvas;
    public PlayerController player1;
    public PlayerController player2;
    public TextMeshProUGUI player1StatsText;
    public TextMeshProUGUI player2StatsText;
    public TextMeshProUGUI highScoreText;

    private float forwardSpeed;
    //private float elapsedTime = 0f;
    //private bool gameStarted = false;
    private GameData gameData;
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
        
    }

    private void StartGame()
    {
        // Start countdown and game
        startButton.gameObject.SetActive(false); // Hide the start button
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
        loseCanvas.alpha = 1f;
        //gameStarted = false;

        // Display final stats for each player
        player1StatsText.text = $"Player 1 - Final Coins: {player1.GetCoinCount()} Final Time: {player1.GetElapsedTime():F2} s";
        player2StatsText.text = $"Player 2 - Final Coins: {player2.GetCoinCount()} Final Time: {player2.GetElapsedTime():F2} s";

        // Save scores
        SaveScores();
        
    }

    private void SaveScores()
    {
        gameData.players.Clear();

        gameData.players.Add(new PlayerData { playerName = "Player 1", score = player1.GetCoinCount() });
        gameData.players.Add(new PlayerData { playerName = "Player 2", score = player2.GetCoinCount() });

        SaveManager.SaveGame(gameData);

        int highScore = SaveManager.LoadHighScore();
        int currentScore1 = Mathf.CeilToInt(player1.GetCoinCount() * 200 + player1.transform.position.z * 1.5f); 
        int currentScore2 = Mathf.CeilToInt(player1.GetCoinCount() * 200 + player1.transform.position.z * 1.5f);
        
        int currentHighScore = Mathf.Max(currentScore1, currentScore2);
        
        if (currentHighScore > highScore)
        {
            SaveManager.SaveHighScore(currentHighScore);
            highScoreText.text = "High Score: " + currentHighScore;
        }
    }
}
