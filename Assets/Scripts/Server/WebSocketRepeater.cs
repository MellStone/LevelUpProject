using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

public class WebSocketRepeater : MonoBehaviour
{
    public static WebSocketRepeater Instance { get; private set; }   
    public bool restartGame = false;
    public bool startGame = false;
    public bool hidePlayerName = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; ;
        }
        else
        {
            Destroy(gameObject);
        }
        Application.targetFrameRate = 60;
    }
    
    private void Start()
    {
        WebSocketServerBehavior.ResetClients();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        {
            WebSocketServerBehavior.ResetClients();
        }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.G))
        {
            GameController.Instance.ResetHighScore();
        }
        
        
        if (Input.GetKey(KeyCode.RightControl))
        {
            hidePlayerName = true;
            GameController.Instance.HidePlayerNames(!hidePlayerName);
            hidePlayerName = false;
        }
        if (hidePlayerName)
        {
            GameController.Instance.HidePlayerNames(!hidePlayerName);
            hidePlayerName = false;
        }

        if (restartGame)
        {
            GameController.Instance.RestartGame();
            restartGame = false;
        }
        if (startGame)
        {
            GameController.Instance.StartGame();
            startGame = false;
        }
    }
}
