using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerSystem : MonoBehaviour
{
    private bool inBattle = false;
    private bool isWinner = false;
    
    private int hp = 100;
    private int battleCount = 35;
    private int playerCliks = 0;

    [SerializeField] private WebSocketServerBehavior webSocket;
    private PlayerController playerController;
    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("WhiteLine") && other.gameObject.CompareTag("Player"))
        {
            inBattle = true;
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("WhiteLine") && isWinner)
        {

        }
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
            playerController.EndGame();
    }

    public void ResetContest()
    {
        playerCliks = 0;
    }
}
