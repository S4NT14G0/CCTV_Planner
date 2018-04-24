using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTVPlacementController : MonoBehaviour {

	[SerializeField]
	Canvas cameraCanvas;

    [SerializeField]
    CameraController cameraPrefab;

	AreaSecurityDefinitionController areaController;
	Phase2CanvasController cameraBankController;

	// All the active cameras
	List<CameraController> cameras;

	List<Material> originalMaterials;

	List<GameObject> coveredGrids;
	List<GameObject> mapGrid;

	// Use this for initialization
	void Start ()
	{
        this.enabled = false;
		cameraCanvas.enabled = false;

		originalMaterials = new List<Material> ();

		cameras = new List<CameraController> ();
		coveredGrids = new List<GameObject> ();
		mapGrid = new List<GameObject> ();

		areaController = this.GetComponent<AreaSecurityDefinitionController>();
		cameraBankController = cameraCanvas.gameObject.GetComponent<Phase2CanvasController> ();

		cameraBankController.onCreateCamera += HandleNewCameraEvent;
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void OnDisable()
    {
        if (cameraBankController != null)
            cameraBankController.onCreateCamera -= HandleNewCameraEvent;

        if (cameras != null)
        {
            foreach (CameraController camera in cameras)
            {
				if (camera != null) {
					camera.doneMoving -= OnCameraFinishedMoving;
					camera.clearCameraList -= OnCameraClearingList;
				}
            }
        }
    }

    public void BeginPhase()
    {
        this.enabled = true;
		cameraCanvas.enabled = true;
        mapGrid = areaController.GetMapGrid();

		foreach (GameObject grid in mapGrid) 
		{
			originalMaterials.Add (grid.GetComponent<Renderer> ().material);
		}
			
        areaController.enabled = false;
    }
		

	void HandleNewCameraEvent ()
	{
        CameraController camera = (CameraController)Instantiate(cameraPrefab, new Vector3(50f, 0.5f, 50f), new Quaternion(0f,0f,0f, 0f));
        cameras.Add(camera);
		camera.doneMoving += OnCameraFinishedMoving;
		camera.clearCameraList += OnCameraClearingList;
    }

	public void OnCameraFinishedMoving (List<GameObject> detectedGrids) 
	{
		foreach(GameObject grid in detectedGrids ) 
		{
			if (grid.GetComponent<Material> () != areaController.gridBuilding) 
			{
				if (coveredGrids.Contains (grid))
					Debug.Log ("Dupe");
				coveredGrids.Add (grid);
				grid.GetComponent<Renderer> ().material = areaController.gridCovered;
			}
		}
	}

	public void OnCameraClearingList (List<GameObject> removedGrids) 
	{
		foreach(GameObject grid in removedGrids ) 
		{
			if (coveredGrids.Contains (grid)) 
			{
				coveredGrids.Remove (grid);

			}

			if (!coveredGrids.Contains(grid))
				grid.GetComponent<Renderer> ().material = originalMaterials [mapGrid.IndexOf (grid)];
			
		}
	}
}
