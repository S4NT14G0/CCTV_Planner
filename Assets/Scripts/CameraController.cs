using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	private bool isBeingMoved;
	private Vector3 priorFramePosition;
    private Quaternion priorFrameRotation;
	public Transform lens;

    public delegate void OnDetected(List<GameObject> detectedGrids);
    public event OnDetected onDetected;

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
		priorFramePosition = transform.position;
        priorFrameRotation = transform.rotation;
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

        List<GameObject> detectedGrids = new List<GameObject>();

        for (int i = 0; i < numLines; i++)
        {
            Vector3 shootVec =  Quaternion.AngleAxis(-1 * arcAngle / 2 + (i * arcAngle / numLines), transform.up) * transform.forward;

            RaycastHit[] hits;
            hits = Physics.RaycastAll(lens.position, shootVec, 20f);

            for (int j = 0; j < hits.Length; j++)
            {
                RaycastHit hit = hits[j];

                if (!detectedGrids.Contains(hit.transform.gameObject))
                    detectedGrids.Add(hit.transform.gameObject);

                Debug.DrawLine(lens.position, hit.point, Color.green);
            }

            if (onDetected != null)
                onDetected(detectedGrids);

        }

    }
}
