using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;

public class ServerManager : MonoBehaviour
{
    public WindowManager localWindow;
    public List<ForeignWindow> foreignWindows;
    public float pingFrequency = 5f; // Minutes; 10 minutes is timeout

    private WebSocket webSocket;
    private Dictionary<int, ForeignWindow> foreignWindowsMap = new Dictionary<int, ForeignWindow>();

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
                int index = 0;

                foreach (CompleteDataResult.ForeignWindowData foreignWindow in foreignWindowData.data)
                {
                    Debug.Log($"Foreign Apartment ID: {foreignWindow.apartmentId}");

                    foreignWindows[index].AddData(foreignWindow);
                    foreignWindowsMap.Add(foreignWindow.apartmentId, foreignWindows[index]);
                    
                    index++;
                }
            }
            else if (message.type == "addStickyNote")
            {
                QuestDebug.Instance.Log("Getting new sticky info");
                AddStickyNoteResult stickyNoteData = JsonUtility.FromJson<AddStickyNoteResult>(bytesData);

                if (foreignWindowsMap.ContainsKey(stickyNoteData.apartmentId))
                    foreignWindowsMap[stickyNoteData.apartmentId].AddStickyNoteFromData(stickyNoteData.data);
            }
            else if (message.type == "removeStickyNote")
            {
                QuestDebug.Instance.Log("Removing foreign sticky note");
                RemoveStickyNoteResult removeStickyNoteData = JsonUtility.FromJson<RemoveStickyNoteResult>(bytesData);

                if (foreignWindowsMap.ContainsKey(removeStickyNoteData.apartmentId))
                    foreignWindowsMap[removeStickyNoteData.apartmentId].RemoveStickyNoteFromData(removeStickyNoteData.stickyNoteId);
            }
            else if (message.type == "localWindowData")
            {
                Debug.Log("Got local window data");
                LocalWindowResult localWindowResult = JsonUtility.FromJson<LocalWindowResult>(bytesData);
                Debug.Log(localWindowResult.data);

                if (localWindowResult.data != null)
                    localWindow.AddData(localWindowResult.data);
            }
            else if (message.type == "setOnlineStatus")
            {
                SetOnlineResult setOnlineResult = JsonUtility.FromJson<SetOnlineResult>(bytesData);
                Debug.Log($"Setting online to: {setOnlineResult.onlineStatus} of apartment {setOnlineResult.apartmentId}");

                if (foreignWindowsMap.ContainsKey(setOnlineResult.apartmentId))
                    foreignWindowsMap[setOnlineResult.apartmentId].SetOnlineStatus(setOnlineResult.onlineStatus);
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
            await webSocket.SendText("{\"action\" : \"getData\", \"deviceId\" : \"" + SystemInfo.deviceUniqueIdentifier + "\"}");
        }
    }

    public async void AddStickyNote(StickyNote stickyNote)
    {
        if (webSocket.State == WebSocketState.Open)
        {
            StickyNoteData stickyNoteData = new StickyNoteData(stickyNote);
            string serializedStickyNoteData = JsonUtility.ToJson(stickyNoteData);

            QuestDebug.Instance.Log("Sending add message");

            await webSocket.SendText("{\"action\" : \"addStickyNote\", \"deviceId\": \"" + SystemInfo.deviceUniqueIdentifier + "\", \"stickyNoteData\" : " + serializedStickyNoteData + "}");
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

            await webSocket.SendText("{\"action\" : \"removeStickyNote\", \"deviceId\": \"" + SystemInfo.deviceUniqueIdentifier + "\", \"stickyNoteId\" : \"" + stickyNote.id + "\"}");
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
        this.position = new StickyNotePos(stickyNote.transform.localPosition, stickyNote.transform.eulerAngles.x);
        this.color = stickyNote.color;
        this.id = stickyNote.id;
    }

    [System.Serializable]
    public class StickyNotePos
    {
        public StickyNotePos(Vector3 position, float rotation = 0f)
        {
            this.x = position.x;
            this.y = position.y;
            this.z = position.z;
            this.rotation = rotation;
        }

        public float x, y, z;
        public float rotation;
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
        public int apartmentId;
        public bool onlineStatus;
        public List<StickyNoteData> stickyNotes;
    }

    public List<ForeignWindowData> data;
}

[System.Serializable]
public class AddStickyNoteResult
{
    public StickyNoteData data;
    public int apartmentId;
}

[System.Serializable]
public class RemoveStickyNoteResult
{
    public string stickyNoteId;
    public int apartmentId;
}

[System.Serializable]
public class LocalWindowResult
{
    [System.Serializable]
    public class LocalWindowData
    {
        public int apartmentId;
        public List<StickyNoteData> stickyNotes;
    }

    public LocalWindowData data;
}

[System.Serializable]
public class SetOnlineResult
{
    public int apartmentId;
    public bool onlineStatus;
}
