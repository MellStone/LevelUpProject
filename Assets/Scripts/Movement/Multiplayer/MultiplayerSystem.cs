using System.Collections;
using UnityEngine;

public class MultiplayerSystem : MonoBehaviour
{
    private bool inBattle = false;
    private bool isWinner = false;
    private int hp = 100;
    private int playerClicks = 0;

    private Coroutine exitCoroutine;
    private Coroutine contestCoroutine;
    private MultiplayerSystem opponent;
    private bool isActiveInBattle = false;

    [SerializeField] private WebSocketServerBehavior webSocket;
    private PlayerController playerController;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        opponent = FindOpponent(); // Метод для поиска противника на сцене
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("WhiteLine"))
        {
            EnterBattle();
        }
        else if (other.gameObject.CompareTag("WhitePoint"))
        {
            if (!isWinner)
            {
                WinAutomatically();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("WhiteLine"))
        {
            exitCoroutine = StartCoroutine(ExitBattleAfterDelay());
        }
    }

    private void EnterBattle()
    {
        inBattle = true;
        if (exitCoroutine != null)
        {
            StopCoroutine(exitCoroutine);
            exitCoroutine = null;
        }
        
        // Начало конкуренции
        if (opponent.inBattle)
        {
            StartContest();
        }
    }

    private IEnumerator ExitBattleAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        inBattle = false;
        if (contestCoroutine != null)
        {
            StopCoroutine(contestCoroutine);
            contestCoroutine = null;
        }
    }

    private void StartContest()
    {
        if (contestCoroutine == null)
        {
            contestCoroutine = StartCoroutine(ContestRoutine());
        }
    }

    private IEnumerator ContestRoutine()
    {
        isActiveInBattle = true;
        while (inBattle && opponent.inBattle)
        {
            // Логика для отслеживания кликов
            if (playerClicks > opponent.playerClicks)
            {
                isWinner = true;
                opponent.isWinner = false;
            }
            else if (playerClicks < opponent.playerClicks)
            {
                isWinner = false;
                opponent.isWinner = true;
            }
            yield return null;
        }
        isActiveInBattle = false;
    }

    private void WinAutomatically()
    {
        if (!opponent.inBattle || !opponent.isActiveInBattle)
        {
            isWinner = true;
            // Здесь можно добавить логику для выдачи награды
            GiveReward();
        }
    }

    private void GiveReward()
    {
        // Логика для выдачи награды
        Debug.Log("Player " + gameObject.name + " has won the battle and receives the reward.");
    }

    private MultiplayerSystem FindOpponent()
    {
        MultiplayerSystem[] players = FindObjectsOfType<MultiplayerSystem>();
        foreach (var player in players)
        {
            if (player != this)
            {
                return player;
            }
        }
        return null;
    }
}
