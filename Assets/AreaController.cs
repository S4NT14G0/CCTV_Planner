using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaController : MonoBehaviour {

	private GameObject[] grid;

	[SerializeField]
	private Material gridLow, gridMedium, gridHigh, gridSelected;

	private List<GameObject> selectedGrids;

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

		selectedGrids = new List<GameObject> ();
	}

	public void SelectGrid (List<GameObject> selectedGridItems) {
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
	void Update () {

		if (Input.GetMouseButton(0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast (ray, out hit)) {
				Transform objectHit = hit.transform;

				// Do something with the object that was hit by the raycast.
				SelectGrid (objectHit.gameObject);
			} else {
				DeselectGrid ();
			}
		}
	}
}