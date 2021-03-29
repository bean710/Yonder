using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class QuestDebug : MonoBehaviour
{    
    static QuestDebug _instance;
    public static QuestDebug Instance { get { return _instance; } }

    [SerializeField] Transform rotationAnchor;
    GameObject debugWindow;
    TMP_Text debugText;
    int maxLines = 13;
    static string[] logs;


    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        if (!rotationAnchor) { rotationAnchor = GameObject.Find("CenterEyeAnchor").transform; }

        debugWindow = transform.GetChild(0).gameObject;
        debugText = debugWindow.GetComponentInChildren<TMP_Text>();
        
        logs = new string[maxLines];

        ClearLogs();
    }

    void Update()
    {
        // Toggle debug window.
        if (OVRInput.GetDown(OVRInput.Button.Two)) {
            debugWindow.SetActive(!debugWindow.activeSelf);
        }

        // Rotate canvas according to player viewplane.
        if (debugWindow.activeSelf) {
            transform.rotation = rotationAnchor.rotation;
        }
    }

    public void Log(string line) {

        // Shift existing log lines up an index, from back to front.
        int i;

        for (i = 0; i < maxLines - 1; i++) {
            logs[i] = logs[i + 1];
        }
        
        // When we reach index 0, assign new log line.
        logs[i] = $"{GetFormattedTimestamp()}: {line}";
        
        // Format text with newlines for UI element.
        string formattedText = "";

        foreach (string log in logs) {
            if (log.Length > 0)
                formattedText += ($"{log}\n");
        }

        // Assign formatted text to UI element.
        debugText.text = formattedText;
    }

    void ClearLogs() {

        for (int i = 0; i < maxLines; i++) {
            logs[i] = "";
        }

        debugText.text = "";
    }

    string GetFormattedTimestamp() {

        float time = Time.time;

        int minutes = (int) time / 60 ;
        int seconds = (int) time - 60 * minutes;
        int milliseconds = (int) (1000 * (time - minutes * 60 - seconds));
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);

    }
}
