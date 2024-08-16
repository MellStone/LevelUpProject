using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WebSocketRepeater : MonoBehaviour
{
    public static WebSocketRepeater Instance { get; private set; }   
    public bool restartGame = false;
    public bool startGame = false;
    
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
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        {
            WebSocketServerBehavior.ResetClients();
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
