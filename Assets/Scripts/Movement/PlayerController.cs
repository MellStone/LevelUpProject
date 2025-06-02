using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float laneSwitchSpeed = 10f;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI coinText;
    public GameObject bonusIcon;
    
    public Transform coinPocket;
    public AudioSource soundSource;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode specialUpKey = KeyCode.W;
    public KeyCode specialDownKey = KeyCode.S;
    public KeyCode leftKeyAdditonal = KeyCode.LeftArrow;
    public KeyCode rightKeyAdditonal = KeyCode.RightArrow;
    public KeyCode specialUpKeyAdditonal = KeyCode.UpArrow;
    public KeyCode specialDownKeyAdditonal = KeyCode.DownArrow;
    
    private Rigidbody rb;
    private int currentLane = 1; // Middle start (left = 0, center = 1, right = 2) 
    private float[] lanes = { -4f, 0f, 4f }; // Positions for lanes
    private float forwardSpeed;
    private float previousSpeed;
    private float baseSpeed; // Базовая скорость, которая не изменяется
    private float currentSpeedModifier = 1f; // Текущий модификатор скорости
    private float targetSpeedModifier = 1f;
    public float transitionDuration = 0.4f;
    private bool isOnWhiteLine = false;
    
    [SerializeField]
    private float increseSpeed = 1.3f;
    [SerializeField]
    private float decreaseSpeed = 0.4f;
    private float bonusDuration = 3f;
    
    private float elapsedTime = 0f;
    private int coinCount = 0;
    private int score = 0;
    public bool hasBonus = false;
    
    public int hP = 100;

    private bool isShouldStop = false;
    public bool isGameOvered = false;

    private int turnDirection = 0;
    public GameObject controllerSprite;
    private GameController gameManager;
    private RandomSequenceGenerator sequence;

    private static int[] playersOnLane = { 0, 0, 0 }; // Count of players on each lane
    private static Dictionary<int, List<GameObject>> playersOnEachLane = new Dictionary<int, List<GameObject>>()
    {
        { 0, new List<GameObject>() },
        { 1, new List<GameObject>() },
        { 2, new List<GameObject>() }
    };
    
    
    private bool moveLeftRepeat = false;
    private bool moveRightRepeat = false;
    private bool specialLeftRepeat = false;
    private bool specialRightRepeat = false;
    private bool specialMidRepeat = false;
    
    
    private void Start()
    {
        sequence = gameObject.AddComponent<RandomSequenceGenerator>();
        rb = GetComponent<Rigidbody>();
        baseSpeed = forwardSpeed;
        scoreText.gameObject.SetActive(false);
        speedText.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
        coinText.gameObject.SetActive(false);
        playersOnEachLane[currentLane].Add(gameObject);
    }

    public void StartGame(float initialSpeed)
    {
        baseSpeed = initialSpeed; // Сохраняем исходную скорость в момент старта игры
        forwardSpeed = baseSpeed;
        currentSpeedModifier = 1f; // Сбрасываем все модификаторы скорости
        forwardSpeed = initialSpeed;
        isShouldStop = false;
        isGameOvered = false;
        elapsedTime = 0f;
        coinCount = 0;
        hP = 100;
        targetSpeedModifier = 1f;
        
        scoreText.gameObject.SetActive(true);
        speedText.gameObject.SetActive(true);
        timeText.gameObject.SetActive(true);
        coinText.gameObject.SetActive(true);
        coinText.text = "Coins: " + coinCount;
        
        controllerSprite.transform.DOShakePosition(0.2f, 0.08f, 10)
            .SetLoops(-1, LoopType.Restart);
    }

    public int GetScore()
    {
        return score = Mathf.CeilToInt(GetCoinCount() * 200 + gameObject.transform.position.z * 1.5f);
    }
    public void EndGame()
    {
        isShouldStop = true;
        isGameOvered = true;
        rb.linearVelocity = Vector3.zero;
        playersOnLane[currentLane]--; // Уменьшение счетчика игроков на текущей линии
    }

    private void Update()
    {
        if (isShouldStop) return;
        
        elapsedTime += Time.deltaTime;
        baseSpeed += Time.deltaTime * GameController.Instance.speedIncreaseRate; // Обновляем базовую скорость
        
        forwardSpeed = baseSpeed * currentSpeedModifier;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, forwardSpeed);
       
        TurnLineCheck();
        KeyInput();
        WebSocketCheck();
        TurnLineCheck();
        UpdatePersonalUI();
        
        Vector3 targetPosition = CalculateTargetPosition(currentLane);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * laneSwitchSpeed);
    }

    private void TurnLineCheck()
    {
        if (turnDirection != 0)
        {
            LaneSwitchAnimate(turnDirection);
            turnDirection = 0;
        }
    }

    private void UpdatePersonalUI()
    {
        scoreText.text = "Score: " + GetScore();
        speedText.text = "Speed: " + forwardSpeed.ToString("F1") + " m/s";
        timeText.text = "Time: " + elapsedTime.ToString("F2") + " s";
    }

    private void UpdateBonusUI()
    {
        if (hasBonus)
        {
            bonusIcon.SetActive(true);
        }
        else
        {
            bonusIcon.SetActive(false);
        }
        
    }

    private void KeyInput()
    {
        if (Input.GetKeyDown(leftKey) || Input.GetKeyDown(leftKeyAdditonal))
        {
            HandleLaneSwitch(-1);
        }
        else if (Input.GetKeyDown(rightKey) || Input.GetKeyDown(rightKeyAdditonal))
        {
            HandleLaneSwitch(1);
        }
        if (Input.GetKeyDown(specialUpKey) || Input.GetKeyDown(specialUpKeyAdditonal))
        {
            UseFastDownBonus();
        }
        else if (Input.GetKeyDown(specialDownKey) || Input.GetKeyDown(specialDownKeyAdditonal))
        {
            UseSlowDownBonus();
        }
    }

    public void MobileKeyInputLeft()
    {
        HandleLaneSwitch(-1);
    }

    public void MobileKeyInputRight()
    {
        HandleLaneSwitch(1);
    }

    private void WebSocketCheck()
    {
        if (moveLeftRepeat)
        {
            HandleLaneSwitch(-1);
            moveLeftRepeat = false;
        }

        if (moveRightRepeat)
        {
            HandleLaneSwitch(1);
            moveRightRepeat = false;
        }

        if (specialLeftRepeat)
        {
            HandleSpecialSwitch(-1);
            specialLeftRepeat = false;
        }

        if (specialRightRepeat)
        {
            HandleSpecialSwitch(1);
            specialRightRepeat = false;
        }

        if (specialMidRepeat)
        {
            HandleSpecialSwitch(0);
            specialMidRepeat = false;
        }
    }

    public void RepeatMoveLeft()
    {
        moveLeftRepeat = true;
    }

    public void RepeatMoveRight()
    {
        moveRightRepeat = true;
    }

    public void RepeatSpecialLeft()
    {
        specialLeftRepeat = true;
    }

    public void RepeatSpecialRight()
    {
        specialRightRepeat = true;
    }

    public void RepeatSpecialMid()
    {
        specialMidRepeat = true;
    }

    private void LaneSwitchAnimate(int direction)
    {
        if (direction == -1)
        {
            controllerSprite.transform.DORotate(new Vector3(90f, -45f, 0f), 0.2f).OnComplete(() =>
            {
                controllerSprite.transform.DORotate(new Vector3(90f, 0f, 0f), 0.2f);
            });
        }
        else if (direction == 1)
        {
            controllerSprite.transform.DORotate(new Vector3(90f, 45f, 0f), 0.2f).OnComplete(() =>
            {
                controllerSprite.transform.DORotate(new Vector3(90f, 0f, 0f), 0.2f);
            });
        }
    }

    public void HandleLaneSwitch(int direction)
    {
        playersOnEachLane[currentLane].Remove(gameObject); // Удаление игрока из текущей линии
        currentLane = Mathf.Clamp(currentLane + direction, 0, lanes.Length - 1);
        playersOnEachLane[currentLane].Add(gameObject); // Добавление игрока на новую линию
        turnDirection = direction;
    }

    private Vector3 CalculateTargetPosition(int laneIndex)
    {
        float basePosition = lanes[laneIndex];
        List<GameObject> playersOnLane = playersOnEachLane[laneIndex];
        int playerIndex = playersOnLane.IndexOf(gameObject);

        // В зависимости от позиции игрока на линии устанавливаем его смещение
        if (playersOnLane.Count == 1)
        {
            return new Vector3(basePosition, transform.position.y, transform.position.z);
        }
        else if (playersOnLane.Count == 2)
        {
            return new Vector3(basePosition + (playerIndex == 0 ? -0.9f : 0.9f), transform.position.y, transform.position.z);
        }
        return new Vector3(basePosition, transform.position.y, transform.position.z);
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
    private bool IsFirstPlayerOnLane()
    {
        // Простейшая логика для определения, первый ли это игрок на линии
        return transform.position.x < lanes[currentLane];
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
                // Логика для мультиплеера
            }
            else
            {
                EndGame();
                GameController.Instance.GameOver();
            }
        }
        if (other.gameObject.CompareTag("WhitePoint"))
        {
            hasBonus = true; // Игрок получает бонус при входе на WhitePoint
            UpdateBonusUI();
        }
    }

    // Ускорение на белой линии

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("WhiteLine"))
        {
            if (!isOnWhiteLine)
            {
                ChangeSpeedModifier(increseSpeed);
                isOnWhiteLine = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("WhiteLine"))
        {
            isOnWhiteLine = false;
            ChangeSpeedModifier(1f);
        }
    }
    // Метод для использования бонуса замедления
    public void UseSlowDownBonus()
    {
        if (hasBonus)
        {
            StartCoroutine(SlowDown());
            hasBonus = false; // После использования бонус пропадает
            UpdateBonusUI();
        }
    }
    public void UseFastDownBonus()
    {
        if (hasBonus)
        {
            StartCoroutine(FastDown());
            hasBonus = false; // После использования бонус пропадает
            UpdateBonusUI();
        }
    }

    // Замедление игрока на короткое время
    private IEnumerator SlowDown()
    {
        ChangeSpeedModifier(decreaseSpeed);
        yield return new WaitForSeconds(bonusDuration); // Длительность замедления
        ChangeSpeedModifier(1f);
    }
    private IEnumerator FastDown()
    {
        ChangeSpeedModifier(increseSpeed);
        yield return new WaitForSeconds(bonusDuration); // Длительность замедления
        ChangeSpeedModifier(1f);
    }
    private void ChangeSpeedModifier(float newModifier)
    {
        // Плавно изменяем currentSpeedModifier с использованием DOTween
        DOTween.To(() => currentSpeedModifier, x => currentSpeedModifier = x, newModifier, transitionDuration);
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
    private void OnDestroy()
    {
        playersOnEachLane[currentLane].Remove(gameObject);
    }
}
