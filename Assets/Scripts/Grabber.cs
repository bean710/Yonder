using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    // Used for the dropdown in the inspector
    public enum LeftOrRight
    {
        Left,
        Right
    };

    public GameObject stickyNote;
    public Transform stickyPos;
    public GameObject window;
    public LeftOrRight leftOrRight = LeftOrRight.Left;

    private GameObject holding = null;

    private StickyStack touchingStack = null;
    private StickyNote touchingStickyNote = null;

    private OVRInput.Controller controller;
    private Collider windowCollider;

    // Start is called before the first frame update
    void Start()
    {
        // Set the controller var based on the inspector selection
        if (leftOrRight == LeftOrRight.Left)
            controller = OVRInput.Controller.LTouch;
        else
            controller = OVRInput.Controller.RTouch;

        windowCollider = window.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        OVRInput.Update();

        if (holding == null && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            if (touchingStack != null)
            {
                holding = Instantiate(stickyNote, stickyPos);
                StickyNote newStickyNote = holding.GetComponent<StickyNote>();
                newStickyNote.color = touchingStack.color.ToString();
            }
            else if (touchingStickyNote != null)
            {
                holding = touchingStickyNote.gameObject;
                holding.transform.SetParent(stickyPos, true);
                holding.transform.position = stickyPos.position;
                StickyNote stickyNote = holding.GetComponent<StickyNote>();
                stickyNote.PickUp();
            }
        }

        if (holding != null && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller) == false)
        {
            StickyNote sticky = holding.GetComponent<StickyNote>();
            Vector3 closest = windowCollider.ClosestPoint(holding.transform.position);
            float dist = Vector3.Distance(closest, holding.transform.position);

            QuestDebug.Instance.Log($"Dist: {dist}");

            if (dist < 0.5f)
                sticky.Stick(window, closest);
            else
                sticky.Drop();

            holding = null;
        }
    }

    private void FixedUpdate() {
        OVRInput.FixedUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "StickyStack")
        {
            touchingStack = other.gameObject.GetComponent<StickyStack>();
        }
        else if (other.gameObject.tag == "StickyNote")
        {
            touchingStickyNote = other.gameObject.GetComponent<StickyNote>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "StickyStack")
            touchingStack = null;
        else if (other.gameObject.tag == "StickyNote")
            touchingStickyNote = null;
    }
}
