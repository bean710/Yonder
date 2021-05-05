using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public ServerManager serverManager;
    public GameObject stickyNotePrefab;

    private List<GameObject> items = new List<GameObject>();
    private List<StickyNote> notes = new List<StickyNote>();

    public void addItem(GameObject gameObject)
    {
        items.Add(gameObject);
        serverManager.AddStickyNote(gameObject.GetComponent<StickyNote>());
    }

    public void removeItem(GameObject item)
    {
        serverManager.RemoveStickyNote(item.GetComponent<StickyNote>());
        items.Remove(item);
    }

    void AddNote(StickyNote sticky)
    {
        notes.Add(sticky);
        sticky.Stick(gameObject, sticky.transform.position, true);
    }

    public void AddData(LocalWindowResult.LocalWindowData localWindowData)
    {
        foreach (StickyNoteData stickyNoteData in localWindowData.stickyNotes)
        {
            AddStickyNoteFromData(stickyNoteData);
        }
    }

    public void AddStickyNoteFromData(StickyNoteData stickyNoteData)
    {
        GameObject newStickyNote = Instantiate(stickyNotePrefab, transform);
        
        Vector3 stickyPos = new Vector3(stickyNoteData.position.x, stickyNoteData.position.y, stickyNoteData.position.z);
        newStickyNote.transform.localPosition = stickyPos;
        newStickyNote.transform.Rotate(0f, 0f, 90f);

        StickyNote stickyNote = newStickyNote.GetComponent<StickyNote>();
        stickyNote.isOwnSticky = true;
        stickyNote.color = stickyNoteData.color;
        stickyNote.id = stickyNoteData.id;

        AddNote(stickyNote);
    }
}
