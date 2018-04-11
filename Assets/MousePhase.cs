using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePhase : MonoBehaviour {

    public enum Phase
    {
        Clicked,
        Began,
        Moved,
        Ended
    }

    bool isClicked;
    public static Phase phase {
        get;
        set;
    }

    // Use this for initialization
    void Start () {
        isClicked = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0))
            isClicked = true;

        if (isClicked && !Input.GetMouseButton(0))
            phase = Phase.Clicked;

        if (Input.GetMouseButton(0) && !isClicked)
        {
            phase = Phase.Began;
        }
        else if (Input.GetMouseButton(0) && isClicked)
        {
            phase = Phase.Moved;
        }
        else if (!Input.GetMouseButton(0) && isClicked)
        {
            phase = Phase.Ended;
        }
    }
}
