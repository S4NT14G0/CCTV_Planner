using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementController : MonoBehaviour {

	[SerializeField]
	Canvas cameraCanvas;

    AreaController areaController;
	CameraBankController cameraBankController;

	// All the active cameras
	List<CameraController> cameras;

	// Use this for initialization
	void Start ()
	{
        this.enabled = false;
		cameraCanvas.enabled = false;

		cameras = new List<CameraController> ();

        areaController = this.GetComponent<AreaController>();
		cameraBankController = cameraCanvas.gameObject.GetComponent<CameraBankController> ();

		cameraBankController.onCreateCamera += HandleNewCameraEvent;
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void BeginPhase()
    {
        this.enabled = true;
		cameraCanvas.enabled = true;
        List<GameObject> mapGrid = areaController.GetMapGrid();
        areaController.enabled = false;
    }
		

	void HandleNewCameraEvent ()
	{
		Debug.Log ("Handled");
	}
}
