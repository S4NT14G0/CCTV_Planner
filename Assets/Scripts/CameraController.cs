using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

    public Transform lens;
    public Canvas rotateCanvas;
    public Button btnClockwise, btnCounterClockwise;

    private bool isBeingMoved;
	private Vector3 priorFramePosition;
    private Quaternion priorFrameRotation;


	public delegate void DoneMoving(List<GameObject> detectedGrids);
    public event DoneMoving doneMoving;

	public delegate void ClearCameraList(List<GameObject> removedGrids);
	public event ClearCameraList clearCameraList;

	public List<GameObject> detectedGrids;

    private bool isSelected;

    public bool IsSelected
    {
        get { return isSelected; }

        set
        {
            this.rotateCanvas.enabled = value;

            isSelected = value;
        }
    }

	bool IsBeingMoved {
		get { return this.isBeingMoved; }

		set {
            if (this.isBeingMoved && !value)
                ShootRay();
            this.isBeingMoved = value; 
		}
	}

	// Use this for initialization
	void Start () {
		IsBeingMoved = false;
        IsSelected = false;
		priorFramePosition = transform.position;
        priorFrameRotation = transform.rotation;
		detectedGrids = new List<GameObject> ();

        btnClockwise.onClick.AddListener(() => {
            // Rotate 15 deg clockwise
            this.transform.Rotate(new Vector3(0, 15f));
        });

        btnCounterClockwise.onClick.AddListener(() => {
            // Rotate 15 deg counterclockwise
            this.transform.Rotate(new Vector3(0, - 15f));
        });

        ShootRay();
	}
	
	// Update is called once per frame
	void Update () {

		if ((Vector3.Distance(transform.position, priorFramePosition) > 0.01f) || Quaternion.Angle(transform.rotation, priorFrameRotation) > 0.01f) {
			IsBeingMoved = true;
		} else {
            if (IsBeingMoved)
			    IsBeingMoved = false;
		}

        priorFramePosition = transform.position;
        priorFrameRotation = transform.rotation;
	}

	void ShootRay () {
        float arcAngle = 100;
        int numLines = 25;

		if (clearCameraList != null)
			clearCameraList (detectedGrids);
		
		detectedGrids.Clear ();

        for (int i = 0; i < numLines; i++)
        {
            Vector3 shootVec =  Quaternion.AngleAxis(-1 * arcAngle / 2 + (i * arcAngle / numLines), transform.up) * transform.forward;

            RaycastHit[] hits;
            hits = Physics.RaycastAll(lens.position, shootVec, 17f);

            for (int j = 0; j < hits.Length; j++)
            {
                RaycastHit hit = hits[j];

				if (!detectedGrids.Contains (hit.transform.gameObject)) 
				{
					detectedGrids.Add(hit.transform.gameObject);
				}


                Debug.DrawLine(lens.position, hit.point, Color.green);
            }
        }

		if (doneMoving != null)
			doneMoving(detectedGrids);
    }
}
