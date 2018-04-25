using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CCTVPlacementController : MonoBehaviour {

	[SerializeField]
	Canvas cameraCanvas;

	[SerializeField]
	Text securityCoverageIndexTextBox;

    [SerializeField]
    CameraController cameraPrefab;

	AreaSecurityDefinitionController areaController;
	Phase2CanvasController cameraBankController;

    CameraController selectedCamera;

	// All the active cameras
	List<CameraController> cameras;

	List<Material> originalMaterials;

	List<GameObject> coveredGrids;
	List<GameObject> mapGrid;

    enum MousePhase
    {
        Clicked,
        Began,
        Moved,
        Ended
    }

    private Vector3 dragBegin, dragEnd;

    bool isClickHeld;
    MousePhase phase;

    // Use this for initialization
    void Start ()
	{
        this.enabled = false;
		cameraCanvas.enabled = false;

        isClickHeld = false;
        phase = MousePhase.Ended;

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

        if (phase == MousePhase.Moved)
        {
            dragEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            selectedCamera.transform.position = new Vector3(dragEnd.x, 0.5f, dragEnd.z);
        }


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

            if (EventSystem.current.IsPointerOverGameObject(-1))
                return;
            // Click Begin
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;

                CameraController tempCam = objectHit.gameObject.GetComponent<CameraController>();

                if (tempCam != null)
                {
                    if (selectedCamera != null)
                        selectedCamera.IsSelected = false;

                    selectedCamera = tempCam;
                    selectedCamera.IsSelected = true;
                }
            }

        }
        else if (isClickHeld && phase == MousePhase.Began)
        {
            phase = MousePhase.Moved;

            dragEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            selectedCamera.transform.position = new Vector3(dragEnd.x, 0.5f, dragEnd.z);
        }
        else if (!isClickHeld && phase == MousePhase.Moved)
        {
            phase = MousePhase.Ended;

            // Dragging has ended
            dragEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            selectedCamera.transform.position = new Vector3(dragEnd.x, 0.5f, dragEnd.z);
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

                CameraController tempCam = objectHit.gameObject.GetComponent<CameraController>();

                if (tempCam != null)
                {
                    if (selectedCamera != null)
                        selectedCamera.IsSelected = false;

                    selectedCamera = tempCam;
                    selectedCamera.IsSelected = true;
                }
            }

            phase = MousePhase.Ended;
        }
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
			originalMaterials.Add (grid.GetComponent<Renderer> ().sharedMaterial);
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
			if (grid.GetComponent<Renderer> ().sharedMaterial.name != areaController.gridBuilding.name) 
			{
				coveredGrids.Add (grid);
				grid.GetComponent<Renderer> ().sharedMaterial = areaController.gridCovered;
			}
		}
		securityCoverageIndexTextBox.text = "Security Coverage Index: " + (CalculateSecurityCoverageIndex () * 100).ToString ("00.00") + "%";
	}

	float CalculateSecurityCoverageIndex () 
	{
		float weightedCameraAreaCovered = 0;
		float totalWeightedArea = 0;

		foreach (GameObject grid in mapGrid) 
		{
			if (grid.GetComponent<Renderer> ().sharedMaterial == areaController.gridCovered) {

				Material originalMaterial = originalMaterials [mapGrid.IndexOf (grid)];


				if (originalMaterial == areaController.gridHigh) 
				{
					weightedCameraAreaCovered += 3;
					totalWeightedArea += 3;
				} 
				else if (originalMaterial == areaController.gridMedium) 
				{
					weightedCameraAreaCovered += 2;
					totalWeightedArea += 2;
				}				
				else if (originalMaterial == areaController.gridLow) 
				{
					weightedCameraAreaCovered += 1;
					totalWeightedArea += 1;
				}
				else if (originalMaterial == areaController.gridNone) 
				{
					weightedCameraAreaCovered += 0.5f;
					totalWeightedArea += 0.5f;
				}

			} else {
				Material material = grid.GetComponent<Renderer> ().sharedMaterial;


				if (material == areaController.gridHigh) 
				{
					totalWeightedArea += 3;
				} 
				else if (material == areaController.gridMedium) 
				{
					totalWeightedArea += 2;
				}				
				else if (material == areaController.gridLow) 
				{
					totalWeightedArea += 1;
				}
				else if (material == areaController.gridNone) 
				{
					totalWeightedArea += 0.5f;
				}
			}
		}

		return weightedCameraAreaCovered / totalWeightedArea;
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
				grid.GetComponent<Renderer> ().sharedMaterial = originalMaterials [mapGrid.IndexOf (grid)];
			
		}
	}
}
