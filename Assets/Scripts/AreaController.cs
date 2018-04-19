﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AreaController : MonoBehaviour {

	private List<GameObject> mapGrid;
    private List<Material> gridMarkings;
    
	[SerializeField]
	private Material gridLow, gridMedium, gridHigh, gridSelected, gridNone, gridBuilding;

    private Vector3 dragBegin, dragEnd;

	private List<GameObject> selectedGrids;

    [SerializeField]
    GameObject areaCanvas;

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
        mapGrid = new List<GameObject>();
        gridMarkings = new List<Material>();

		for (int x = 0; x < 100; x++) {
			for (int y = 0; y < 100; y++) {
                GameObject grid = GameObject.CreatePrimitive (PrimitiveType.Cube);

                grid.transform.position = new Vector3 (x + 0.5f, 0.5f, y + 0.5f);
                grid.GetComponent<Renderer> ().material = gridNone;

                mapGrid.Add(grid);

                gridMarkings.Add(gridNone);
			}
		}

        isClickHeld = false;
        phase = MousePhase.Ended;
        selectedGrids = new List<GameObject> ();
	}

	void SelectGrid (List<GameObject> selectedGridItems) {
        DeselectGrid();

		selectedGrids = selectedGridItems;

		foreach (GameObject grid in selectedGrids) {
            grid.GetComponent<Renderer> ().material = gridSelected;
		}
	}

	void SelectGrid (GameObject selectedGridItems) {
		DeselectGrid ();
		selectedGrids.Add(selectedGridItems);

		foreach (GameObject grid in selectedGrids) {
			grid.GetComponent<Renderer> ().material = gridSelected;
		}
	}

	void DeselectGrid () {

        foreach(GameObject gridItem in selectedGrids)
        {
            int index = mapGrid.IndexOf(gridItem);
            gridItem.GetComponent<Renderer>().material = gridMarkings[index];
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
            if (EventSystem.current.IsPointerOverGameObject(-1))
                return;
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

    public void TagLowRisk()
    {
        foreach (GameObject gridItem in selectedGrids)
        {
            int index = mapGrid.IndexOf(gridItem);
            gridItem.GetComponent<Renderer>().material = gridLow;
            gridMarkings[index] = gridLow;
        }

        DeselectGrid();
    }

    public void TagMediumRisk()
    {
        foreach (GameObject gridItem in selectedGrids)
        {
            int index = mapGrid.IndexOf(gridItem);
            gridItem.GetComponent<Renderer>().material = gridMedium;
            gridMarkings[index] = gridMedium;
        }

        DeselectGrid();
    }

    public void TagHighRisk()
    {
        foreach (GameObject gridItem in selectedGrids)
        {
            int index = mapGrid.IndexOf(gridItem);
            gridItem.GetComponent<Renderer>().material = gridHigh;
            gridMarkings[index] = gridHigh;
        }

        DeselectGrid();
    }

    public void TagBuilding()
    {
        foreach (GameObject gridItem in selectedGrids)
        {
            int index = mapGrid.IndexOf(gridItem);
            gridItem.GetComponent<Renderer>().material = gridBuilding;
            gridMarkings[index] = gridBuilding;
        }

        DeselectGrid();
    }

    public List<GameObject> GetMapGrid ()
    {
        return mapGrid;
    }

    private void OnDisable()
    {
        areaCanvas.SetActive(false);
    }
}