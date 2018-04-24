using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementController : MonoBehaviour {

	[SerializeField]
	Canvas cameraCanvas;

    [SerializeField]
    CameraController cameraPrefab;

    AreaController areaController;
	Phase2CanvasController cameraBankController;

	// All the active cameras
	List<CameraController> cameras;

	// Use this for initialization
	void Start ()
	{
        this.enabled = false;
		cameraCanvas.enabled = false;

		cameras = new List<CameraController> ();

        areaController = this.GetComponent<AreaController>();
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
                if (camera != null)
                    camera.onDetected -= OnGridDetectedByCCTV;
            }
        }
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
        CameraController camera = (CameraController)Instantiate(cameraPrefab, new Vector3(50f, 0.5f, 50f), new Quaternion(0f,0f,0f, 0f));
        cameras.Add(camera);
        camera.onDetected += OnGridDetectedByCCTV;
    }


    public void OnGridDetectedByCCTV (List<GameObject> detectedGrids)
    {

    }
    
}
