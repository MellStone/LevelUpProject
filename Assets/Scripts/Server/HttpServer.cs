using UnityEngine;
using System.IO;
using System.Net;
using System.Text;

public class HttpServer : MonoBehaviour
{
    private HttpListener listener;
    private string localIP;

    void Start()
    {
        localIP = GetLocalIPAddress();
        listener = new HttpListener();
        listener.Prefixes.Add($"http://{localIP}:8081/");
        listener.Start();
        listener.BeginGetContext(new System.AsyncCallback(OnRequest), listener);
        Debug.Log($"HTTP server started at http://{localIP}:8081");
    }

    private void OnRequest(System.IAsyncResult result)
    {
        if (!listener.IsListening) return;

        var context = listener.EndGetContext(result);
        listener.BeginGetContext(new System.AsyncCallback(OnRequest), listener);
        
        var request = context.Request;
        var response = context.Response;
        
        string clientIP = request.RemoteEndPoint.ToString();
        string userAgent = request.UserAgent;
        Debug.Log($"Request from {clientIP}, User-Agent: {userAgent}");

        // StreamingAssets folder (save exactly file like in editor)
        // For build version otherwise it didn't open
        string templatePath = Path.Combine(Application.streamingAssetsPath, "index.html.template");
        string page = File.ReadAllText(templatePath);

        // Replacing the placeholder with a real IP address
        page = page.Replace("__SERVER_IP__", localIP);

        byte[] buffer = Encoding.UTF8.GetBytes(page);
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
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

    void OnApplicationQuit()
    {
        if (listener != null)
        {
            listener.Stop();
            listener.Close();
        }
    }
}
