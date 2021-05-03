using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StickyNote : MonoBehaviour
{
    private Collider m_Collider;
    private Rigidbody m_Rigidbody;
    private bool stuck = false;

    public string color = "Red";
    public bool isOwnSticky = true;
    public string id = "";

    public Material Red;
    public Material Blue;
    public Material Yellow;

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
            case "Red":
                meshRenderer.material = Red;
                break;
            case "Blue":
                meshRenderer.material = Blue;
                break;
            case "Yellow":
                meshRenderer.material = Yellow;
                break;
            default:
                meshRenderer.material = Yellow;
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
