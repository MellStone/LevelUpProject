using UnityEngine;
using WebSocketSharp.Server;

public class WebSocketServerManager : MonoBehaviour
{
    private WebSocketServer wss;

    void Start()
    {
        string localIP = GetLocalIPAddress();
        wss = new WebSocketServer($"ws://{localIP}:8080");
        wss.AddWebSocketService<WebSocketServerBehavior>("/GameControl");
        wss.Start();
        Debug.Log($"WebSocket server started at ws://{localIP}:8080/GameControl");
    }

    void OnApplicationQuit()
    {
        if (wss != null)
        {
            wss.Stop();
        }
    }

    private string GetLocalIPAddress()
    {
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
}
