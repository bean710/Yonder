using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForeignWindow : MonoBehaviour
{
    public GameObject stickyNotePrefab;
    public GameObject backDrop;

    public Material offline;
    public Material online;

    List<StickyNote> notes = new List<StickyNote>();

    private Renderer backDropRenderer = null;

    // Start is called before the first frame update
    void Start()
    {
        backDropRenderer = backDrop.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddNote(StickyNote sticky)
    {
        notes.Add(sticky);
        sticky.Stick(gameObject, sticky.transform.position);
    }

    public void AddData(CompleteDataResult.ForeignWindowData foreignWindowData)
    {
        SetOnlineStatus(foreignWindowData.onlineStatus);

        foreach (StickyNoteData stickyNoteData in foreignWindowData.stickyNotes)
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
        newStickyNote.transform.Rotate(stickyNoteData.position.rotation, 0f, 0f);

        StickyNote stickyNote = newStickyNote.GetComponent<StickyNote>();
        stickyNote.isOwnSticky = false;
        stickyNote.color = stickyNoteData.color;
        stickyNote.id = stickyNoteData.id;

        AddNote(stickyNote);
    }

    public void RemoveStickyNoteFromData(string stickyNoteId)
    {
        StickyNote noteToRemove = notes.Find((note) => note.id == stickyNoteId);

        if (noteToRemove)
        {
            notes.Remove(noteToRemove);
            Destroy(noteToRemove.gameObject);
        }
    }

    public void SetOnlineStatus(bool onlineStatus)
    {
        if (backDropRenderer != null)
            backDropRenderer.material = onlineStatus ? online : offline;
    }
}
