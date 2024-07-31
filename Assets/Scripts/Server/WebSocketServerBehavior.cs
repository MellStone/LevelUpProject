using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Collections.Generic;

public class WebSocketServerBehavior : WebSocketBehavior
{
    private static Dictionary<string, string> clientIds = new Dictionary<string, string>();

    protected override void OnMessage(MessageEventArgs e)
    {
        Debug.Log("Message received from client: " + e.Data);
        var message = JsonUtility.FromJson<ClientMessage>(e.Data);
        HandleCommand(message.clientId, message.command);
    }

    private void HandleCommand(string clientId, string command)
    {
        switch (command)
        {
            case "move_left":
                Debug.Log($"Client {clientId} Moving Left");
                if (PlayerMovement.Instance != null)
                {
                    PlayerMovement.Instance.HandleLaneSwitch(-1);
                }

                break;
            case "move_right":
                Debug.Log($"Client {clientId} Moving Right");
                if (PlayerMovement.Instance != null)
                {
                    PlayerMovement.Instance.HandleLaneSwitch(1);
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