using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;

public class ServerManager : MonoBehaviour
{
    private WebSocket webSocket;

    private float pingFrequency = 5f; // Minutes; 10 minutes is timeout

    // Start is called before the first frame update
    async void Start()
    {
        webSocket = new WebSocket("wss://1kvhz41590.execute-api.us-west-1.amazonaws.com/production");

        webSocket.OnOpen += () =>
        {
            Debug.Log("Connected!!!");

            InvokeRepeating("SendWebSocketPing", 0.0f, 60f * pingFrequency);
        };

        webSocket.OnMessage += (bytes) =>
        {
            Debug.Log($"Message: {System.Text.Encoding.UTF8.GetString(bytes)}");
        };

        await webSocket.Connect();
    }

    // Update is called once per frame
    void Update()
    {
    #if !UNITY_WEBGL || UNITY_EDITOR
        webSocket.DispatchMessageQueue();
    #endif 
    }

    async void SendWebSocketPing()
    {
        if (webSocket.State == WebSocketState.Open)
        {
            Debug.Log("Pinging");
            await webSocket.SendText("{'action' : 'ping'}");
        }
    }

    async void OnApplicationQuit()
    {
        await webSocket.Close();
    }
}
