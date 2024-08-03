using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReadyPlayer2 : MonoBehaviour
{
    public float initialForwardSpeed = 10f;
    public float laneSwitchSpeed = 10f;
    public float speedIncreaseRate = 0.1f; // Speed increase per second
    public TextMeshProUGUI speedText; // UI Text for speed display
    public TextMeshProUGUI timeText; // UI Text for time display
    public TextMeshProUGUI coinText;

    public Transform coinPocket;
    public GameObject controllerSprite;

    private Rigidbody rb;
    private int currentLane = 1; // Start in the middle lane (0: far left, 1: left, 2: middle, 3: right, 4: far right)
    private float[] lanes = { -4f, 0f, 4f }; // Positions for lanes
    private float forwardSpeed;
    private float elapsedTime = 0f;
    private int coinCount = 0;
    private bool gameStarted = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        forwardSpeed = initialForwardSpeed;
        UpdateCoinText();

        speedText.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
        coinText.gameObject.SetActive(false);
    }

    public void HandleLaneSwitch(int direction)
    {
        ChangeLane(direction);
        // Additional logic or effects can be added here
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

            // Move the player forward
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, forwardSpeed);

            // Handle lane switching
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ChangeLane(-1);

                //controllerSprite.transform.DOShakePosition(0.1f, 1, 10);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ChangeLane(1);

                //controllerSprite.transform.DOShakePosition(0.1f, 1, 10);
            }



            // Smoothly move the player to the target lane position
            Vector3 targetPosition = new Vector3(lanes[currentLane], transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * laneSwitchSpeed);
        }
    }

    private void ChangeLane(int direction)
    {
        currentLane = Mathf.Clamp(currentLane + direction, 0, lanes.Length - 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            coinCount++;
            //soundSource.Play();
            UpdateCoinText();
            other.gameObject.transform.DOMove(coinPocket.transform.position, 1f).OnComplete(() =>
            {
                Destroy(other.gameObject);
            });
        }
        if (other.gameObject.CompareTag("Blocker"))
        {
            Debug.Log("Game End!");
            //loseCanvas.alpha = 1f;

        }
    }

    private void UpdateCoinText()
    {
        coinText.text = "Coins: " + coinCount;
    }

    public void StartGame()
    {
        // Start countdown and game
        //startButton.gameObject.SetActive(false); // Hide the start button
        //StartCoroutine(CountdownCoroutine());
        //cameraHook.transform.DOLocalMoveY(-70, 3f);
        gameStarted = true;

        speedText.gameObject.SetActive(true);
        timeText.gameObject.SetActive(true);
        coinText.gameObject.SetActive(true);

    }

    //private IEnumerator CountdownCoroutine()
    //{
    //    countdownText.gameObject.SetActive(true); // Show countdown text

    //    for (int i = 3; i > 0; i--)
    //    {
    //        countdownText.text = i.ToString();
    //        yield return new WaitForSeconds(1f);
    //    }

    //    countdownText.text = "Go!";
    //    yield return new WaitForSeconds(1f);

    //    countdownText.gameObject.SetActive(false); // Hide countdown text
    //    speedText.gameObject.SetActive(true);
    //    timeText.gameObject.SetActive(true);
    //    coinText.gameObject.SetActive(true);

    //    gameStarted = true; // Start the game
    //}
}