using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public float jumpPower;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Jump") || Input.touchCount > 0) {
			GetComponent<Rigidbody>().velocity = new Vector3(0, jumpPower, 0);
		}
	}

	void OnCollisionEnter (Collision other) {
		Application.LoadLevel(Application.loadedLevel);
	}
}
