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

    public int hP = 100;
    
    private bool isGameStarted = false;


    private int turnDirection = 0;
    public GameObject controllerSprite;
    private GameController gameManager;
    private RandomSequenceGenerator sequence;

    private void Start()
    {
        sequence = gameObject.AddComponent<RandomSequenceGenerator>();
        rb = GetComponent<Rigidbody>();
        speedText.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
        coinText.gameObject.SetActive(false);
    }

    public void StartGame(float initialSpeed)
    {
        forwardSpeed = initialSpeed;
        isGameStarted = true;
        
        speedText.gameObject.SetActive(true);
        timeText.gameObject.SetActive(true);
        coinText.gameObject.SetActive(true);
        coinText.text = "Coins: " + 0;
    }

    public void EndGame()
    {
        isGameStarted = false;
    }

    private void Update()
    {
        if (!isGameStarted) return;
        
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
        }
        else if (Input.GetKeyDown(rightKey))
        {
            HandleLaneSwitch(1);
        }

        if (turnDirection != 0)
        {
            LaneSwitchAnimate(turnDirection);
            turnDirection = 0;
        }
        // Smoothly move the player to the target lane position
        Vector3 targetPosition = new Vector3(lanes[currentLane], transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * laneSwitchSpeed);
    }

    private void LaneSwitchAnimate(int direction)
    {
        if (direction == -1)
        {
            controllerSprite.transform.DORotate(new Vector3(90f, -45f, 0f), 0.2f).OnComplete(() =>
            {
                controllerSprite.transform.DORotate(new Vector3(90f, 0f, 0f), 0.2f);
            });
            Debug.Log("Animating: " + direction);
        }
        else if (direction == 1)
        {
            controllerSprite.transform.DORotate(new Vector3(90f, 45f, 0f), 0.2f).OnComplete(() =>
            {
                controllerSprite.transform.DORotate(new Vector3(90f, 0f, 0f), 0.2f);
            });
            Debug.Log("Animating: "+ direction);
        }   
        else
        {
            Debug.Log("It doesnt work :(");
            Debug.Log("Current side: " + direction);
        }
    }
    
    public void HandleLaneSwitch(int direction)
    {
        currentLane = Mathf.Clamp(currentLane + direction, 0, lanes.Length - 1);
        turnDirection = direction;
    }

    public void HandleSpecialSwitch(int direction) // -1 left, 0 mid, 1 right;
    {
        
        sequence.PlaySequence(direction);
        
        switch (direction)
        {
            case -1:
                Debug.Log("Special button left");
                break;
            case 0:
                Debug.Log("Special button mid");
                break;
            case 1:
                Debug.Log("Special button right");
                break;
            default:
                Debug.LogError("Incorrect value for HandleSpecialSwitch");
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            coinCount++;
            coinText.text = "Coins: " + coinCount;
            soundSource.Play();
            other.gameObject.transform.DOMove(coinPocket.transform.position, 1f).OnComplete(() =>
            {
                Destroy(other.gameObject);
            });
        }
        if (other.gameObject.CompareTag("Blocker"))
        {
            if (GameController.Instance.isMultiplayer)
            {
                
            }
            else
            {
                GameController.Instance.GameOver();
            }
        }
    }

    public void GetDamage(int damage)
    {
        hP -= damage;
        if (hP <= 0)
            EndGame();
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
