using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlacementController : MonoBehaviour {
    EventSystem FindEvent;
    Event Find2;

    AreaController areaController;

    [SerializeField]
    Canvas placementCanvas;

	// Use this for initialization
	void Start () {
        this.enabled = false;
        placementCanvas.enabled = false;
        areaController = this.GetComponent<AreaController>();
	}
	
	// Update is called once per frame
	void Update () {


	}

    public void BeginPhase()
    {
        this.enabled = true;
        placementCanvas.enabled = true;
        List<GameObject> mapGrid = areaController.GetMapGrid();

        areaController.enabled = false;
    }
}
