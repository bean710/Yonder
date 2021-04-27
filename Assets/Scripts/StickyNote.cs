using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyNote : MonoBehaviour
{
    private Collider m_Collider;
    private Rigidbody m_Rigidbody;
    private bool stuck = false;

    // Start is called before the first frame update
    void Start()
    {
        m_Collider = GetComponent<Collider>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Drop()
    {
        QuestDebug.Instance.Log($"Dropping");
        m_Collider.enabled = true;
        m_Rigidbody.isKinematic = false;
        m_Rigidbody.useGravity = true;
        transform.SetParent(null, true);
        stuck = false;
    }

    public void Stick(GameObject gameObject, Vector3 closestPosition)
    {
        QuestDebug.Instance.Log($"Sticking");
        transform.SetParent(gameObject.transform, true);
        transform.position = closestPosition;
        stuck = true;
    }
}
