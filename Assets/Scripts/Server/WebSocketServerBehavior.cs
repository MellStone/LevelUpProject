using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Collections.Generic;
using Unity.VisualScripting;

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
            // Assign player controller instances to clients if not already mapped
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
                return;
            }
        }

        PlayerController player = clientPlayerMap[clientId];

        switch (command)
        {
            case "move_left":
                Debug.Log($"Client {clientId} Moving Left");
                if (player != null)
                {
                    player.HandleLaneSwitch(-1);
                }
                break;
            case "move_right":
                Debug.Log($"Client {clientId} Moving Right");
                if (player != null)
                {
                    player.HandleLaneSwitch(1);
                }
                break;
            
            case "special_left":
                Debug.Log($"Client {clientId} use special left");
                if (player != null)
                {
                    player.HandleSpecialSwitch(-1);
                }
                break;
            case "special_right":
                Debug.Log($"Client {clientId} use special right");
                if (player != null)
                {
                    player.HandleSpecialSwitch(1);
                }
                break;
            case "special_mid":
                Debug.Log($"Client {clientId} use special mid");
                if (player != null)
                {
                    player.HandleSpecialSwitch(0);
                }
                break;
            
            default:
                Debug.Log($"Unknown command from client {clientId}: {command}");
                break;
        }
    }

    private class ClientMessage
    {
        public string clientId;
        public string command;
    }
}
