using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaController : MonoBehaviour {

	private GameObject[] grid;

	[SerializeField]
	private Material gridLow, gridMedium, gridHigh, gridSelected;

    private Vector3 dragBegin, dragEnd;

	private List<GameObject> selectedGrids;

    enum MousePhase
    {
        Clicked,
        Began,
        Moved,
        Ended
    }

    bool isClickHeld;
    MousePhase phase;

    // Use this for initialization
    void Start () {
		grid = new GameObject[100];

		for (int x = 0; x < grid.Length; x++) {
			for (int y = 0; y < grid.Length; y++) {
				grid [x] = GameObject.CreatePrimitive (PrimitiveType.Cube);

				grid [x].transform.position = new Vector3 (x + 0.5f, 0.5f, y + 0.5f);
				grid [x].GetComponent<Renderer> ().material = gridLow;
			}
		}

        isClickHeld = false;
        phase = MousePhase.Ended;
        selectedGrids = new List<GameObject> ();
	}

	public void SelectGrid (List<GameObject> selectedGridItems) {
        DeselectGrid();

		selectedGrids = selectedGridItems;

		foreach (GameObject grid in selectedGrids) {
			grid.GetComponent<Renderer> ().material = gridSelected;
		}
	}

	public void SelectGrid (GameObject selectedGridItems) {
		DeselectGrid ();

		selectedGrids.Add(selectedGridItems);

		foreach (GameObject grid in selectedGrids) {
			grid.GetComponent<Renderer> ().material = gridSelected;
		}
	}

	public void DeselectGrid () {
		foreach (GameObject grid in selectedGrids) {
			grid.GetComponent<Renderer> ().material = gridLow;
		}

		selectedGrids.Clear ();
	}

    // Update is called once per frame
    void Update() {

        if (!isClickHeld && Input.GetMouseButtonDown(0))
        {
            phase = MousePhase.Clicked;
        }
        else
        {
            if (Input.GetMouseButton(0))
                isClickHeld = true;
            else
                isClickHeld = false;
        }



        if (isClickHeld && phase == MousePhase.Ended)
        {
            phase = MousePhase.Began;

            // Dragging has began
            dragBegin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (isClickHeld && phase == MousePhase.Began)
        {
            phase = MousePhase.Moved;
            dragEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (!isClickHeld && phase == MousePhase.Moved)
        {
            phase = MousePhase.Ended;

            // Dragging has ended
            dragEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Calculate the rect that was formed and select the respective cubes
            Vector3 p1 = dragBegin;
            Vector3 p2 = dragEnd;

            Vector3 scale = p1 - p2;
            scale.x = Mathf.Abs(scale.x);
            scale.y = Mathf.Abs(15f);
            scale.z = Mathf.Abs(scale.z);

            Vector3 center = (p1 + p2) * 0.5f;
            Vector3 halfExtents = scale * 0.5f; //Halfextents are units in each direction instead of total length in units.
            RaycastHit[] check = Physics.BoxCastAll(center, halfExtents, Vector3.up);

            List<GameObject> selectedObjects = new List<GameObject>();

            foreach (RaycastHit hit in check)
            {
                selectedObjects.Add(hit.transform.gameObject);
            }

            SelectGrid(selectedObjects);
        }

        if (phase == MousePhase.Clicked)
        {
            // Click Begin
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;

                // Do something with the object that was hit by the raycast.
                SelectGrid(objectHit.gameObject);
            }
            else
            {
                DeselectGrid();
            }

            phase = MousePhase.Ended;
        }
	}
}