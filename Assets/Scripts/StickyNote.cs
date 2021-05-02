using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StickyNote : MonoBehaviour
{
    private Collider m_Collider;
    private Rigidbody m_Rigidbody;
    private bool stuck = false;

    public string color = "red";
    public bool isOwnSticky = true;
    public string id = "";

    public Material red;
    public Material blue;
    public Material yellow;

    // Start is called before the first frame update
    void Start()
    {
        m_Collider = GetComponent<Collider>();
        m_Rigidbody = GetComponent<Rigidbody>();

        if (id == "")
            id = Guid.NewGuid().ToString();

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        switch (color)
        {
            case "red":
                meshRenderer.material = red;
                break;
            case "blue":
                meshRenderer.material = blue;
                break;
            case "yellow":
                meshRenderer.material = yellow;
                break;
            default:
                meshRenderer.material = yellow;
                break;
        }
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

    public void Stick(GameObject stuckTo, Vector3 closestPosition)
    {
        QuestDebug.Instance.Log($"Sticking");
        transform.SetParent(stuckTo.transform, true);
        transform.position = closestPosition;
        stuck = true;

        if (isOwnSticky)
        {
            WindowManager windowManager = stuckTo.GetComponent<WindowManager>();
            if (windowManager != null)
            {
                windowManager.addItem(gameObject);
            }
            else
            {
                QuestDebug.Instance.Log("No window manager found");
            }
        }
    }
}
