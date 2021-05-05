using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForeignWindow : MonoBehaviour
{
    public GameObject stickyNotePrefab;

    List<StickyNote> notes = new List<StickyNote>();

    // Start is called before the first frame update
    void Start()
    {
        
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

    public void RemoveStickyNoteFromData(RemoveStickyNoteResult.RemoveStickyNoteData stickyNoteData)
    {
        foreach (StickyNote stickyNote in notes)
        {
            if (stickyNote.id == stickyNoteData.stickyNoteId)
            {
                notes.Remove(stickyNote);
                Destroy(stickyNote.gameObject);
            }
        }
    }
}
