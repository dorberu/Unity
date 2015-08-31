﻿using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	GameObject cameraParent;

	Vector3 defaultPosition;
	Quaternion defaultRotation;
	float defaultZoom;

	// Use this for initialization
	void Start () {
		cameraParent = GameObject.Find ("CameraParent");

		// default
		defaultPosition = Camera.main.transform.position;
		defaultRotation = Camera.main.transform.rotation;
		defaultZoom = Camera.main.fieldOfView;
	}
	
	// Update is called once per frame
	void Update () {
		// Translate
		if ( Input.GetMouseButton (0) ) {
			Camera.main.transform.Translate(Input.GetAxisRaw("Mouse X") / 10,
			                                Input.GetAxisRaw("Mouse Y") / 10,
			                                0);
		}

		// Rotate
		if ( Input.GetMouseButton (1) ) {
			cameraParent.transform.Rotate(Input.GetAxisRaw("Mouse Y") * 10,
			                             Input.GetAxisRaw("Mouse X") * 10,
			                             0);
		}

		// Zoom
		Camera.main.fieldOfView += (20 * Input.GetAxis ("Mouse ScrollWheel"));
		if (Camera.main.fieldOfView < 10) {
			Camera.main.fieldOfView = 10;
		}

		// reset
		if ( Input.GetMouseButton(2) ) {
			Camera.main.transform.position = defaultPosition;
			cameraParent.transform.rotation = defaultRotation;
			Camera.main.fieldOfView = defaultZoom;
		}
	}
}
