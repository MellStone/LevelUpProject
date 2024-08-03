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
    public TextMeshProUGUI speedText; // UI Text for speed display
    public TextMeshProUGUI timeText; // UI Text for time display
    public TextMeshProUGUI coinText1;
    public TextMeshProUGUI coinText2;
    public TextMeshProUGUI countdownText; // UI Text for countdown display
    public Button startButton; // Button to start the game
    public CanvasGroup loseCanvas;
    public PlayerController player1;
    public PlayerController player2;
    public TextMeshProUGUI player1StatsText;
    public TextMeshProUGUI player2StatsText;
    public TextMeshProUGUI highScoreText;

    private float forwardSpeed;
    private float elapsedTime = 0f;
    private int coinCount1 = 0;
    private int coinCount2 = 0;
    private bool gameStarted = false;
    private GameData gameData;

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
        UpdateCoinText();
        highScoreText.text = "High Score: " + SaveManager.LoadHighScore();

        // Setup start button
        startButton.onClick.AddListener(StartGame);

        // Setup countdown display
        countdownText.gameObject.SetActive(false);
        speedText.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
        coinText1.gameObject.SetActive(false);
        coinText2.gameObject.SetActive(false);

        // Assign control keys for players
        player1.leftKey = KeyCode.A;
        player1.rightKey = KeyCode.D;
        player2.leftKey = KeyCode.LeftArrow;
        player2.rightKey = KeyCode.RightArrow;
    }

    private void Update()
    {
        if (gameStarted)
        {
            // Increase speed over time
            elapsedTime += Time.deltaTime;
            forwardSpeed = initialForwardSpeed + elapsedTime * speedIncreaseRate;

            // Update UI
            speedText.text = "Speed: " + forwardSpeed.ToString("F2") + " m/s";
            timeText.text = "Time: " + elapsedTime.ToString("F2") + " s";
        }
    }

    public void AddCoin(PlayerController player)
    {
        // Update player-specific stats
        if (player == player1)
        {
            coinCount1++;
            player1StatsText.text = $"Player 1 - Coins: {player1.GetCoinCount()} Time: {player1.GetElapsedTime():F2} s";
            UpdateCoinText();
        }
        else if (player == player2)
        {
            coinCount2++;
            player2StatsText.text = $"Player 2 - Coins: {player2.GetCoinCount()} Time: {player2.GetElapsedTime():F2} s";
            UpdateCoinText();
        }
    }

    private void UpdateCoinText()
    {
        coinText1.text = "Coins: " + coinCount1;
        coinText2.text = "Coins: " + coinCount2;
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
        speedText.gameObject.SetActive(true);
        timeText.gameObject.SetActive(true);
        coinText1.gameObject.SetActive(true);
        coinText2.gameObject.SetActive(true);

        gameStarted = true; // Start the game
        player1.StartGame(forwardSpeed);
        player2.StartGame(forwardSpeed);
    }

    public void EndGame()
    {
        loseCanvas.alpha = 1f;
        gameStarted = false;

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
