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
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI countdownText; // UI Text for countdown display
    public Button startButton; // Button to start the game
    public CanvasGroup loseCanvas;
    public PlayerController player1;
    public PlayerController player2;

    private float forwardSpeed;
    private float elapsedTime = 0f;
    private int coinCount = 0;
    private bool gameStarted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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

        // Setup start button
        startButton.onClick.AddListener(StartGame);

        // Setup countdown display
        countdownText.gameObject.SetActive(false);
        speedText.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
        coinText.gameObject.SetActive(false);

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

    public void AddCoin()
    {
        coinCount++;
        UpdateCoinText();
    }

    private void UpdateCoinText()
    {
        coinText.text = "Coins: " + coinCount;
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
        coinText.gameObject.SetActive(true);

        gameStarted = true; // Start the game
        player1.StartGame(forwardSpeed);
        player2.StartGame(forwardSpeed);
    }

    public void EndGame()
    {
        loseCanvas.alpha = 1f;
        gameStarted = false;
    }
}
