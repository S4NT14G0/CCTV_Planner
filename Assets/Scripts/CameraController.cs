using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	private bool isBeingMoved;
	private Transform priorFrameTransform;
	public Transform lens;

	bool IsBeingMoved {
		get { return this.IsBeingMoved; }

		set {

			Debug.Log (value);

			this.isBeingMoved = value; 
		}
	}

	// Use this for initialization
	void Start () {
		IsBeingMoved = false;
		priorFrameTransform = this.transform;
	}
	
	// Update is called once per frame
	void Update () {

		if (Vector3.Distance(lens.transform.position, priorFrameTransform.position) > 0.1f ) {
			IsBeingMoved = true;
		} else {
			IsBeingMoved = false;
		}

		priorFrameTransform = this.transform;
	}

	void ShootRay () {

		float arcAngle = 180.0f;
		int numLines = 180;

		for (int i = 0; i < numLines; i++) {
			Vector3 shootVec = lens.rotation * Quaternion.AngleAxis(-1*arcAngle/2+(i*arcAngle/numLines), lens.transform.up) * lens.transform.forward;

			RaycastHit hit;

			if (Physics.Raycast(lens.position, shootVec, out hit, 200.0f)) {
				Debug.DrawLine(lens.position, hit.point + new Vector3(0,0,20f), Color.green);

				if (hit.transform != null) {
					Debug.Log ("Gottem");
				}
			}
		}
	}
}
