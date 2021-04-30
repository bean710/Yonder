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

            InvokeRepeating("SendWebSocketPing", 60f, 60f * pingFrequency);
            GetForeignData();
        };

        webSocket.OnMessage += (bytes) =>
        {
            Debug.Log($"Message: {System.Text.Encoding.UTF8.GetString(bytes)}");
        };

        webSocket.OnMessage += (bytes) =>
        {
            string bytesData = System.Text.Encoding.UTF8.GetString(bytes);

            WebSocketMessage message = JsonUtility.FromJson<WebSocketMessage>(bytesData);

            if (message.type == "foreignWindowData")
            {
                CompleteDataResult foreignWindowData = JsonUtility.FromJson<CompleteDataResult>(bytesData);

                foreach (CompleteDataResult.ForeignWindowData foreignWindow in foreignWindowData.data)
                {
                    Debug.Log($"Owner ID: {foreignWindow.ownerId}");
                    Debug.Log($"Apartment ID: {foreignWindow.apartmentId}");
                    Debug.Log($"Data: {foreignWindow.data}");

                    Debug.Log($"Color: {foreignWindow.data.stickyNotes[0].color}");
                }
            }
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

    async void GetForeignData()
    {
        if (webSocket.State == WebSocketState.Open)
        {
            Debug.Log("Getting foreign window data");
            await webSocket.SendText("{'action' : 'getData'}");
        }
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

[System.Serializable]
public class WebSocketMessage
{
    public string type;
}

[System.Serializable]
public class CompleteDataResult
{
    [System.Serializable]
    public class ForeignWindowData
    {
        [System.Serializable]
        public class ForeignWindowObjectData
        {
            [System.Serializable]
            public class StickyNoteData
            {
                [System.Serializable]
                public class StickyNotePos
                {
                    public int x, y, z;
                }

                public StickyNotePos position;
                public string color;
            }

            public List<StickyNoteData> stickyNotes;
        }

        public string apartmentId;
        public string ownerId;
        public ForeignWindowObjectData data;
    }

    public List<ForeignWindowData> data;
}
