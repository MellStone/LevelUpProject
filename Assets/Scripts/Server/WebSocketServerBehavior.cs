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
