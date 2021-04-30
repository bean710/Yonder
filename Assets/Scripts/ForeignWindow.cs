using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForeignWindow : MonoBehaviour
{
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
}
