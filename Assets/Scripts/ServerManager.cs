using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;

public class ServerManager : MonoBehaviour
{
    public List<ForeignWindow> foreignWindows;
    public float pingFrequency = 5f; // Minutes; 10 minutes is timeout

    private WebSocket webSocket;

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
                    Debug.Log($"Apartment ID: {foreignWindow.apartmentId}");

                    foreignWindows[0].AddData(foreignWindow);
                }
            }
            else if (message.type == "addStickyNote")
            {
                QuestDebug.Instance.Log("Getting new sticky info");
                AddStickyNoteResult stickyNoteData = JsonUtility.FromJson<AddStickyNoteResult>(bytesData);
                foreignWindows[0].AddStickyNoteFromData(stickyNoteData.data);
            }
            else if (message.type == "removeStickyNote")
            {
                QuestDebug.Instance.Log("Removing foreign sticky note");
                RemoveStickyNoteResult removeStickyNoteData = JsonUtility.FromJson<RemoveStickyNoteResult>(bytesData);
                Debug.Log(removeStickyNoteData);
                Debug.Log(removeStickyNoteData.data);
                Debug.Log(removeStickyNoteData.data.stickyNoteId);
                foreignWindows[0].RemoveStickyNoteFromData(removeStickyNoteData.data);
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

    public async void AddStickyNote(StickyNote stickyNote)
    {
        if (webSocket.State == WebSocketState.Open)
        {
            StickyNoteData stickyNoteData = new StickyNoteData(stickyNote);
            string serializedStickyNoteData = JsonUtility.ToJson(stickyNoteData);

            QuestDebug.Instance.Log("Sending add message");

            await webSocket.SendText("{\"action\" : \"addStickyNote\", \"apartmentId\" : \"0#0\", \"stickyNoteData\" : " + serializedStickyNoteData + "}");
        }
        else
        {
            QuestDebug.Instance.Log("No Socket Connection");
        }
    }

    public async void RemoveStickyNote(StickyNote stickyNote)
    {
        if (webSocket.State == WebSocketState.Open)
        {
            QuestDebug.Instance.Log("Sending remove message");

            await webSocket.SendText("{\"action\" : \"removeStickyNote\", \"apartmentId\" : \"0#0\", \"stickyNoteId\" : \"" + stickyNote.id + "\"}");
        }
        else
        {
            QuestDebug.Instance.Log("No Socket Connection");
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
public class StickyNoteData
{
    public StickyNoteData(StickyNote stickyNote)
    {
        this.position = new StickyNotePos(stickyNote.transform.localPosition);
        this.color = stickyNote.color;
        this.id = stickyNote.id;
    }

    [System.Serializable]
    public class StickyNotePos
    {
        public StickyNotePos(Vector3 position)
        {
            this.x = position.x;
            this.y = position.y;
            this.z = position.z;
        }

        public float x, y, z;
    }

    public StickyNotePos position;
    public string id;
    public string color;
}

[System.Serializable]
public class CompleteDataResult
{
    [System.Serializable]
    public class ForeignWindowData
    {
        public string apartmentId;
        public List<StickyNoteData> stickyNotes;
    }

    public List<ForeignWindowData> data;
}

[System.Serializable]
public class AddStickyNoteResult
{
    public StickyNoteData data;
}

[System.Serializable]
public class RemoveStickyNoteResult
{
    [System.Serializable]
    public class RemoveStickyNoteData
    {
        public string apartmentId;
        public string stickyNoteId;
    }

    public RemoveStickyNoteData data;
}
