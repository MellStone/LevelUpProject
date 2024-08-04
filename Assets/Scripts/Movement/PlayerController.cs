using DG.Tweening;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float laneSwitchSpeed = 10f;
    public TextMeshProUGUI speedText; // UI Text for speed display
    public TextMeshProUGUI timeText; // UI Text for time display
    public TextMeshProUGUI coinText;
    public Transform coinPocket;
    public AudioSource soundSource;
    public KeyCode leftKey = KeyCode.None;
    public KeyCode rightKey = KeyCode.None;

    private Rigidbody rb;
    private int currentLane = 1; // Middle start (left = 0, center = 1, right = 2) 
    private float[] lanes = { -4f, 0f, 4f }; // Positions for lanes
    private float forwardSpeed;
    private float elapsedTime = 0f;
    private int coinCount = 0;
    private bool gameStarted = false;
    private bool shouldStop = false;

    public GameObject controllerSprite;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        speedText.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
        coinText.gameObject.SetActive(false);
    }

    public void StartGame(float initialSpeed)
    {
        forwardSpeed = initialSpeed;
        gameStarted = true;

        speedText.gameObject.SetActive(true);
        timeText.gameObject.SetActive(true);
        coinText.gameObject.SetActive(true);
    }

    public void EndGame()
    {
        gameStarted = false;
    }

    private void Update()
    {
        if (gameStarted)
        {
            // Increase speed over time
            elapsedTime += Time.deltaTime;
            forwardSpeed += Time.deltaTime * GameController.Instance.speedIncreaseRate;

            // Update UI
            speedText.text = "Speed: " + forwardSpeed.ToString("F2") + " m/s";
            timeText.text = "Time: " + elapsedTime.ToString("F2") + " s";

            // Move the player forward
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, forwardSpeed);

            // Handle local lane switching
            if (Input.GetKeyDown(leftKey))
            {
                HandleLaneSwitch(-1);

                controllerSprite.transform.DORotate(new Vector3(90f, -45f, 0f), 0.2f)
                    .OnComplete(() => 
                    {
                        controllerSprite.transform.DORotate(new Vector3(90f, 0f, 0f), 0.2f);
                    });
                //controllerSprite.transform.DOShakePosition(0.1f, 1, 10);
            }
            else if (Input.GetKeyDown(rightKey))
            {
                HandleLaneSwitch(1);

                controllerSprite.transform.DORotate(new Vector3(90f, 45f, 0f), 0.2f)
                    .OnComplete(() =>
                    {
                        controllerSprite.transform.DORotate(new Vector3(90f, 0f, 0f), 0.2f);
                    });
                //controllerSprite.transform.DOShakePosition(0.1f, 1, 10);
            }

            // Smoothly move the player to the target lane position
            Vector3 targetPosition = new Vector3(lanes[currentLane], transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * laneSwitchSpeed);
        }
    }

    public void HandleLaneSwitch(int direction)
    {
        currentLane = Mathf.Clamp(currentLane + direction, 0, lanes.Length - 1);
        // Add visual or audio feedback for lane switching if necessary
        switch (currentLane)
        {
            case 0:
                Debug.Log("Red");

                break;

            case 1:
                Debug.Log("Yellow");


                break;

            case 2:
                Debug.Log("Blue");


                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            coinCount++;
            GameController.Instance.AddCoin(this);
            soundSource.Play();
            other.gameObject.transform.DOMove(coinPocket.transform.position, 1f).OnComplete(() =>
            {
                Destroy(other.gameObject);
            });
        }
        if (other.gameObject.CompareTag("Blocker"))
        {
            GameController.Instance.EndGame();
        }
    }

    public int GetCoinCount()
    {
        return coinCount;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}
