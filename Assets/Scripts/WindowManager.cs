using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public ServerManager serverManager;

    private List<GameObject> items = new List<GameObject>();

    public void addItem(GameObject gameObject)
    {
        items.Add(gameObject);
        serverManager.AddStickyNote(gameObject.GetComponent<StickyNote>());
    }
}
