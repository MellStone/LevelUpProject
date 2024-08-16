using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Collections.Generic;

public class WebSocketServerBehavior : WebSocketBehavior
{
    private static Dictionary<string, PlayerController> clientPlayerMap = new Dictionary<string, PlayerController>();

    protected override void OnMessage(MessageEventArgs e)
    {
        Debug.Log("Message received from client: " + e.Data);
        var message = JsonUtility.FromJson<ClientMessage>(e.Data);
        HandleCommand(message.clientId, message.command);
    }

    private void HandleCommand(string clientId, string command)
    {
        if (!clientPlayerMap.ContainsKey(clientId))
        {
            AssignPlayerToClient(clientId);
        }

        PlayerController player = clientPlayerMap[clientId];

        if (player == null)
        {
            Debug.LogWarning($"No player found for client {clientId}");
            return;
        }

        if (GameController.Instance.isInMenu)
        {
            if (GameController.Instance.isGameOver)
            {
                HandleMenuCommands(command);
            }
            else
            {
                HandlePreGameCommands(command);
            }
        }
        else
        {
            HandleGameCommands(command, player);
        }
    }

    private void AssignPlayerToClient(string clientId)
    {
        if (clientPlayerMap.Count == 0)
        {
            clientPlayerMap[clientId] = GameController.Instance.player1;
        }
        else if (clientPlayerMap.Count == 1)
        {
            clientPlayerMap[clientId] = GameController.Instance.player2;
        }
        else
        {
            Debug.LogWarning("No available players to assign.");
        }
    }

    private void HandleMenuCommands(string command)
    {
        switch (command)
        {
            case "special_mid":
                Debug.Log("Start game command received from menu");
                WebSocketRepeater.Instance.restartGame = true;
                break;
            default:
                Debug.Log($"Unknown menu command: {command}");
                break;
        }
    }

    private void HandlePreGameCommands(string command)
    {
        switch (command)
        {
            case "special_mid":
                Debug.Log("Start game command received from pre comand");
                WebSocketRepeater.Instance.startGame = true;
                break;
            case "special_right":
                Debug.Log("Start game command received from pre comand");
                WebSocketRepeater.Instance.hidePlayerName = true;
                break;
            default:
                Debug.Log($"Unknown pre-game command: {command}");
                break;
        }
    }

    private void HandleGameCommands(string command, PlayerController player)
    {
        switch (command)
        {
            case "move_left":
                Debug.Log("Move left command received");
                player.RepeatMoveLeft();
                break;
            case "move_right":
                Debug.Log("Move right command received");
                player.RepeatMoveRight();
                break;
            case "special_left":
                Debug.Log("Special left command received");
                player.RepeatSpecialLeft();
                break;
            case "special_right":
                Debug.Log("Special right command received");
                player.RepeatSpecialRight();
                break;
            case "special_mid":
                Debug.Log("Special mid command received");
                player.RepeatSpecialMid();
                break;
            default:
                Debug.Log($"Unknown game command: {command}");
                break;
        }
    }


    // Метод для сброса клиентов
    public static void ResetClients()
    {
        clientPlayerMap.Clear();
        Debug.Log("Client assignments have been reset.");
    }

    private class ClientMessage
    {
        public string clientId;
        public string command;
    }
}
