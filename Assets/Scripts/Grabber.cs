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
    public LeftOrRight leftOrRight = LeftOrRight.Left;

    private GameObject holding = null;

    private bool touchingStack = false;

    private OVRInput.Controller controller;

    // Start is called before the first frame update
    void Start()
    {
        // Set the controller var based on the inspector selection
        if (leftOrRight == LeftOrRight.Left)
            controller = OVRInput.Controller.LTouch;
        else
            controller = OVRInput.Controller.RTouch;
    }

    // Update is called once per frame
    void Update()
    {
        OVRInput.Update();

        if (holding == null && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            QuestDebug.Instance.Log($"Grabbing with { (leftOrRight == LeftOrRight.Left ? "Left" : "Right") }");
            if (touchingStack && holding == null)
                holding = Instantiate(stickyNote, stickyPos);
        }

        if (holding != null && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller) == false)
        {
            StickyNote sticky = holding.GetComponent<StickyNote>();
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
            QuestDebug.Instance.Log($"Collided with a {other.gameObject.tag}");
            touchingStack = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "StickyStack")
            touchingStack = false;
    }
}
